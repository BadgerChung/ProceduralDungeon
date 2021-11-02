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
    protected int corridorLength,
                  minBranchLength, maxBranchLength, // po�et koridor� v jedn� v�tvi
                  minBranchCount, maxBranchCount; // po�et v�tv�

    [SerializeField]
    [Range(0.1f, 1)]
    protected float roomPercent = 0.8f;

    public static DungeonGenerator instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public void RunProceduralGeneration()
    {
        instance = this;

        int branchCount = Random.Range(minBranchCount, maxBranchCount + 1);
        int[] branches = new int[branchCount];

        for (int i = 0; i < branchCount; i++)
        {
            branches[i] = Random.Range(minBranchLength, maxBranchLength + 1); // ka�d� v�tvi v branches se p�id�l� n�hodn� d�lka (po�et koridor�)
        }

        CorridorGenerator.startPosition = startPosition;
        CorridorGenerator.corridorLength = corridorLength;
        CorridorGenerator.roomPercent = roomPercent;
        CorridorGenerator.branches = branches;

        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        CorridorGenerator.GenerateCorridors(floorPositions, potentialRoomPositions);
        HashSet<Vector2Int> roomPositions = CorridorGenerator.GenerateRooms(potentialRoomPositions);

        HashSet<Vector2Int> deadEnds = CorridorGenerator.FindDeadEnds(floorPositions);
        CorridorGenerator.FixDeadEnds(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        tilemapVisualizer.Clear();
        tilemapVisualizer.GenerateFloorTiles(floorPositions); // vykresl� pozice podlahov�ch til� z floorPositions
        WallGenerator.GenerateWalls(floorPositions, tilemapVisualizer); // vykresl� pozice til� zd� z floorPositions (nepou��v� p��mo pozice zfloorPositions, ale upravuje je)
    }

    public HashSet<Vector2Int> RunRandomWalk(Vector2Int position) // vrac� pozice pro floor tiles
    {
        Vector2Int currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < roomParameters.iterations; i++)
        {
            HashSet<Vector2Int> path = ProceduralGenerationAlgorithms.RandomWalk(currentPosition, roomParameters.walkLength); // postupn� generuje r�zn� "cesty" pozic kter� p�id�v� do path (aby z toho vznikla n�jak� v�t�� m�stnost)
            floorPositions.UnionWith(path); // zde slu�uje pozice z path do floorPositions aby floorPositions neobsahovaly duplik�ty

            if (roomParameters.startRandomlyEachIteration) currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count)); // vybere n�hodnou pozici ze kter� rozb�hne dal�� iteraci
        }

        return floorPositions;
    }
}
