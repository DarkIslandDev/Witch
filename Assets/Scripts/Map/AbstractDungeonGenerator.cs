using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    
    public void GenerateDungeon()
    {
        // WallByteTypes.RemoveAllWallsToList();
        // tilemapVisualizer.dungeonTiles.RemoveAllWallsToList();
        
        tilemapVisualizer.Clear();

        // WallByteTypes.AddAllWallsToList();
        // tilemapVisualizer.dungeonTiles.AddAllWallsToList();
        
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}