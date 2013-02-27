namespace csharp PaddleThrift

typedef string uuid

enum MessageId {
    GAME_START,
    ENTITY_POSITION,
    POINTS_UPDATE,
    GAME_OVER,
}

struct PaddlePacket {
    1: required i32 count,
    2: required binary rawData,
}

struct GameStart {
    1: required i64 serverTime,
    2: required double ballDisplacementX,
    3: required double ballDisplacementY,
    4: required i32 playerId,
}

struct EntityPosition {
    1: required string name,
    2: required double positionX,
    3: required double positionY,
}

struct PointsUpdate {
    1: required i32 playerOneScore,
    2: required i32 playerTwoScore,
}

struct GameOver {
    
}
