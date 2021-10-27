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

    public void GenerateFloorTiles(IEnumerable<Vector2Int> floorPositions) // obsahuje pozice podlahov�ch til� a n�sledn� vol� funkce pro jejich vykreslen�
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

    internal void GenerateSingleWall(Vector2Int position)
    {
        GenerateSingleTile(wallsTilemap, wallTile, position);
    }

    public void Clear() // vy�ist� obrazovku od vykreslen�ch til�
    {
        floorTilemap.ClearAllTiles();
        wallsTilemap.ClearAllTiles();
    }
}
