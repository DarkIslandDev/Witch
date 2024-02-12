﻿using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField] protected DungeonData dungeonData;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected GameObject playerObject;

    public TilemapVisualizer TilemapVisualizer => tilemapVisualizer;

    public void GenerateDungeon()
    {
        tilemapVisualizer.Clear();
        
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}