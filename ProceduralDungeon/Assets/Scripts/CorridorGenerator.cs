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
                currentPosition = path[path.Count - 1]; // -1 aby dal�� currentPosition byla stejn� jako posledn� pozice path (po�ad� v listu jde od 0, Count za��n� od 1)
                potentialRoomPositions.Add(currentPosition);
                floorPositions.UnionWith(path);

                int number = Random.Range(0, 3); // tento blok �e��, aby se nevytvo�ily dva corridory v sob� (nap�. jeden by se vygeneroval doprava, a druh� by se vygeneroval zp�tky doleva)
                if (number == 1) direction = Direction2D.TurnLeft(direction); // n�hodn� se vyb�r�, zdali sm�r vektoru dal��ho corridoru se oto�� o 90� doprava, doleva nebo bude pokra�ovat ve stejn�m sm�ru (rovn�)
                else if (number == 2) direction = Direction2D.TurnRight(direction); // d�ky tomu se v�ak nikdy neza�ne vracet zp�t (opa�n�m sm�rem) na p�vodn� za��tek
            }
            currentPosition = potentialRoomPositions.ElementAt(Random.Range(0, potentialRoomPositions.Count()));
        }
    }

    public static HashSet<Vector2Int> GenerateRooms(HashSet<Vector2Int> potentialRoomPositions) // vrac� pozice m�stnost�
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomsCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent); // vybere po�et m�stnost� podle roomPercent

        List<Vector2Int> rooms = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomsCount).ToList(); // nejd��ve se�ad� prvky z potentialRoomPositions pomoc� guid 
                                                                                                                // (guid ka�d�mu prvku p�i�ad� n�hodn� jedine�n� id, d�ky �emu� se prvky se�ad� n�hodn�)
                                                                                                                // pot� z nich vybere n�kolik prvk� podle roomsCount
        foreach (Vector2Int roomPosition in rooms)
        {
            HashSet<Vector2Int> roomFloor = DungeonGenerator.instance.RunRandomWalk(roomPosition);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }
}
