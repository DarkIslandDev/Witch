using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Level", fileName = "LevelBlueprint", order = 0)]
public class LevelBlueprint : ScriptableObject
{
    [Header("Максимальное время уровня")] 
    public float levelTime = 1800;

    [Header("Абилки")] 
    public List<GameObject> abilityPrefabs;

    [Header("Настройки монстров")] 
    public MonsterContainer[] monsters;
    public MiniBossContainer[] miniBosses;
    public BossContainer finalBoss;
    public MonsterSpawnTable monsterSpawnTable;
    [Header("Chest settings")] 
    public ChestBlueprint chestBlueprint;
    public float chestSpawnDelay = 30;
    public float chestSpawnAmount = 2;

    [Header("Exp gem settings")] 
    public int initialGemCount = 25;
    public GemType initialGemType = GemType.BlueXPGem;
    
    private Dictionary<int, (int, int)> monsterIndexMap;
    public Dictionary<int, (int, int)> MonsterIndexMap 
    { 
        get
        {
            if (monsterIndexMap == null)
            {
                monsterIndexMap = new Dictionary<int, (int, int)>();
                int monsterIndex = 0;
                for (int i = 0; i < monsters.Length; i++)
                {
                    for (int j = 0; j < monsters[i].monsterBlueprints.Length; j++)
                        monsterIndexMap[monsterIndex++] = (i, j);
                }
            }
            return monsterIndexMap;
        }
    }
}

[Serializable]
public class MonsterContainer
{
    public GameObject monsterPrefab;
    public MonsterBlueprint[] monsterBlueprints;
}

[Serializable]
public class MiniBossContainer : BossContainer
{
    public float spawnTime;
}

[Serializable]
public class BossContainer
{
    public GameObject bossPrefab;
    public BossMonsterBlueprint bossBlueprint;
}
