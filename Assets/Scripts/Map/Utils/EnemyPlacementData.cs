using System;
using UnityEngine;

[Serializable]
public class EnemyPlacementData
{
    public int minQuantity;
    public int maxQuantity;
    public GameObject enemyPrefab;
    public Vector2Int enemySize;
}