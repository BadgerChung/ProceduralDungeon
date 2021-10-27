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
                var neighbourPosition = position + direction; // do neighbourPosition se d� pozice z floorPositions z prn�ho foreach roz���en� postupn� do v�ech stran p�id�n�m direction
                                                              // (ve v�sledku se prohledaj� pozice do v�ech stran od dan� pozice podlahy, 
                                                              // tak�e kolem ka�d�ho podlahov�ho tilu prohled� v�echny potenci�ln� zdi, kter� zkontroluje v if n�e)
                if (floorPositions.Contains(neighbourPosition) == false) // t�mto zjist�me, zdali dan� neighbourPosition nen� pozic� podlahy, pokud nen�, je to ze�
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }

        return wallPositions;
    }
}
