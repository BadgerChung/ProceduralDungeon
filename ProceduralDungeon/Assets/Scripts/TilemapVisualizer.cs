using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallsTilemap;
    [SerializeField]
    private TileBase floorTile, wallTile;

    public void GenerateFloorTiles(IEnumerable<Vector2Int> floorPositions) // volá funkci GenerateTiles() s parametry pro generaci podlahy
    {
        GenerateTiles(floorPositions, floorTilemap, floorTile);
    }

    public void GenerateWallTiles(HashSet<Vector2Int> wallPositions) // volá funkci GenerateTiles() s parametry pro generaci zdí
    {
        GenerateTiles(wallPositions, wallsTilemap, wallTile);
    }

    private void GenerateTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile) // pomocí foreach volá funkci PaintSingleTile() pro každý prvek v dané kolekci positions
    {
        foreach (Vector2Int position in positions)
        {
            GenerateSingleTile(tilemap, tile, position);
        }
    }

    private void GenerateSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position) // pøevede dané pozice na cell pozice pro danou tilemap a uloží je do ní
    {
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear() // vyèistí všechny tilemap od vykreslených tile
    {
        floorTilemap.ClearAllTiles();
        wallsTilemap.ClearAllTiles();
    }
}
