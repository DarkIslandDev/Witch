using System.Collections.Generic;
using UnityEngine;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] protected int minRoomWidth = 4;
    [SerializeField] protected int minRoomHeight = 4;
    
    [SerializeField] protected int dungeonWidth = 20;
    [SerializeField] protected int dungeonHeight = 20;
    
    [SerializeField] [Range(0, 10)] protected int offset = 1;
    [SerializeField] protected bool randomWalkRooms = false;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        List<BoundsInt> roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(minRoomWidth, minRoomHeight, 0)), minRoomWidth,
            minRoomHeight);

        HashSet<Vector2Int> floor = randomWalkRooms ? CreateSimpleRooms(roomsList) : CreateSimpleRooms(roomsList);

        List<Vector2Int> ropomCenters = new List<Vector2Int>();

        foreach (BoundsInt room in roomsList)
        {
            ropomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }
        
        // HashSet<Vector2Int> corridors = 
        
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        
        foreach (BoundsInt room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }

        return floor;
    }
}