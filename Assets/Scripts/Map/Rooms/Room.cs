using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Room
{
    [SerializeField] protected bool safeRoom;
    [SerializeField] protected bool bossRoom;
    [SerializeField] protected bool traderRoom;
    [SerializeField] protected List<Vector2Int> tilePositions;
    [SerializeField] protected List<BoundsInt> tileBounds;
    [SerializeField] protected List<ItemData> itemData;

    public bool SafeRoom { get => safeRoom; set => safeRoom = value; }

    public bool BossRoom { get => bossRoom; set => bossRoom = value; }
    public bool TraderRoom { get => traderRoom; set => traderRoom = value; }
    public List<Vector2Int> TilePositions { get => tilePositions; set => tilePositions = value; }
    public List<ItemData> ItemData { get => itemData; set => itemData = value; }
}