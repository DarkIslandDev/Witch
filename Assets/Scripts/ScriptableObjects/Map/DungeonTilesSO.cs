using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Map/Dungeon Tiles", fileName = "Dungeon Tiles", order = 0)]
public class DungeonTilesSO : ScriptableObject
{
    [Header("Floor tiles")]
    public TileBase floorTile;
    
    [Header("Door tiles")]
    public TileBase leftDoorTile;
    public TileBase rightDoorTile;
    
    [Header("Wall tiles")]
    public TileBase wallTop;
    public TileBase wallSideRight;
    public TileBase wallSideLeft;
    public TileBase wallBottom;
    public TileBase wallFull;
    public TileBase wallInnerCornerDownRight;
    public TileBase wallInnerCornerDownLeft;
    public TileBase wallDiagonalCornerDownRight;
    public TileBase wallDiagonalCornerDownLeft;
    public TileBase wallDiagonalCornerUpRight;
    public TileBase wallDiagonalCornerUpLeft;
    // public TileBase wallFullEightDirections;
    // public TileBase wallBottomEightDirections;

    // public List<TileBase> walls;
    //
    // public void AddAllWallsToList()
    // {
    //     walls.Add(wallTop);
    //     walls.Add(wallSideRight);
    //     walls.Add(wallSideLeft);
    //     walls.Add(wallBottom);
    //     walls.Add(wallFull);
    //     walls.Add(wallInnerCornerDownRight);
    //     walls.Add(wallInnerCornerDownLeft);
    //     walls.Add(wallDiagonalCornerDownRight);
    //     walls.Add(wallDiagonalCornerDownLeft);
    //     walls.Add(wallDiagonalCornerUpRight);
    //     walls.Add(wallDiagonalCornerUpLeft);
    //     walls.Add(wallFullEightDirections);
    //     walls.Add(wallBottomEightDirections);
    // }
    //
    // public void RemoveAllWallsToList()
    // {
    //     walls.Remove(wallTop);
    //     walls.Remove(wallSideRight);
    //     walls.Remove(wallSideLeft);
    //     walls.Remove(wallBottom);
    //     walls.Remove(wallFull);
    //     walls.Remove(wallInnerCornerDownRight);
    //     walls.Remove(wallInnerCornerDownLeft);
    //     walls.Remove(wallDiagonalCornerDownRight);
    //     walls.Remove(wallDiagonalCornerDownLeft);
    //     walls.Remove(wallDiagonalCornerUpRight);
    //     walls.Remove(wallDiagonalCornerUpLeft);
    //     walls.Add(wallFullEightDirections);
    //     walls.Add(wallBottomEightDirections);
    // }
}