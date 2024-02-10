using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] protected Tilemap floorTilemap;
    [SerializeField] protected Tilemap wallTilemap;
    [SerializeField] protected Tilemap itemTilemap;
    [SerializeField] protected GameObject itemTilePrefab;
    [SerializeField] protected List<Room> rooms;
    [SerializeField] protected DungeonTilesSO dungeonTiles;
    [SerializeField] protected HashSet<Vector2Int> path;
    [SerializeField] protected GameObject player; 
        
    private List<GameObject> itemTiles;

    public HashSet<Vector2Int> Path => path;
    public DungeonTilesSO DungeonTilesSO => dungeonTiles; 
    

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, dungeonTiles.floorTile);
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
            tile = dungeonTiles.wallTop;
        }
        else if (WallByteTypes.wallSideRight.Contains(typeAsInt))
        {
            tile = dungeonTiles.wallSideRight;
        }
        else if (WallByteTypes.wallSideLeft.Contains(typeAsInt))
        {
            tile = dungeonTiles.wallSideLeft;
        }
        else if (WallByteTypes.wallBottom.Contains(typeAsInt))
        {
            tile = dungeonTiles.wallBottom;
        }
        else if (WallByteTypes.wallFull.Contains(typeAsInt))
        {
            tile = dungeonTiles.wallFull;
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
            tile = dungeonTiles.wallInnerCornerDownRight;
        }
        else if (WallByteTypes.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = dungeonTiles.wallInnerCornerDownLeft;
        }
        else if (WallByteTypes.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = dungeonTiles.wallDiagonalCornerDownRight;
        }
        else if (WallByteTypes.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = dungeonTiles.wallDiagonalCornerDownLeft;
        }
        else if (WallByteTypes.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = dungeonTiles.wallDiagonalCornerUpRight;
        }
        else if (WallByteTypes.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = dungeonTiles.wallDiagonalCornerUpLeft;
        }
        else if (WallByteTypes.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = dungeonTiles.wallFull;
        }
        else if (WallByteTypes.wallBottomEightDirections.Contains(typeAsInt))
        {
            tile = dungeonTiles.wallBottom;
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

        for (int i = 0; i < itemTiles.Count; i++)
        {
            Destroy(itemTiles[i].gameObject);
            itemTiles.Remove(itemTiles[i]);
        }
    }

    public void Reset()
    {
        foreach (Room room in rooms)
        {
            foreach (GameObject item in room.PropObjectReferences)
            {
                Destroy(item);
            }

            foreach (GameObject item in room.EnemiesInTheRoom)
            {
                Destroy(item);
            }
        }

        rooms = new List<Room>();
        path = new HashSet<Vector2Int>();
    }

    public IEnumerator ActionCoroutine(Action code)
    {
        yield return new WaitForSeconds(1);
        code();
    }
}