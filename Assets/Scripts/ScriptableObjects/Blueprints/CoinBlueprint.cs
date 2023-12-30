using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Coin", fileName = "Coin", order = 1)]
public class CoinBlueprint : ScriptableObject
{
    public EnumDataContainer<CoinType, Sprite> coinSprites;
}