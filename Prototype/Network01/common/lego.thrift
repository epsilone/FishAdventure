namespace java com.funcom.lego.generated
namespace csharp com.funcom.lego.generated

typedef string uuid

enum MessageId {
    PLAYER_CONNECT,
    PLAYER_CONNECTED,
    BRICK_UPDATE,
    WORLD_INFO,
}

enum BrickColor {
    WHITE,
    RED,
    BLUE,
    GREEN,
    YELLOW
}

struct ClientActions {
    1: required uuid playerId,
    2: required binary actions,
}

struct BrickUpdate {
    1: required i32 row,
    2: required i32 column,
    3: required BrickColor color,
}

struct WorldInfo {
    1: required list<BrickColor> bricks,
    2: required i32 rows,
    3: required i32 columns,
}

struct PlayerConnect {
    1: required uuid id,
    2: required string name,
}

struct PlayerConnected {
    1: required BrickColor color,
    2: required string name,
}
