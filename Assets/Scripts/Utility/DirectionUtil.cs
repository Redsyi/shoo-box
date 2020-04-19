using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionUtil : MonoBehaviour
{
    public static Direction Opposite(Direction direction)
    {
        if (direction == Direction.NORTH)
            return Direction.SOUTH;
        else if (direction == Direction.SOUTH)
            return Direction.NORTH;
        else if (direction == Direction.EAST)
            return Direction.WEST;
        else
            return Direction.EAST;
    }

    public static Direction Left(Direction direction)
    {
        if (direction == Direction.NORTH)
            return Direction.WEST;
        else if (direction == Direction.SOUTH)
            return Direction.EAST;
        else if (direction == Direction.EAST)
            return Direction.NORTH;
        else
            return Direction.SOUTH;
    }

    public static Direction Right(Direction direction)
    {
        if (direction == Direction.NORTH)
            return Direction.EAST;
        else if (direction == Direction.SOUTH)
            return Direction.WEST;
        else if (direction == Direction.EAST)
            return Direction.SOUTH;
        else
            return Direction.NORTH;
    }

    public static void FillDict<T>(Dictionary<Direction, T> dictionary, T value)
    {
        dictionary[Direction.EAST] = value;
        dictionary[Direction.WEST] = value;
        dictionary[Direction.NORTH] = value;
        dictionary[Direction.SOUTH] = value;
    }
}
