using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Room
{
    [SerializeField] protected bool safeRoom;
    [SerializeField] protected bool bossRoom;
    [SerializeField] protected bool traderRoom;
    [SerializeField] protected bool isVisited = false;
    [SerializeField] protected List<Vector2Int> doors;
    [SerializeField] protected List<Vector2Int> tilePositions;
    [SerializeField] protected BoundsInt tileBounds;
    [SerializeField] protected Vector3 center;
    [SerializeField] protected List<ItemData> itemData;
    public bool SafeRoom { get => safeRoom; set => safeRoom = value; }

    public bool BossRoom { get => bossRoom; set => bossRoom = value; }
    public bool TraderRoom { get => traderRoom; set => traderRoom = value; }
    public bool IsVisited { get => isVisited; set => isVisited = value; }
    public List<Vector2Int> Doors { get => doors; set => doors = value; }
    public List<Vector2Int> TilePositions { get => tilePositions; set => tilePositions = value; }
    public BoundsInt TileBounds { get => tileBounds; set => tileBounds = value; }
    public Vector3 Center { get => center; set => center = value; }
    public List<ItemData> ItemData { get => itemData; set => itemData = value; }

    public UnityEvent RoomVisited;
    
    public void OpenRoom()
    {
        if (safeRoom) isVisited = true;
        
        if (!isVisited)
        {
            isVisited = true;
            // RoomVisited.AddListener(OpenRoom);
            Debug.Log("visited");
        }
    }
}