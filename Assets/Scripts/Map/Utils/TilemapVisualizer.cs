using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] protected Tilemap floorTilemap;
    [SerializeField] protected Tilemap wallTilemap;
    [SerializeField] protected Tilemap itemTilemap;
    [SerializeField] protected GameObject itemTilePrefab;
    [SerializeField] protected DungeonTilesSO dungeonTilesSO;
        
    private List<GameObject> itemTiles;

    public Tilemap FloorTilemap => floorTilemap;
    public DungeonTilesSO DungeonTilesSoSo => dungeonTilesSO;
    

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, dungeonTilesSO.floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (Vector2Int position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    public void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        
        if (WallByteTypes.wallTop.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallTop;
        }
        else if (WallByteTypes.wallSideRight.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallSideRight;
        }
        else if (WallByteTypes.wallSideLeft.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallSideLeft;
        }
        else if (WallByteTypes.wallBottom.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallBottom;
        }
        else if (WallByteTypes.wallFull.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallFull;
        }

        // tile = WallByteTypes.FindRightWall(dungeonTiles, typeAsInt);
        
        if (tile != null) PaintSingleTile(wallTilemap, tile, position);
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }
    
    public void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallByteTypes.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallInnerCornerDownRight;
        }
        else if (WallByteTypes.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallInnerCornerDownLeft;
        }
        else if (WallByteTypes.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallDiagonalCornerDownRight;
        }
        else if (WallByteTypes.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallDiagonalCornerDownLeft;
        }
        else if (WallByteTypes.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallDiagonalCornerUpRight;
        }
        else if (WallByteTypes.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallDiagonalCornerUpLeft;
        }
        else if (WallByteTypes.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallFull;
        }
        else if (WallByteTypes.wallBottomEightDirections.Contains(typeAsInt))
        {
            tile = dungeonTilesSO.wallBottom;
        }
        
        // tile = WallByteTypes.FindRightWall(dungeonTiles, typeAsInt);
        
        if (tile != null) 
        {
            PaintSingleTile(wallTilemap, tile, position);
        }
    }

    public void PaintItemTiles(IEnumerable<Vector2Int> positions, Sprite item)
    {
        foreach (Vector2Int position in positions)
        {
            PaintSingleItemTile(position, item);
        }
    }

    // :(
    private void PaintSingleItemTile(Vector2Int position, Sprite item)
    {
        Vector3 newVector = new Vector3(position.x, position.y, 0);
        GameObject go = Instantiate(itemTilePrefab, newVector, Quaternion.identity);
        itemTiles.Add(go);
        SpriteRenderer sp = go.GetComponent<SpriteRenderer>();
        sp.sprite = item;
        go.transform.SetParent(itemTilemap.transform);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        // itemTilemap.ClearAllTiles();

        // for (int i = 0; i < itemTiles.Count; i++)
        // {
        //     Destroy(itemTiles[i].gameObject);
        //     itemTiles.Remove(itemTiles[i]);
        // }
    }
}