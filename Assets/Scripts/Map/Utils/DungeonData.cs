using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData : MonoBehaviour
{
    [SerializeField] protected GameObject playerReference;
    [SerializeField] protected List<Room> rooms;
    [SerializeField] protected HashSet<Vector2Int> path;
    
    public List<Room> Rooms => rooms;
    public HashSet<Vector2Int> Path => path;
    public GameObject PlayerReference => playerReference;
    
    public void Reset()
    {
        foreach (Room room in Rooms)
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
        Destroy(PlayerReference);
    }

    public IEnumerator ActionCoroutine(Action code)
    {
        yield return new WaitForSeconds(1);
        code();
    }
}

[Serializable]
public class Room
{
    public Vector2 RoomCenterPos { get; set; }
    public HashSet<Vector2Int> FloorTiles { get; private set; }

    public HashSet<Vector2Int> NearWallTilesUp { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesDown { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesLeft { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesRight { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> CornerTiles { get; set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> InnerTiles { get; set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> PropPositions { get; set; } = new HashSet<Vector2Int>();
    public List<GameObject> PropObjectReferences { get; set; } = new List<GameObject>();

    public List<Vector2Int> PositionsAccessibleFromPath { get; set; } = new List<Vector2Int>();

    public List<GameObject> EnemiesInTheRoom { get; set; } = new List<GameObject>();

    public Room(Vector2 roomCenterPos, HashSet<Vector2Int> floorTiles)
    {
        this.RoomCenterPos = roomCenterPos;
        this.FloorTiles = floorTiles;
    }
}