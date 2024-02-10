using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    private static readonly List<Vector2Int> neighbours4Directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1),   // UP
        new Vector2Int(1, 0),   // RIGHT
        new Vector2Int(0, -1),   // DOWN
        new Vector2Int(-1, 0),   // LEFT
    };
    
    private static readonly List<Vector2Int> neighbours8Directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1),   // UP
        new Vector2Int(1, 0),   // RIGHT
        new Vector2Int(0, -1),  // DOWN
        new Vector2Int(-1, 0),  // LEFT
        new Vector2Int(1, 1),   // DIAGONAL
        new Vector2Int(1, -1),  // DIAGONAL
        new Vector2Int(-1, 1),  // DIAGONAL
        new Vector2Int(-1, -1), // DIAGONAL
    };
        
    private readonly List<Vector2Int> graph;

    public Graph(IEnumerable<Vector2Int> vertices) => graph = new List<Vector2Int>(vertices);

    public List<Vector2Int> GetNeighbours4Directions(Vector2Int startPosition)
    {
        return GetNeighbours(startPosition, neighbours4Directions);
    }

    public List<Vector2Int> GetNeighbours8Directions(Vector2Int startPosition)
    {
        return GetNeighbours(startPosition, neighbours8Directions);
    }

    private List<Vector2Int> GetNeighbours(Vector2Int startPosition, List<Vector2Int> neighboursOffsetList)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        
        foreach (Vector2Int neighboursDirection in neighboursOffsetList)
        {
            Vector2Int potentialNeigbour = startPosition + neighboursDirection;
            if (graph.Contains(potentialNeigbour))
            {
                neighbours.Add(potentialNeigbour);
            }
        }

        return neighbours;
    }
}