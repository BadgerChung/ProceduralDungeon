using System.Collections.Generic;
using UnityEngine;

public static class Direction2D
{
    public static List<Vector2Int> directionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // nahoru
        new Vector2Int(0, -1), // dolů
        new Vector2Int(1, 0), // doprava
        new Vector2Int(-1, 0) // doleva
    };

    public static Vector2Int GetRandomDirection()
    {
        return directionsList[Random.Range(0, directionsList.Count)];
    }

    public static Vector2Int TurnRight(Vector2Int lastDirection)
    {
        return new Vector2Int(lastDirection.y, -lastDirection.x);
    }

    public static Vector2Int TurnLeft(Vector2Int lastDirection)
    {
        return new Vector2Int(-lastDirection.y, lastDirection.x);
    }
}