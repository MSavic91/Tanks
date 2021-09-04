public enum TileType
{
    Clear = 1,
    Base = 2,
    PlayerStart = 4,
    EnemyStart = 8,
    Wall = 16,
    Destructible = 32
}

public enum Direction
{
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8
}

public enum Sounds
{
    Click = 0,
    BaseDestroy = 1,
    DestroyableDestroy = 2,
    GameStart = 3,
    MissionFail = 4,
    MissionSuccess = 5,
    TankMove = 6,
    TankShoot = 7,
    TankDestroy = 8,
    WallHit = 9
}
