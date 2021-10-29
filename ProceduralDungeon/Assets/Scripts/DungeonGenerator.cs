using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    public TilemapVisualizer tilemapVisualizer;

    [SerializeField]
    private RoomTypesSO roomParameters;

    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    [SerializeField]
    protected int corridorLength = 14, corridorCount = 5;

    [SerializeField]
    [Range(0.1f, 1)]
    protected float roomPercent = 0.8f;

    [SerializeField]
    protected RoomTypesSO corridorParameters;

    public virtual void RunProceduralGeneration() // z funkce RunRandomWalk dostane pozice pro floor tiles a ulo�� je do floorPositions
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(roomParameters);

        tilemapVisualizer.Clear();
        tilemapVisualizer.GenerateFloorTiles(floorPositions); // vykresl� pozice podlahov�ch til� z floorPositions
        WallGenerator.GenerateWalls(floorPositions, tilemapVisualizer); // vykresl� pozice til� zd� z floorPositions (nepou��v� p��mo pozice zfloorPositions, ale upravuje je)
    }

    private HashSet<Vector2Int> RunRandomWalk(RoomTypesSO roomParameters) // vrac� pozice pro floor tiles
    {
        Vector2Int currentPosition = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < roomParameters.iterations; i++)
        {
            HashSet<Vector2Int> path = ProceduralGenerationAlgorithms.RandomWalk(currentPosition, roomParameters.walkLength); // postupn� generuje r�zn� "cesty" pozic kter� p�id�v� do path (aby z toho vznikla n�jak� v�t�� m�stnost)
            floorPositions.UnionWith(path); // zde slu�uje pozice z path do floorPositions aby floorPositions neobsahovaly duplik�ty

            if (roomParameters.startRandomlyEachIteration) currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count)); // vybere n�hodnou pozici ze kter� rozb�hne dal�� iteraci
        }

        return floorPositions;
    }

    public void RunCorridorGeneration()
    {
        CorridorGenerator.startPosition = startPosition;
        CorridorGenerator.corridorCount = corridorCount;
        CorridorGenerator.corridorLength = corridorLength;

        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        
        CorridorGenerator.GenerateCorridors(floorPositions);

        tilemapVisualizer.Clear();
        tilemapVisualizer.GenerateFloorTiles(floorPositions);
        WallGenerator.GenerateWalls(floorPositions, tilemapVisualizer);
    }
}
