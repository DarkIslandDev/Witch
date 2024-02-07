using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] protected Tilemap floorTilemap;
    [SerializeField] protected Tilemap wallTilemap;
    
    [SerializeField] protected TileBase floorTile;
    [SerializeField] protected TileBase wallTop;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (Vector2Int position in positions)
        {
            PaintSignleTile(tilemap, tile, position);
        }
    }

    public void PaintSingleBasicWall(Vector2Int position)
    {
        PaintSignleTile(wallTilemap, wallTop, position);
    }

    private void PaintSignleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }
    
    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }
}