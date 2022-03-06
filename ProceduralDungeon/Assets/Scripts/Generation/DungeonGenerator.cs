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
                  minBranchLength, maxBranchLength, // poèet koridorù v jedné vìtvi
                  minBranchCount, maxBranchCount; // poèet vìtví

    [SerializeField]
    [Range(0.1f, 1)]
    protected float roomPercent = 0.8f;

    [SerializeField]
    public int startRoomRectHeight, startRoomRectWidth;

    public int seed = 0;

    public static DungeonGenerator instance { get; private set; }

    public List<HashSet<Vector2Int>> roomsPositions;

    public HashSet<Vector2Int> corridorsPositions;

    public HashSet<Vector2Int> wallPositions;

    private void Awake()
    {
        instance = this;
        seed = Random.Range(0, 600);
    }

    private void Start()
    {
        RunProceduralGeneration();
    }

    public void RunProceduralGeneration()
    {

        Random.InitState(seed);

        instance = this;

        int branchCount = Random.Range(minBranchCount, maxBranchCount + 1);
        int[] branches = new int[branchCount];

        for (int i = 0; i < branchCount; i++)
        {
            branches[i] = Random.Range(minBranchLength, maxBranchLength + 1); // každé vìtvi v branches se pøidìlí poèet koridorù
        }

        roomsPositions = new List<HashSet<Vector2Int>>();
        corridorsPositions = new HashSet<Vector2Int>();

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

        wallPositions = WallFinder.FindWallPositions(floorPositions, Direction2D.directionsList);

        tilemapVisualizer.Clear();
        tilemapVisualizer.GenerateFloorTiles(floorPositions); // vykreslí pozice podlahových tilù z floorPositions
        tilemapVisualizer.GenerateWallTiles(wallPositions); // vykreslí pozice tilù zdí z floorPositions (nepoužívá pøímo pozice zfloorPositions, ale upravuje je)

        ObjectGenerator.InitObjectGenerator(roomsPositions);
        HashSet<Vector2Int> chestPositions = ObjectGenerator.GenerateChestPositions();
        HashSet<Vector2Int> enemyPositions = ObjectGenerator.GenerateEnemyPositions();
        wallPositions.UnionWith(chestPositions);
        GameObject chestPrefab = Resources.Load<GameObject>("Prefabs/chest");
        GameObject[] enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");
        foreach(Vector2Int position in chestPositions)
        {
            GameObject chest = Instantiate(chestPrefab, (Vector2)position, Quaternion.identity);
            ChestInteractable cInteractable = chest.GetComponentInChildren<ChestInteractable>();
            cInteractable.inventory = new Inventory(10);
            foreach(ItemProbability itemProbability in InventoryVisualizer.instance.lootTable)
            {
                float rand = Random.Range(0f, 1f);
                if(rand < itemProbability.probability)
                {
                    cInteractable.inventory.TryAddItem(itemProbability.item);
                }
            }
        }
        foreach (Vector2Int position in enemyPositions)
        {
            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], (Vector2)position, Quaternion.identity);
        }
    }


    public HashSet<Vector2Int> RunRandomWalk(Vector2Int position) // vrací pozice pro floor tiles
    {
        Vector2Int currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < roomParameters.iterations; i++)
        {
            HashSet<Vector2Int> path = ProceduralGenerationAlgorithms.RandomWalk(currentPosition, roomParameters.walkLength); // postupnì generuje rùzné "cesty" pozic které pøidává do path (aby z toho vznikla nìjaká vìtší místnost)
            floorPositions.UnionWith(path); // zde sluèuje pozice z path do floorPositions aby floorPositions neobsahovaly duplikáty

            if (roomParameters.startRandomlyEachIteration) currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count)); // vybere náhodnou pozici ze které rozbìhne další iteraci
        }

        return floorPositions;
    }
}
