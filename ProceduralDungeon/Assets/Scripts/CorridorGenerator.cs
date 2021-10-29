using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CorridorGenerator
{
    public static Vector2Int startPosition;
    public static int corridorCount, corridorLength;

    public static void GenerateCorridors(HashSet<Vector2Int> floorPositions)
    {
        Vector2Int currentPosition = startPosition;
        Vector2Int direction = Direction2D.GetRandomDirection();

        for (int i = 0; i < corridorCount; i++)
        {
            List<Vector2Int> path = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength, direction);
            currentPosition = path[path.Count - 1]; // -1 aby dal�� currentPosition byla stejn� jako posledn� pozice path (po�ad� v listu jde od 0, Count za��n� od 1)
            floorPositions.UnionWith(path);

            int number = Random.Range(0, 3); // tento blok �e��, aby se nevytvo�ily dva corridory v sob� (nap�. jeden by se vygeneroval doprava, a druh� by se vygeneroval zp�tky doleva)
            if (number == 1) direction = Direction2D.TurnLeft(direction); // n�hodn� se vyb�r�, zdali sm�r vektoru dal��ho corridoru se oto�� o 90� doprava, doleva nebo bude pokra�ovat ve stejn�m sm�ru (rovn�)
            else if (number == 2) direction = Direction2D.TurnRight(direction); // d�ky tomu se v�ak nikdy neza�ne vracet zp�t (opa�n�m sm�rem) na p�vodn� za��tek
        }
    }
}
