using UnityEngine;

[System.Serializable]
public class Loot<T>
{
        public T item;
        [Range(0, 1)] public float dropChance;
        [HideInInspector] public CoinType coinType = CoinType.Coin;
}