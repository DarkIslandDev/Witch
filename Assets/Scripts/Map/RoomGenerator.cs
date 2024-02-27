using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    public Tilemap floorTilemap; // Reference to the floor Tilemap component
    public Tilemap wallTilemap; // Reference to the wall Tilemap component
    public TileBase floorTile; // The floor tile you want to use
    public TileBase wallTile; // The wall tile you want to use

    public GameObject doorPrefab; // Reference to the door prefab
    public int roomWidthMin = 5; // Minimum room width
    public int roomWidthMax = 10; // Maximum room width
    public int roomHeightMin = 5; // Minimum room height
    public int roomHeightMax = 10; // Maximum room height

    private List<GameObject> doors = new List<GameObject>(); // Array to store doors

    private void Start()
    {
        GenerateRoom();
    }

    public void GenerateRoom()
    {
        // Randomly determine the room width and height
        int roomWidth = Random.Range(roomWidthMin, roomWidthMax + 1);
        int roomHeight = Random.Range(roomHeightMin, roomHeightMax + 1);

        // Create the floor tiles
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
            }
        }

        // Create the wall tiles (outer border)
        for (int x = -1; x <= roomWidth; x++)
        {
            wallTilemap.SetTile(new Vector3Int(x, -1, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(x, roomHeight, 0), wallTile);
        }
        for (int y = 0; y < roomHeight; y++)
        {
            wallTilemap.SetTile(new Vector3Int(-1, y, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(roomWidth, y, 0), wallTile);
        }

        // Add doors (one on each side)
        wallTilemap.SetTile(new Vector3Int(roomWidth / 2, -1, 0), null); // Top door
        wallTilemap.SetTile(new Vector3Int(roomWidth / 2, roomHeight, 0), null); // Bottom door
        wallTilemap.SetTile(new Vector3Int(-1, roomHeight / 2, 0), null); // Left door
        wallTilemap.SetTile(new Vector3Int(roomWidth, roomHeight / 2, 0), null); // Right door

        // Instantiate the door prefab
        Vector3 doorPosition = new Vector3(roomWidth / 2, roomHeight / 2, 0);
        GameObject newDoor = Instantiate(doorPrefab, doorPosition, Quaternion.identity);
        doors.Add(newDoor);

        // Add noise to create non-rectangular shape
        for (int i = 0; i < 10; i++)
        {
            int x = Random.Range(1, roomWidth - 1);
            int y = Random.Range(1, roomHeight - 1);
            wallTilemap.SetTile(new Vector3Int(x, y, 0), null);
        }
    }
    
    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }
}