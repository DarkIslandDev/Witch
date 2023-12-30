using UnityEngine;

[System.Serializable]
public class LootTable<T>
{
    public Loot<T>[] lootTable;

    /// <summary>
    /// Используй этот метод для таблицы добычи, где шансы не состовляют 100%
    /// т.е. лут не всегда выпадает
    /// </summary>
    public bool TryDropLoot(out T loot)
    {
        if (TryDropLootObject(out Loot<T> lootObject))
        {
            loot = lootObject.item;
            return true;
        }
        else
        {
            loot = default(T);
            return false;
        }
    }

    /// <summary>
    /// Используй этот метод для таблицы добычи, где шансы не состовляют 100%
    /// т.е. лут не всегда выпадает
    /// </summary>
    public bool TryDropLootObject(out Loot<T> loot)
    {
        float rand = Random.Range(0f, 1.0f);
        float cumulative = 0;

        foreach (Loot<T> drop in lootTable)
        {
            cumulative += drop.dropChance;
            if (rand < cumulative)
            {
                loot = drop;
                return true;
            }
        }

        loot = null;
        return false;
    }

    /// <summary>
    /// Используй это для таблицы добычи, где сумма шансов достигает 100%
    /// т.е. добыча всегда выпадает
    /// </summary>
    public T DropLoot() => DropLootObject().item;

    /// <summary>
    /// Используй это для таблицы добычи, где сумма шансов достигает 100%
    /// т.е. добыча всегда выпадает
    /// </summary>
    public Loot<T> DropLootObject()
    {
        float rand = Random.Range(0f, 1.0f);
        float cumulative = 0;

        foreach (Loot<T> drop in lootTable)
        {
            cumulative += drop.dropChance;
            if (rand < cumulative)
            {
                return drop;
            }
        }

        // Отказоустойчивость в случае ошибок точности с плавающей запятой
        // или ошибки пользователя при настройке таблицы добычи
        if (lootTable.Length > 0)
        {
            Debug.LogError("Не получилось дропнуть лут, убедись что шансы выпадения состовляют 100%");
            return lootTable[^1];
        }

        Debug.LogError("Не получилось дропнуть лут, таблица добычи пуста");
        return null;
    }
}