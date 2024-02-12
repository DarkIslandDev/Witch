using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentPlacer : MonoBehaviour
{
    [SerializeField] protected int playerRoomIndex;
    [SerializeField] protected List<int> roomEnemiesCount;
    [SerializeField] protected DungeonGenerator dungeonGenerator;

    private void Start()
    {
        PlaceAgent();
    }

    public void PlaceAgent()
    {
        for (int i = 0; i < dungeonGenerator.RoomsList.Count; i++)
        {
            BoundsInt room = dungeonGenerator.RoomsList[i];

            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(dungeonGenerator.Floor);
            roomFloor.IntersectWith(dungeonGenerator.Corridors);
        }
    }
    
  
}