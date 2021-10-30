using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CorridorGenerator
{
    public static Vector2Int startPosition;
    public static int corridorLength;
    public static int[] branches;

    [Range(0.1f, 1)]
    public static float roomPercent;

    public static void GenerateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        Vector2Int currentPosition = startPosition;
        Vector2Int direction = Direction2D.GetRandomDirection();
        potentialRoomPositions.Add(currentPosition);

        for (int x = 0; x < branches.Length; x++)
        {
            for (int i = 0; i < branches[x]; i++)
            {
                List<Vector2Int> path = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength, direction);
                currentPosition = path[path.Count - 1]; // -1 aby další currentPosition byla stejná jako poslední pozice path (poøadí v listu jde od 0, Count zaèíná od 1)
                potentialRoomPositions.Add(currentPosition);
                floorPositions.UnionWith(path);

                int number = Random.Range(0, 3); // tento blok øeší, aby se nevytvoøily dva corridory v sobì (napø. jeden by se vygeneroval doprava, a druhý by se vygeneroval zpátky doleva)
                if (number == 1) direction = Direction2D.TurnLeft(direction); // náhodnì se vybírá, zdali smìr vektoru dalšího corridoru se otoèí o 90° doprava, doleva nebo bude pokraèovat ve stejném smìru (rovnì)
                else if (number == 2) direction = Direction2D.TurnRight(direction); // díky tomu se však nikdy nezaène vracet zpìt (opaèným smìrem) na pùvodní zaèátek
            }
            currentPosition = potentialRoomPositions.ElementAt(Random.Range(0, potentialRoomPositions.Count()));
        }
    }

    public static HashSet<Vector2Int> GenerateRooms(HashSet<Vector2Int> potentialRoomPositions) // vrací pozice místností
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomsCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent); // vybere poèet místností podle roomPercent

        List<Vector2Int> rooms = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomsCount).ToList(); // nejdøíve seøadí prvky z potentialRoomPositions pomocí guid 
                                                                                                                // (guid každému prvku pøiøadí náhodné jedineèné id, díky èemuž se prvky seøadí náhodnì)
                                                                                                                // poté z nich vybere nìkolik prvkù podle roomsCount
        foreach (Vector2Int roomPosition in rooms)
        {
            HashSet<Vector2Int> roomFloor = DungeonGenerator.instance.RunRandomWalk(roomPosition);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }
}
