import argparse
import binascii
import ConfigParser

import lego_game
import lego_transport

from twisted.internet import protocol, reactor

from lego.ttypes import ClientActions

SECTION_WORLD = 'world'
        
class LegoProtocol(protocol.Protocol): 
    def __init__(self, state, handlers, users):
        self.users = users
        self.handlers = handlers
        self.state = state
        self.message_to_class = {}
        self.message_to_handler = {}
        for handler in handlers:
            self.message_to_class[handler.message_id] = handler.transport_class
            self.message_to_handler[handler.message_id] = handler
    
    def connectionMade(self):
        print("Connection made")
        
    def dataReceived(self, data):
        #~ print("Data received {0}".format(binascii.hexlify(data)))
        client_actions = ClientActions()
        lego_transport.read_binary(client_actions, data)
        
        player_id = client_actions.playerId
        self.name = player_id
        self.users[self.name] = self
        print("Player {0}".format(player_id))
        tuples = lego_transport.read_custom(client_actions.actions, self.message_to_class)
        queue = []
        for id, tbase in tuples:
            print(tbase)
            self.message_to_handler[id].handle(self.state, player_id, tbase, queue)
            
        queue.extend(self.state.players[player_id].pending)
        self.state.players[player_id].pending = []
        if queue:
            self.write(lego_transport.write_custom(queue))
            
        for name, protocol in self.users.iteritems():
            if protocol != self:
                protocol.write(lego_transport.write_custom(self.state.players[name].pending))
                self.state.players[name].pending = []
        
    def connectionLost(self, reason = protocol.connectionDone):
        print("Connection lost")
        if self.users.has_key(self.name):
            del self.users[self.name]
        
    def write(self, data):
        #~ print("Response {0}".format(binascii.hexlify(data)))
        self.transport.write(data)
        
class LegoFactory(protocol.ServerFactory):
    def __init__(self, state):
        self.handlers = [lego_game.PlayerConnectHandler(), lego_game.BrickUpdateHandler()]
        self.state = state
        self.users = {}
        
    def buildProtocol(self, addr):
        return LegoProtocol(self.state, self.handlers, self.users)

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--config', '-c', required = True)
    arguments = parser.parse_args()
    
    config = ConfigParser.ConfigParser()
    config.read(arguments.config)
    rows = config.getint(SECTION_WORLD, 'rows')
    columns = config.getint(SECTION_WORLD, 'columns')

    game_state = lego_game.GameState(rows, columns)
    factory = LegoFactory(game_state)

    reactor.listenTCP(8123, factory)
    reactor.run()
    