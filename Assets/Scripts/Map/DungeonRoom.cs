using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    [SerializeField] protected List<Vector2Int> walls;
    [SerializeField] protected List<Vector2Int> doors;

    private TilemapVisualizer tilemapVisualizer;
    private HashSet<Vector2Int> floor;
    
    public List<Vector2Int> Walls => walls;
    public List<Vector2Int> Doors => doors;

    public void Init(TilemapVisualizer tilemapVisualizer)
    {
        this.tilemapVisualizer = tilemapVisualizer;
    }

    // public void UpdateRoom(bool[] status)
    // {
    //     for (int i = 0; i < status.Length; i++)
    //     {
    //         switch (status[i])
    //         {
    //             // set active doors or walls
    //             case true:
    //                 tilemapVisualizer.PaintFloorTiles(doors);
    //                 break;
    //             case false:
    //                 tilemapVisualizer.PaintFloorTiles(walls);
    //                 break;
    //         }
    //     }
    // }
    
    
}