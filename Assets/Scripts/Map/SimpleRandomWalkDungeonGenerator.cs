using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [Header("Parameters")] 
    [SerializeField] protected SimpleRandomWalkSO randomWalkParameters;
    
    private void Start()
    {
        GenerateDungeon();
    }

    protected override void RunProceduralGeneration()
    {
        tilemapVisualizer.Clear();
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position)
    {
        Vector2Int currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        
        for (int i = 0; i < randomWalkParameters.iterations; i++)
        {
            IEnumerable<Vector2Int> path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, randomWalkParameters.walkLenght);
            floorPositions.UnionWith(path);

            if (randomWalkParameters.startRandomlyEachIteration) currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }

        return floorPositions;
    }
}