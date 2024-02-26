using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Cell
{
    public bool visited = false;
    public bool[] status = new bool[4];
}

[Serializable]
public class Rule
{
    public Vector2Int minPosition;
    public Vector2Int maxPosition;
    public bool obligatory;

    public int ProbabilityOfSpawning(int x, int y)
    {
        if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
        {
            return obligatory ? 2 : 1;
        }

        return 0;
    }
}
public class DungeonMap : MonoBehaviour
{
    public Vector2Int size;
    public int startPosition = 0;
    public List<Rule> rooms;
    public Vector2 offset;
    public TilemapVisualizer tilemapVisualizer;

    private List<Cell> board;

    private void Start()
    {
        
    }

    private void GenerateDungeon()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Cell currentCell = board[(x + y * size.x)];
                if (currentCell.visited)
                {
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    for (int k = 0; k < rooms.Count; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(x, y);

                        if (p == 2)
                        {
                            randomRoom = k;
                            break;
                        }
                        else if (p == 1)
                        {
                            availableRooms.Add(k);
                        }
                    }

                    if (randomRoom == -1)
                    {
                        if (availableRooms.Count > 0)
                        {
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }
                    
                    //
                }
            }
        }
    }
}