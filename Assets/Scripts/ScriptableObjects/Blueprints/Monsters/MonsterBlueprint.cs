using UnityEngine;

public class MonsterBlueprint : ScriptableObject
{
    [Header("Stats")] 
    public new string name;
    public float hp;
    public float atk;
    public float recovery;
    public float armor;
    public float atkSpeed;
    public float moveSpeed;
    public float acceleration;
    [Header("Drops")] 
    public LootTable<GemType> gemLootTable;
    public LootTable<CoinType> coinLootTable;
}