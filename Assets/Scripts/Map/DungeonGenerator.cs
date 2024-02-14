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

    protected List<BoundsInt> roomsList;
    protected HashSet<Vector2Int> floor;
    protected HashSet<Vector2Int> corridors;
    protected List<Vector2Int> roomCenters;
    protected List<Vector2Int> spawnPositions;

    public HashSet<Vector2Int> Floor => floor;
    public HashSet<Vector2Int> Corridors => corridors;
    public List<BoundsInt> RoomsList => roomsList;
    public List<Vector2Int> SpawnPositions => spawnPositions;
    
    private Vector2Int currentPosition;

    protected override void RunProceduralGeneration() => CreateRooms();

    private void CreateRooms()
    {
        roomsList = new List<BoundsInt>();
        corridors = new HashSet<Vector2Int>();
        spawnPositions = new List<Vector2Int>();
        roomCenters = new List<Vector2Int>();

        roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth,
            minRoomHeight);
        
        floor = randomWalkRooms ? CreateRoomsRandomly() : CreateSimpleRooms();

        foreach (BoundsInt room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        corridors = ConnectRooms();
        corridors = IncreaseCorridorBrush3By3(corridors);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
        
        rooms[Random.Range(0, rooms.Count)].SafeRoom = true;
        
        SpawnPlayer();

    }

    public void SpawnPlayer()
    {
        Room newRoom = rooms.Find(x => x.SafeRoom);
        Vector2Int newRoomPosition = new Vector2Int();
        
        if (newRoom.SafeRoom)
        {
            newRoomPosition = newRoom.TilePositions[Random.Range(0, newRoom.TilePositions.Count)];
        }
        
        float spawnX = newRoomPosition.x;
        float spawnY = newRoomPosition.y;
        playerObject.transform.position = new Vector3(spawnX, spawnY, 0);
    }
        
        
    
    private HashSet<Vector2Int> ConnectRooms()
    {
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

    public HashSet<Vector2Int> IncreaseCorridorBrush3By3(HashSet<Vector2Int> corridor)
    {
        HashSet<Vector2Int> newCorridor = new HashSet<Vector2Int>();

        for (int i = 1; i < corridors.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    foreach (Vector2Int vector2Int in corridors)
                    {
                        newCorridor.Add(vector2Int + new Vector2Int(x,y));
                    }
                }
            }
        }

        return newCorridor;
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
    
    private HashSet<Vector2Int> CreateRoomsRandomly()
    {
        floor = new HashSet<Vector2Int>();
        rooms = new List<Room>();

        Vector2Int roomCenter = new Vector2Int();

        foreach (BoundsInt roomBounds in roomsList)
        {
            roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            spawnPositions.Add(roomCenter);

            Room newRoom = new Room
            {
                TilePositions = new List<Vector2Int>()
            };

            HashSet<Vector2Int> roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (Vector2Int position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + 2 + offset) && position.x <= (roomBounds.xMax - 2 - offset) &&
                    position.y >= (roomBounds.yMin + 2 - offset) && position.y <= (roomBounds.yMax - 2 - offset))
                {
                    floor.Add(position);

                    newRoom.TilePositions.Add(position);

                }
            }
            rooms.Add(newRoom);
            
        }

        currentPosition = roomCenter;

        return floor;
    }
}