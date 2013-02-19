import argparse
import binascii
import ConfigParser
import logging

import lego_game
import lego_transport

from twisted.internet import protocol, reactor

from lego.ttypes import *

SECTION_WORLD = 'world'

LOG_FORMAT = '%(asctime)-15s %(message)s'
logging.basicConfig(format=LOG_FORMAT)
LOGGER = logging.getLogger('lego_server')
LOGGER.setLevel(logging.DEBUG)
        
class LegoProtocol(protocol.Protocol): 
    def __init__(self, registry, handlers):
        self.registry = registry
        self.handlers = handlers
        self.queue = []
        self.game = None
        self.player_id = None
        self.__message_to_class = {}
        self.__message_to_handler = {}
        for handler in handlers:
            self.__message_to_class[handler.message_id] = handler.transport_class
            self.__message_to_handler[handler.message_id] = handler
    
    def connectionMade(self):
        LOGGER.info("Connection made")
        
    def dataReceived(self, data):
        #~ LOGGER.debug("Data received {0}".format(binascii.hexlify(data)))
        client_actions = ClientActions()
        lego_transport.read_binary(client_actions, data)
        
        self.player_id = client_actions.playerId
        LOGGER.info("Player {0}".format(self.player_id))
        tuples = lego_transport.read_custom(client_actions.actions, self.__message_to_class)
        queue = []
        for id, tbase in tuples:
            print(tbase)
            self.__message_to_handler[id].handle(self, self.player_id, tbase)
            
        # Make sure nothing stays
        if self.queue:
            LOGGER.debug(self.queue)
            self.write(lego_transport.write_custom(self.queue))
            self.queue = []
            
        if self.game:
            for id, player in self.game.players.iteritems():
                print("try broadcast", id, player, player.protocol.queue)
                if player.protocol.queue:
                    if player.protocol != self:
                        print("broadcasting", id, player)
                        player.protocol.write(lego_transport.write_custom(player.protocol.queue))
                        player.protocol.queue = []
        
    def connectionLost(self, reason = protocol.connectionDone):
        LOGGER.info("Connection lost")
        if self.game:
            message = PlayerDisconnect()
            message.playerId = self.player_id
            self.__message_to_handler[MessageId.LEAVE_GAME].handle(self, self.player_id, message)
        
    def write(self, data):
        #~ LOGGER.debug("Response {0}".format(binascii.hexlify(data)))
        self.transport.write(data)
        
class LegoFactory(protocol.ServerFactory):
    def __init__(self, state):
        self.registry = {}
        self.handlers = [lego_game.ListGameHandler(), lego_game.JoinGameHandler(),  lego_game.CreateGameHandler(), lego_game.BrickUpdateHandler(), lego_game.LeaveGameHandler()]
        
    def buildProtocol(self, addr):
        return LegoProtocol(self.registry, self.handlers)

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
    