namespace java com.funcom.lego.generated
namespace csharp Funcom.Lego.Thrift.Generated

typedef string uuid

enum MessageId {
    RETRIEVE_GAMES,
    CREATE_GAME,
    JOIN_GAME,
    LEAVE_GAME,
    
    // Events
    AVAILABLE_GAMES,
    GAME_JOINED,
    PLAYER_CONNECTED,
    PLAYER_DISCONNECTED,
    BRICK_UPDATE,
    GAME_END,
    
    ERROR,
}

enum BrickColor {
    WHITE,
    RED,
    BLUE,
    GREEN,
    YELLOW
}

enum NetErrorStatus {
    SERVER_ERROR,
}

struct ClientActions {
    1: required uuid playerId,
    2: required binary actions,
}

struct NetError {
    1: required NetErrorStatus status,
    2: optional string name,
}

// Lobby
struct RetrieveGames {
    1: required uuid playerId,
    2: required bool full,
}

struct NetGameInfo {
    1: required uuid gameId,
    2: required string name,
    3: required string host,
    4: required i32 port,
    5: required bool visible,
    6: required i32 maxPlayers,
    7: required list<string> players,
}

struct NetGames {
    1: required list<NetGameInfo> gameInfos,
}

struct CreateGame {
    1: required string playerName,
    2: required string gameName,
    3: required bool visible,
    4: required i32 maxPlayers,
}

struct JoinGame {
    1: required uuid gameId,
    2: required string playerName,
}

// Game
struct WorldInfo {
    1: required list<BrickColor> bricks,
    2: required i32 rows,
    3: required i32 columns,
}

struct PlayerConnected {
    1: required uuid playerId,
    2: required string name,
    3: required BrickColor color,
}

struct GameJoined {
    1: required WorldInfo world,
    2: required list<PlayerConnected> players,
}

// Game event
struct BrickUpdate {
    1: required i32 row,
    2: required i32 column,
    3: required BrickColor color,
}

struct PlayerDisconnect {
    1: required uuid playerId,
}
