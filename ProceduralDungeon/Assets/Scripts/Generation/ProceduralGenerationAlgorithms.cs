using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector2Int> RandomWalk(Vector2Int startPosition, int walkLength) // vrac� n�hodn� pozice co dohromady tvo�� "cestu"
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);

        Vector2Int previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            Vector2Int newPosition = previousPosition + Direction2D.GetRandomDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength, Vector2Int direction) // pou��v� se list, proto�e hashset neuchov�v� po�ad� element�,
                                                                                                                          // a v tomto p��pad� je pot�eba zachovat prvn� a posledn� pozici
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        Vector2Int currentPosition = startPosition;

        corridor.Add(currentPosition); // bez tohoto by se prvn� startPosition nep�idala do listu, proto�e u� by pro�la for

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }
}