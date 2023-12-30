using UnityEngine;

[CreateAssetMenu(fileName = "Exp Gem", menuName = "Blueprints/ExpGem", order = 1)]
public class ExpGemBlueprint : ScriptableObject
{
    public EnumDataContainer<GemType, Sprite, Color> gemSpritesAndColors;
}