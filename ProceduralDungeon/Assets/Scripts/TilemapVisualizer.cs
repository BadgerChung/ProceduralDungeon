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

    private void GenerateTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile) // pomocí foreach a funkce PaintSingleTile zajistí vykreslení tilù na dané pozice
    {
        foreach (Vector2Int position in positions)
        {
            GenerateSingleTile(tilemap, tile, position);
        }
    }

    private void GenerateSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position) // pøevede pozici tilu na speciální pozice pro tily a vykreslí ho
    {
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void GenerateWallTiles(HashSet<Vector2Int> wallPositions)
    {
        GenerateTiles(wallPositions, wallsTilemap, wallTile);
    }

    public void Clear() // vyèistí obrazovku od vykreslených tilù
    {
        floorTilemap.ClearAllTiles();
        wallsTilemap.ClearAllTiles();
    }
}
