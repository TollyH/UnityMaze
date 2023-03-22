public enum MoveEvent
{
    Moved,
    MovedGridDiagonally,
    AlternateCoordChosen,
    Pickup,
    PickedUpKey,
    PickedUpKeySensor,
    PickedUpGun,
    Won,
    MonsterCaught
}

public enum WallDirection
{
    North,
    East,
    South,
    West
}

public enum RequestType
{
    Ping,
    Join,
    Fire,
    Respawn,
    Leave
}

public enum ShotResponse
{
    Denied,
    Missed,
    HitNoKill,
    Killed
}
