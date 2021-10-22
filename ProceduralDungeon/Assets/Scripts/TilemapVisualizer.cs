using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap;
    [SerializeField]
    private TileBase floorTile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions) // obsahuje pozice podlahov�ch til� a n�sledn� vol� funkce pro jejich vykreslen�
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile) // pomoc� foreach a funkce PaintSingleTile zajist� vykreslen� til� na dan� pozice
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position) // p�evede pozici tilu na speci�ln� pozice pro tily a vykresl� ho
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear() // vy�ist� obrazovku od vykreslen�ch til�
    {
        floorTilemap.ClearAllTiles();
    }
}
