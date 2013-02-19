import binascii
import logging
import random
import uuid

from functools import partial

import lego_transport

from twisted.internet import defer, protocol, reactor

import sys
sys.path.append('gen-py')

from lego.ttypes import *

LOG_FORMAT = '%(asctime)-15s %(message)s'
logging.basicConfig(format=LOG_FORMAT)
LOGGER = logging.getLogger('lego_client')
LOGGER.setLevel(logging.DEBUG)

MAPPING = {
    MessageId.AVAILABLE_GAMES : NetGames,
    MessageId.GAME_JOINED : GameJoined,
    MessageId.PLAYER_CONNECTED : PlayerConnected,
    MessageId.PLAYER_DISCONNECTED : PlayerDisconnect,
    MessageId.BRICK_UPDATE : BrickUpdate,
    MessageId.ERROR : NetError
}

class State(object):
    def __init__(self):
        self.player_id = str(uuid.uuid4())
        self.world = None
        self.color = None
        
    def retrieve_games(self):
        message = RetrieveGames()
        message.playerId = self.player_id
        message.full = False
        
        raw = lego_transport.write_custom(((MessageId.RETRIEVE_GAMES, message),))
        LOGGER.debug("Data packed {0}".format(binascii.hexlify(raw)))
        return raw
        
    def create_game(self):
        message = CreateGame()
        message.playerName = self.player_id
        message.gameName = "Test Game by {0}".format(self.player_id)
        message.visible = True
        message.maxPlayers = 4
        
        raw = lego_transport.write_custom(((MessageId.CREATE_GAME, message),))
        LOGGER.debug("Data packed {0}".format(binascii.hexlify(raw)))
        return raw
        
    def join_game(self, game_id):
        message = JoinGame()
        message.gameId = game_id
        message.playerName = self.player_id
        
        raw = lego_transport.write_custom(((MessageId.JOIN_GAME, message),))
        LOGGER.debug("Data packed {0}".format(binascii.hexlify(raw)))
        return raw
        
    def brick_update(self):
        message = BrickUpdate()
        message.row = random.randint(0, self.world.rows - 1)
        message.column = random.randint(0, self.world.columns - 1)
        message.color = self.color
        
        raw = lego_transport.write_custom(((MessageId.BRICK_UPDATE, message),))
        LOGGER.debug("Data packed {0}".format(binascii.hexlify(raw)))
        return raw
        
    def onResponse(self, messages, protocol):
        for message_id, message in messages:
            LOGGER.debug("{0}: {1}".format(message_id, message))
            if message_id == MessageId.AVAILABLE_GAMES:
                LOGGER.info("Available games")
                if message.gameInfos:
                    protocol.sendActions(partial(self.join_game, message.gameInfos[0].gameId))
                else:
                    protocol.sendActions(self.create_game)
            elif message_id == MessageId.GAME_JOINED:
                LOGGER.info("Game joined")
                self.world = message.world
                for p in message.players:
                    if p.playerId == self.player_id:
                        self.color = p.color
                protocol.sendActions(self.brick_update)
            elif message_id == MessageId.PLAYER_CONNECTED:
                LOGGER.info("Player connected")
            elif message_id == MessageId.PLAYER_DISCONNECTED:
                LOGGER.info("Player disconnected")
            elif message_id == MessageId.BRICK_UPDATE:
                LOGGER.info("Brick update")
            elif message_id == MessageId.ERROR:
                LOGGER.info("Error")
                
class LegoProtocol(protocol.Protocol): 
    def __init__(self):
        self.state = State()
        
    def connectionMade(self):
        LOGGER.info("Connection made")
        self.sendActions(self.state.retrieve_games)
        
    def dataReceived(self, data):
        LOGGER.debug("Data received {0}".format(binascii.hexlify(data)))
        tuples = lego_transport.read_custom(data, MAPPING)
        self.state.onResponse(tuples, self)
        
    def connectionLost(self, reason = protocol.connectionDone):
        LOGGER.info("Connection lost")
        
    def sendActions(self, method):
        client_actions = ClientActions()
        client_actions.playerId = self.state.player_id
        client_actions.actions = method()
        data = lego_transport.write_binary(client_actions)
        LOGGER.debug("Data sent {0}".format(binascii.hexlify(data)))
        self.transport.write(data)
        
class LegoFactory(protocol.ClientFactory):
    def startedConnecting(self, connector):
        LOGGER.info('Started to connect.')

    def buildProtocol(self, addr):
        LOGGER.info('Connected.')
        return LegoProtocol()

    def clientConnectionLost(self, connector, reason):
        LOGGER.info('Lost connection.  Reason: {0}'.format(reason))

    def clientConnectionFailed(self, connector, reason):
        LOGGER.info('Connection failed. Reason: {0}'.format(reason))
        
if __name__ == '__main__':
    reactor.connectTCP("localhost", 8123, LegoFactory())
    reactor.run()