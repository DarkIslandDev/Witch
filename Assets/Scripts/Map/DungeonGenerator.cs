﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] protected int minRoomWidth = 4;
    [SerializeField] protected int minRoomHeight = 4;

    [SerializeField] protected int dungeonWidth = 20;
    [SerializeField] protected int dungeonHeight = 20;

    private int offset = 1;

    private HashSet<Vector2Int> floor;
    private HashSet<Vector2Int> corridors;
    private List<Vector2Int> roomCenters;

    public List<BoundsInt> RoomsList { get; private set; }
    public List<Vector2Int> SpawnPositions { get; private set; }


    protected override void RunProceduralGeneration() => CreateRooms();

    private void CreateRooms()
    {
        RoomsList = new List<BoundsInt>();
        corridors = new HashSet<Vector2Int>();
        SpawnPositions = new List<Vector2Int>();
        roomCenters = new List<Vector2Int>();
        rooms = new List<Room>();

        RoomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth,
            minRoomHeight);
        
        floor = CreateRoomsRandomly();

        
        foreach (BoundsInt room in RoomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        corridors = ConnectRooms();
        corridors = IncreaseCorridorBrush2By2(corridors);

        floor.UnionWith(corridors);

        if (rooms.Count < 17) RunProceduralGeneration();
        
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
        
        rooms[Random.Range(0, rooms.Count)].SafeRoom = true;
        
        SpawnPlayer();

    }

    private void SpawnPlayer()
    {
        // Room newRoom = rooms.Find(x => x.SafeRoom);
        // Vector2Int newRoomPosition = new Vector2Int();
        //
        // if (newRoom.SafeRoom)
        // {
        //     newRoomPosition = newRoom.TilePositions[Random.Range(0, newRoom.TilePositions.Count)];
        // }
        //
        // float spawnX = newRoomPosition.x;
        // float spawnY = newRoomPosition.y;
        // playerObject.transform.position = new Vector3(spawnX, spawnY, 0);

        foreach (Room room in rooms)
        {
            playerObject.transform.position = room.Center;
        }
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

    private HashSet<Vector2Int> IncreaseCorridorBrush2By2(HashSet<Vector2Int> corridor)
    {
        HashSet<Vector2Int> newCorridor = new HashSet<Vector2Int>();

        for (int i = 0; i < corridors.Count; i++)
        {
            for (int x = -1; x < 1; x++)
            {
                for (int y = -1; y < 1; y++)
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

    private IEnumerable<Vector2Int> CreateCorridor(Vector2Int position, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int> { position };
        
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
            if (!(currentDistance < distance)) continue;
            distance = currentDistance;
            closest = position;
        }

        return closest;
    }
    
    private HashSet<Vector2Int> CreateRoomsRandomly()
    {
        floor = new HashSet<Vector2Int>();
        rooms = new List<Room>();

        foreach (BoundsInt roomBounds in RoomsList)
        {
            Vector2Int roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            SpawnPositions.Add(roomCenter);

            Room newRoom = new Room
            {
                TilePositions = new List<Vector2Int>(),
                TileBounds = roomBounds,
                Center = roomBounds.center
            };
            
            HashSet<Vector2Int> roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (Vector2Int position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) &&
                    position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                    newRoom.TilePositions.Add(position);
                }
            }
            rooms.Add(newRoom);
        }
        
        return floor;
    }
    
    private void CreateRoomEntrances()
    {
        foreach (Room room in rooms)
        {
            Vector2Int center = new Vector2Int((int)room.Center.x, (int)room.Center.y);
            Vector2Int closestPoint = FindClosestPointTo(center);
            IEnumerable<Vector2Int> corridor = CreateCorridor(center, closestPoint);
    
            if (room.Doors == null)
            {
                room.Doors = new List<Vector2Int>();
            }
    
            // Проверяем количество проходов в комнате
            while (room.Doors.Count < 2)
            {
                List<Vector2Int> newDoor = new List<Vector2Int>();
    
                foreach (Vector2Int position in corridor)
                {
                    if (floor.Contains(position))
                    {
                        // Позиция уже занята другим объектом или уже является входом в комнату, пропускаем ее
                        continue;
                    }
    
                    // Добавляем позицию входа в проход
                    newDoor.Add(position);
                    // Добавляем позицию входа в список позиций пола
                    floor.Add(position);
    
                    if (newDoor.Count >= 2)
                    {
                        // Если проход содержит две позиции, добавляем его в список проходов комнаты и выходим из цикла
                        foreach (Vector2Int newDoors in newDoor)
                        {
                            room.Doors.Add(newDoors);
                        }
                        break;
                    }
                }
            }
        }
    }
    
    // Function to find passages and add their positions as doors to a room
    public void FindAndAddPassages(Room room, List<Vector2Int> corridorPositions)
    {
        if (room.Doors == null)
        {
            room.Doors = new List<Vector2Int>();
        }

        // Iterate through the corridor positions
        foreach (Vector2Int corridorPos in corridorPositions)
        {
            // Check if the corridor position is adjacent to the room
            if (IsAdjacentToRoom(room, corridorPos))
            {
                // Add the corridor position as a door
                room.Doors.Add(corridorPos);
            }
        }
    }

    // Function to check if a position is adjacent to the room
    private bool IsAdjacentToRoom(Room room, Vector2Int position)
    {
        // Convert the position to a Vector3Int
        Vector3Int position3D = new Vector3Int(position.x, position.y, 0);

        // Check if the position is within one unit of the room's bounds
        return room.TileBounds.Contains(position3D + Vector3Int.left) ||
               room.TileBounds.Contains(position3D + Vector3Int.right) ||
               room.TileBounds.Contains(position3D + Vector3Int.up) ||
               room.TileBounds.Contains(position3D + Vector3Int.down);
    }
}