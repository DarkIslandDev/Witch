using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static IEnumerable<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int> { startPosition };

        Vector2Int previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            Vector2Int newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        Vector2Int direction = Direction2D.GetRandomCardinalDirection();
        Vector2Int currentPosition = startPosition;
        corridor.Add(currentPosition);
        
        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }
}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionList = new List<Vector2Int>
    {
        new Vector2Int(0, 1),   //  UP
        new Vector2Int(1, 0),   //  RIGHT
        new Vector2Int(0, -1),  //  DOWN
        new Vector2Int(-1, 0)   //  LEFT
    };

    public static Vector2Int GetRandomCardinalDirection() => cardinalDirectionList[Random.Range(0, cardinalDirectionList.Count)];
}