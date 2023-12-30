using UnityEngine;
using UnityEngine.Pool;

public class CoinPool : Pool
{
    protected ObjectPool<Coin> pool;

    public override void Init(EntityManager entityManager, Player player, GameObject prefab, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
    {
        base.Init(entityManager, player, prefab, collectionCheck);
        pool = new ObjectPool<Coin>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPooledItem, base.collectionCheck, defaultCapacity, maxSize);
    }

    public Coin Get() => pool.Get();

    public void Release(Coin coin) => pool.Release(coin);

    protected Coin CreatePooledItem()
    {
        Coin coin = Instantiate(prefab, transform).GetComponent<Coin>();
        coin.Init(entityManager, player);
        return coin;
    }

    protected void OnTakeFromPool(Coin coin) => coin.gameObject.SetActive(true);
    
    protected void OnReturnedToPool(Coin coin) => coin.gameObject.SetActive(false);

    protected void OnDestroyPooledItem(Coin coin) => Destroy(coin.gameObject);
}