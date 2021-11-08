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

    public void GenerateFloorTiles(IEnumerable<Vector2Int> floorPositions) // vol� funkci GenerateTiles() s parametry pro generaci podlahy
    {
        GenerateTiles(floorPositions, floorTilemap, floorTile);
    }

    private void GenerateTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile) // pomoc� foreach a funkce PaintSingleTile zajist� vykreslen� til� na dan� pozice
    {
        foreach (Vector2Int position in positions)
        {
            GenerateSingleTile(tilemap, tile, position);
        }
    }

    private void GenerateSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position) // p�evede pozici tilu na speci�ln� pozice pro tily a vykresl� ho
    {
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void GenerateWallTiles(HashSet<Vector2Int> wallPositions)
    {
        GenerateTiles(wallPositions, wallsTilemap, wallTile);
    }

    public void Clear() // vy�ist� obrazovku od vykreslen�ch til�
    {
        floorTilemap.ClearAllTiles();
        wallsTilemap.ClearAllTiles();
    }
}
