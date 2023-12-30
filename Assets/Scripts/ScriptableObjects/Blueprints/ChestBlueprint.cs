using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Chest", fileName = "Chest", order = 1)]
public class ChestBlueprint : ScriptableObject
{
    public bool abilityChest = false;
    public Sprite closedChest;
    public Sprite openingChest;
    public Sprite openChest;
    public LootTable<GameObject> lootTable;
}