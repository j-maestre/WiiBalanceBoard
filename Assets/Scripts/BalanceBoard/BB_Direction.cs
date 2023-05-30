using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Define a fixed possible directions 
/// </summary>
public enum Direction {
    LEFT,
    RIGHT,
    FORWARD,
    BACK,
    NONE,
    OFF,
    COUNT
}

/// <summary>
/// Define set controller types
/// </summary>
public enum DirectionType {
    LEFT_RIGHT,
    FORWARD_BACK,
    FOUR_WAY,
    COUNT
}
