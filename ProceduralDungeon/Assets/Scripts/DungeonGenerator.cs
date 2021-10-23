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
    private int iterations = 10;
    [SerializeField]
    private int walkLength = 10;
    [SerializeField]
    private bool startRandomlyEachIteration = true; // pokud true, pøi každé iteraci zaène Random Walk z náhodné pozice, kterou už jednou vybral

    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    public void RunProceduralGeneration() // z funkce RunRandomWalk dostane pozice pro floor tiles a uloží je do floorPositions
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();

        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions); // vykreslí pozice z floorPositions
    }

    private HashSet<Vector2Int> RunRandomWalk() // vrací pozice pro floor tiles
    {
        Vector2Int currentPosition = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < iterations; i++)
        {
            HashSet<Vector2Int> path = ProceduralGenerationAlgorithms.RandomWalk(currentPosition, walkLength); // postupnì generuje rùzné "cesty" pozic které pøidává do path (aby z toho vznikla nìjaká vìtší místnost)
            floorPositions.UnionWith(path); // zde sluèuje pozice z path do floorPositions aby floorPositions neobsahovaly duplikáty

            if (startRandomlyEachIteration) currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count)); // vybere náhodnou pozici ze které rozbìhne další iteraci
        }

        return floorPositions;
    }
}
