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

    public void GenerateWallTiles(HashSet<Vector2Int> wallPositions) // vol� funkci GenerateTiles() s parametry pro generaci zd�
    {
        GenerateTiles(wallPositions, wallsTilemap, wallTile);
    }

    private void GenerateTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile) // pomoc� foreach vol� funkci PaintSingleTile() pro ka�d� prvek v dan� kolekci positions
    {
        foreach (Vector2Int position in positions)
        {
            GenerateSingleTile(tilemap, tile, position);
        }
    }

    private void GenerateSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position) // p�evede dan� pozice na cell pozice pro danou tilemap a ulo�� je do n�
    {
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear() // vy�ist� v�echny tilemap od vykreslen�ch tile
    {
        floorTilemap.ClearAllTiles();
        wallsTilemap.ClearAllTiles();
    }
}
