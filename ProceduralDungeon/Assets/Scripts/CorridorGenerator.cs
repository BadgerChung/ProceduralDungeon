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
            currentPosition = path[path.Count - 1]; // -1 aby další currentPosition byla stejná jako poslední pozice path (poøadí v listu jde od 0, Count zaèíná od 1)
            floorPositions.UnionWith(path);

            int number = Random.Range(0, 3); // tento blok øeší, aby se nevytvoøily dva corridory v sobì (napø. jeden by se vygeneroval doprava, a druhý by se vygeneroval zpátky doleva)
            if (number == 1) direction = Direction2D.TurnLeft(direction); // náhodnì se vybírá, zdali smìr vektoru dalšího corridoru se otoèí o 90° doprava, doleva nebo bude pokraèovat ve stejném smìru (rovnì)
            else if (number == 2) direction = Direction2D.TurnRight(direction); // díky tomu se však nikdy nezaène vracet zpìt (opaèným smìrem) na pùvodní zaèátek
        }
    }
}
