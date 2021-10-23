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
    private bool startRandomlyEachIteration = true; // pokud true, p�i ka�d� iteraci za�ne Random Walk z n�hodn� pozice, kterou u� jednou vybral

    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    public void RunProceduralGeneration() // z funkce RunRandomWalk dostane pozice pro floor tiles a ulo�� je do floorPositions
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();

        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions); // vykresl� pozice z floorPositions
    }

    private HashSet<Vector2Int> RunRandomWalk() // vrac� pozice pro floor tiles
    {
        Vector2Int currentPosition = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < iterations; i++)
        {
            HashSet<Vector2Int> path = ProceduralGenerationAlgorithms.RandomWalk(currentPosition, walkLength); // postupn� generuje r�zn� "cesty" pozic kter� p�id�v� do path (aby z toho vznikla n�jak� v�t�� m�stnost)
            floorPositions.UnionWith(path); // zde slu�uje pozice z path do floorPositions aby floorPositions neobsahovaly duplik�ty

            if (startRandomlyEachIteration) currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count)); // vybere n�hodnou pozici ze kter� rozb�hne dal�� iteraci
        }

        return floorPositions;
    }
}
