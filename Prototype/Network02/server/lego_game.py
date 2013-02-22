import uuid

import sys
sys.path.append('gen-py')

from lego.ttypes import *

def next_color():
        colors = (BrickColor.RED, BrickColor.BLUE, BrickColor.GREEN, BrickColor.YELLOW)
        for c in colors:
            yield c
            
class LegoGame(object):
    def __init__(self, host, port, name, max_players, visible):
        self.host = host
        self.port = port
        self.name = name
        self.max_players = max_players
        self.visible = visible
        self.game_id = str(uuid.uuid4())
        self.state = GameState(15, 15)
        self.players = {}
        self.colors = next_color()
        
    def add_player(self, player_id, name, protocol):
        added = False
        player = self.players.get(id)
        if not player:
            try:
                player = Player()
                player.player_id = player_id
                player.name = name
                player.protocol = protocol
                player.color = self.colors.next()
                self.players[player_id] = player
                added = True
            except StopIteration:
                print("Could not add player with id {0}, {1}".format(player_id, name))
                
        return added
        
    def get_player(self, player_id):
        return self.players.get(player_id)
        
    def remove_player(self, player_id):
        if self.players.has_key(player_id):
            del self.players[player_id]
        
    def add_player_action(self, player_id, action):
        for other_id, player in self.players.iteritems():
            if player_id != other_id:
                player.protocol.queue.append(action)

class Player(object):
    def __init__(self):
        self.player_id = None
        self.name = None
        self.color = None
        self.protocol = None

class GameState(object):
    def __init__(self, rows, columns):
        world = WorldInfo()
        world.rows = rows
        world.columns = columns
        world.bricks = [BrickColor.WHITE for c in range(rows * columns)]
        self.world = world
        
    def brick(self, column, row):
        return self.world.bricks[column + row * self.world.rows]
        
    def brick(self, column, row, color):
        self.world.bricks[column + row * self.world.rows] = color
        
    def is_complete(self):
        for b in world.bricks:
            if b == BrickColor.WHITE:
                return False
        
        return True
        
class LegoHandler(object):
    def handle(self, game, player_id, message, queue):
        pass
        
class ListGameHandler(object):
    def __init__(self):
        self.message_id = MessageId.RETRIEVE_GAMES
        self.transport_class = RetrieveGames
        
    def handle(self, protocol, player_id, message):
        all_games = []
        for game in protocol.registry.itervalues():
            gameInfo = NetGameInfo()
            gameInfo.gameId = game.game_id
            gameInfo.name = game.name
            gameInfo.host = game.host
            gameInfo.port = game.port
            gameInfo.visible = game.visible
            gameInfo.maxPlayers = game.max_players
            gameInfo.players = [player.name for player in game.players.itervalues()]
            all_games.append(gameInfo)
        
        net_games = NetGames()
        net_games.gameInfos = all_games
        protocol.queue.append((MessageId.AVAILABLE_GAMES, net_games))
        
class JoinGameHandler(object):
    def __init__(self):
        self.message_id = MessageId.JOIN_GAME
        self.transport_class = JoinGame
        
    def handle(self, protocol, player_id, message):
        print("Joined game", message.gameId, player_id)
        game = protocol.registry.get(message.gameId)
        if game:
            game.add_player(player_id, message.playerName, protocol)
            protocol.game = game
            
            # Add the player connected response so he can use the proper color
            player = game.get_player(player_id)
            
            # Add the action so every user can know of it
            connected = PlayerConnected()
            connected.playerId = player.player_id
            connected.color = player.color
            connected.name = player.name
            game.add_player_action(player_id, (MessageId.PLAYER_CONNECTED, connected))
            
            net_game = GameJoined()
            net_game.world = game.state.world
            
            players = []
            for player in game.players.itervalues():
                connected = PlayerConnected()
                connected.playerId = player.player_id
                connected.color = player.color
                connected.name = player.name
                players.append(connected)
                
            net_game.players = players
            protocol.queue.append((MessageId.GAME_JOINED, net_game))
        
class CreateGameHandler(object):
    def __init__(self):
        self.message_id = MessageId.CREATE_GAME
        self.transport_class = CreateGame
        
    def handle(self, protocol, player_id, message):
        game = LegoGame("10.9.42.223", 8123, message.gameName, message.maxPlayers, message.visible)
        game.add_player(player_id, message.playerName, protocol)
        protocol.game = game
        print("Added game {0}".format(game.game_id))
        protocol.registry[game.game_id] = game
        
        net_game = GameJoined()
        net_game.world = game.state.world

        players = []
        for player in game.players.itervalues():
            connected = PlayerConnected()
            connected.playerId = player.player_id
            connected.color = player.color
            connected.name = player.name
            players.append(connected)
                
        net_game.players = players
        protocol.queue.append((MessageId.GAME_JOINED, net_game))
        
class BrickUpdateHandler(object):
    def __init__(self):
        self.message_id = MessageId.BRICK_UPDATE
        self.transport_class = BrickUpdate
        
    def handle(self, protocol, player_id, message):
        game = protocol.game
        if game:
            # Update the world overiding previous changes
            game.state.brick(message.row, message.column, message.color)
            
            # Add the action so every user can know of it
            game.add_player_action(player_id, (MessageId.BRICK_UPDATE, message))
            
class LeaveGameHandler(object):
    def __init__(self):
        self.message_id = MessageId.LEAVE_GAME
        self.transport_class = PlayerDisconnect
        
    def handle(self, protocol, player_id, message):
        game = protocol.game
        if game:
            protocol.game = None
            # Add the action so every user can know of it
            game.add_player_action(player_id, (MessageId.LEAVE_GAME, message))

if __name__ == '__main__':
    world = WorldInfo()
    world.rows = 15
    world.columns = 15
    world.bricks = [BrickColor.WHITE for i in range(world.rows * world.columns)]
    
    game_state = GameState(world)
    
