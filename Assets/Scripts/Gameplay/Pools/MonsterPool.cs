using UnityEngine;
using UnityEngine.Pool;

public class MonsterPool : Pool
{
    protected ObjectPool<Monster> pool;

    public override void Init(EntityManager entityManager, Player player, GameObject prefab, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
    {
        base.Init(entityManager, player, prefab, collectionCheck, defaultCapacity, maxSize);
        pool = new ObjectPool<Monster>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPooledItem, collectionCheck, defaultCapacity, maxSize);
    }

    public Monster Get() => pool.Get();

    public void Release(Monster monster) => pool.Release(monster);

    protected Monster CreatePooledItem()
    {
        Monster monster = Instantiate(prefab, transform).GetComponent<Monster>();
        monster.Init(entityManager, player);
        return monster;
    }

    protected void OnTakeFromPool(Monster monster) => monster.gameObject.SetActive(true);

    protected void OnReturnedToPool(Monster monster) => monster.gameObject.SetActive(false);

    protected void OnDestroyPooledItem(Monster monster) => Destroy(monster.gameObject);
}