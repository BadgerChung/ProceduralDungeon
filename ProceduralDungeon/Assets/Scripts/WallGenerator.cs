using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void GenerateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var wallPositions = GetWallPositions(floorPositions, Direction2D.directionsList);
        foreach (var position in wallPositions)
        {
            tilemapVisualizer.GenerateSingleWall(position);
        }
    }

    private static HashSet<Vector2Int> GetWallPositions(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionsList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionsList)
            {
                var neighbourPosition = position + direction; // do neighbourPosition se dá pozice z floorPositions z prního foreach rozšíøená postupnì do všech stran pøidáním direction
                                                              // (ve výsledku se prohledají pozice do všech stran od dané pozice podlahy, 
                                                              // takže kolem každého podlahového tilu prohledá všechny potenciální zdi, které zkontroluje v if níže)
                if (floorPositions.Contains(neighbourPosition) == false) // tímto zjistíme, zdali daná neighbourPosition není pozicí podlahy, pokud není, je to zeï
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }

        return wallPositions;
    }
}
