import sys
sys.path.append('gen-py')

from lego.ttypes import BrickColor
from lego.ttypes import BrickUpdate
from lego.ttypes import MessageId
from lego.ttypes import PlayerConnect
from lego.ttypes import PlayerConnected
from lego.ttypes import WorldInfo

class Player(object):
    def __init__(self):
        self.id = None
        self.name = None
        self.color = None
        self.pending = []

class GameState(object):
    def __init__(self, rows, columns):
        world = WorldInfo()
        world.rows = rows
        world.columns = columns
        world.bricks = [BrickColor.WHITE for c in range(rows * columns)]
        
        self.world = world
        self.players = {}
        
        def next_color():
            colors = (BrickColor.RED, BrickColor.BLUE, BrickColor.GREEN, BrickColor.YELLOW)
            for c in colors:
                yield c
        self.colors = next_color()
        
    def brick(self, column, row):
        return self.world.bricks[column + row * self.world.rows]
        
    def brick(self, column, row, color):
        self.world.bricks[column + row * self.world.rows] = color
        
    def is_complete(self):
        for b in world.bricks:
            if b == BrickColor.WHITE:
                return False
        
        return True
        
    def add_player(self, id, name):
        added = False
        player = self.players.get(id)
        if not player:
            try:
                player = Player()
                player.id = id
                player.name = name
                player.color = self.colors.next()
                self.players[id] = player
                added = True
            except StopIteration:
                print("Could not add player with id {0}, {1}".format(id, name))
                
        return added
        
    def get_player(self, id):
        return self.players.get(id)
        
    def add_player_action(self, id, action):
        for player_id, player in self.players.iteritems():
            if player_id != id:
                player.pending.append(action)
                

class PlayerConnectHandler(object):
    def __init__(self):
        self.message_id = MessageId.PLAYER_CONNECT
        self.transport_class = PlayerConnect
        
    def handle(self, state, player_id, message, queue):
        state.add_player(player_id, message.name)
        
        # Add the player connected response so he can use the proper color
        player = state.get_player(message.id)
        connected = PlayerConnected()
        connected.color = player.color
        connected.name = player.name
        queue.append((MessageId.PLAYER_CONNECTED, connected))
        
        # Add the action so every user can know of it
        state.add_player_action(player_id, (MessageId.PLAYER_CONNECTED, connected))
        
        # Add the full world
        world = WorldInfo()
        world = state.world
        queue.append((MessageId.WORLD_INFO, world))
        
class BrickUpdateHandler(object):
    def __init__(self):
        self.message_id = MessageId.BRICK_UPDATE
        self.transport_class = BrickUpdate
        
    def handle(self, state, player_id, message, queue):
        # Update the world overiding previous changes
        state.brick(message.row, message.column, message.color)
        
        # Add the action so every user can know of it
        state.add_player_action(player_id, (MessageId.BRICK_UPDATE, message))

if __name__ == '__main__':
    world = WorldInfo()
    world.rows = 15
    world.columns = 15
    world.bricks = [BrickColor.WHITE for i in range(world.rows * world.columns)]
    
    game_state = GameState(world)
    
