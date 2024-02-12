using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] protected int minRoomWidth = 4;
    [SerializeField] protected int minRoomHeight = 4;

    [SerializeField] protected int dungeonWidth = 20;
    [SerializeField] protected int dungeonHeight = 20;

    [SerializeField] [Range(0, 10)] protected int offset = 1;
    [SerializeField] protected bool randomWalkRooms = false;

    [SerializeField] protected HashSet<Vector2Int> floor;
    [SerializeField] protected IEnumerable<Vector2Int> corridors;
    [SerializeField] protected List<BoundsInt> roomsList;
    [SerializeField] protected List<Vector2Int> roomCenters;
    
    public HashSet<Vector2Int> Floor => floor;
    public IEnumerable<Vector2Int> Corridors => corridors;
    public List<BoundsInt> RoomsList => roomsList;
    public List<Vector2Int> RoomCenters => roomCenters;

    
    
    // [SerializeField] protected PlayerRoom playerRoom;
    // [SerializeField] protected EnemyRoom enemyRoom;

    private Vector2Int currentPosition;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        roomsList = new List<BoundsInt>();

        roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth,
            minRoomHeight);

        floor = randomWalkRooms ? CreateRoomsRandomly() : CreateSimpleRooms();

        roomCenters = new List<Vector2Int>();

        foreach (BoundsInt room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        corridors = ConnectRooms();
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        SpawnPlayer();
    }

    public void SpawnPlayer() => playerObject.transform.position = (Vector3Int)currentPosition;

    private HashSet<Vector2Int> CreateRoomsRandomly()
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        Vector2Int roomCenter = new Vector2Int();
        
        foreach (BoundsInt roomBounds in roomsList)
        {
            roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));

            HashSet<Vector2Int> roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (Vector2Int position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) &&
                    position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }

        currentPosition = roomCenter;

        return floor;
    }

    private IEnumerable<Vector2Int> ConnectRooms()
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        Vector2Int currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter);
            roomCenters.Remove(closest);
            IEnumerable<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    private IEnumerable<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        Vector2Int position = currentRoomCenter;
        corridor.Add(position);

        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }

            corridor.Add(position);
        }

        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }

            corridor.Add(position);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (Vector2Int position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms()
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (BoundsInt room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }

        return floor;
    }
}