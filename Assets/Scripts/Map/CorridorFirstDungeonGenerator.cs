﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] protected int corridorLength = 14;
    [SerializeField] protected int corridorCount = 5;
    [SerializeField] [Range(0.1f, 1f)] protected float roomPercent = 0.8f;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstDungeonGeneration();
    }

    private void CorridorFirstDungeonGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            // corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
            corridors[i] = IncreaseCorridorBrush3By3(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private void CreateRoomAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (Vector2Int position in deadEnds)
        {
            if (roomFloors.Contains(position) == false)
            {
                HashSet<Vector2Int> room = RunRandomWalk(randomWalkParameters, position);
                room.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (Vector2Int position in floorPositions)
        {
            int neighboursCount = 0;

            foreach (Vector2Int direction in Direction2D.cardinalDirectionList)
            {
                if (floorPositions.Contains(position + direction)) neighboursCount++;
            }

            if (neighboursCount == 1) deadEnds.Add(position);
        }

        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate =
            potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        for (int i = 0; i < roomsToCreate.Count; i++)
        {
            Vector2Int roomPosition = roomsToCreate[i];
            HashSet<Vector2Int> roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions,
        HashSet<Vector2Int> potentialRoomPositions)
    {
        Vector2Int currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        for (int i = 0; i < corridorCount; i++)
        {
            List<Vector2Int> corridor =
                ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            corridors.Add(corridor);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }

        return corridors;
    }

    public List<Vector2Int> IncreaseCorridorBrush3By3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();

        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                }
            }
        }

        return newCorridor;
    }
    
    private List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int previousDirection = Vector2Int.zero;

        for (int i = 1; i < corridor.Count; i++)
        {
            Vector2Int directionFromCell = corridor[i] - corridor[i - 1];

            if (previousDirection != Vector2Int.zero && directionFromCell != previousDirection)
            {
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                    }
                }

                previousDirection = directionFromCell;
            }
            else
            {
                Vector2Int newCorridorTileOffset = GetDirection90From(directionFromCell);
                newCorridor.Add(corridor[i - 1]);
                newCorridor.Add(corridor[i - 1] + newCorridorTileOffset);
            }
        }

        return newCorridor;
    }

    private Vector2Int GetDirection90From(Vector2Int direction)
    {
        if(direction == Vector2Int.up) return Vector2Int.right;
        if(direction == Vector2Int.right) return Vector2Int.down;
        if(direction == Vector2Int.down) return Vector2Int.left;
        if(direction == Vector2Int.left) return Vector2Int.up;
        
        return Vector2Int.zero;
    }
}