using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] protected int minRoomWidth = 4;
    [SerializeField] protected int minRoomHeight = 4;

    [SerializeField] protected int dungeonWidth = 20;
    [SerializeField] protected int dungeonHeight = 20;

    private int offset = 1;
    
    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floor = GenerateRoom();
        
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    private HashSet<Vector2Int> GenerateRoom()
    {
        rooms = new List<Room>();
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        
        List<BoundsInt> roomBounds = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth,
            minRoomHeight);

        for (int i = 0; i < roomBounds.Count; i++)
        {
            Room newRoom = new Room
            {
                TilePositions = new List<Vector2Int>(),
                TileBounds = roomBounds[i],
                Center = roomBounds[i].center
            };

            Vector2Int roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds[i].center.x),
                Mathf.RoundToInt(roomBounds[i].center.y));
            
            HashSet<Vector2Int> roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (Vector2Int position in roomFloor)
            {
                if (position.x >= (roomBounds[i].xMin + offset) && position.x <= (roomBounds[i].xMax - offset) &&
                    position.y >= (roomBounds[i].yMin - offset) && position.y <= (roomBounds[i].yMax - offset))
                {
                    floor.Add(position);
                    newRoom.TilePositions.Add(position);
                }
            }
            rooms.Add(newRoom);
        }

        return floor;
    }
}