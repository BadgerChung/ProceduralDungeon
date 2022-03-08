using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ObjectGenerator
{

    private static List<List<Vector2Int>> validObjectPositions;

    private static bool initialized = false;

    public static void InitObjectGenerator(List<HashSet<Vector2Int>> roomsPositions)
    {
        validObjectPositions = new List<List<Vector2Int>>();

        foreach(HashSet<Vector2Int> roomPositions in roomsPositions)
        {
            if (roomPositions == roomsPositions[0]) continue;
            List<Vector2Int> validPositions = new List<Vector2Int>();

            foreach(Vector2Int position in roomPositions)
            {
                if(HasAllNeighbours(position, roomPositions))
                {
                    validPositions.Add(position);
                }
            }

            validObjectPositions.Add(validPositions);
        }
        initialized = true;
    }

    private static bool HasAllNeighbours(Vector2Int position, HashSet<Vector2Int> positions)
    {
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int currentPosition = position + new Vector2Int(x, y);
                if(!positions.Contains(currentPosition)) {
                    return false;
                }
            }
        }
        return true;
    }

    private static void RemoveAllNeighbours(Vector2Int position, List<Vector2Int> positions)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int currentPosition = position + new Vector2Int(x, y);
                /*if(positions.Contains(currentPosition))*/ positions.Remove(currentPosition);
            }
        }
    }

    public static HashSet<Vector2Int> GenerateChestPositions()
    {
        if (!initialized) throw new InvalidOperationException("OBJECT GENERATOR NENÍ INICIALIZOVÁN");

        HashSet<Vector2Int> chestPositions = new HashSet<Vector2Int>();

        foreach (List<Vector2Int> validPositions in validObjectPositions)
        {

            Vector2Int validPos = validPositions[Random.Range(0, validPositions.Count)];
            chestPositions.Add(validPos);
            RemoveAllNeighbours(validPos, validPositions);
        }

        return chestPositions;
    }

    public static HashSet<Vector2Int> GenerateEnemyPositions()
    {
        if (!initialized) throw new InvalidOperationException("OBJECT GENERATOR NENÍ INICIALIZOVÁN");

        HashSet<Vector2Int> enemyPositions = new HashSet<Vector2Int>();

        foreach (List<Vector2Int> validPositions in validObjectPositions)
        {
            int enemyCount = Random.Range(0, 5);

            for (int i = 0; i < enemyCount; i++)
            {
                Vector2Int validPos = validPositions[Random.Range(0, validPositions.Count)];
                enemyPositions.Add(validPos);
                RemoveAllNeighbours(validPos, validPositions);
            }
            
            
        }

        return enemyPositions;
    }

    public static Vector2Int GetPortalPosition()
    {
        List<Vector2Int> room = validObjectPositions[Random.Range(1, validObjectPositions.Count)];
        Vector2Int pos = room[Random.Range(0, room.Count)];
        RemoveAllNeighbours(pos, room);
        return pos;
    }
}
