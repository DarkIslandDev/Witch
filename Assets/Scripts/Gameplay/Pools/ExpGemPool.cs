using UnityEngine;
using UnityEngine.Pool;

public class ExpGemPool : Pool
{
    protected ObjectPool<ExpGem> pool;

    public override void Init(EntityManager entityManager, Player player, GameObject prefab, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
    {
        base.Init(entityManager, player, prefab, collectionCheck);
        pool = new ObjectPool<ExpGem>(CreatePooledItem, 
                            OnTakeFromPool,
                            OnReturnedToPool,
                            OnDestroyPooledItem, 
                            collectionCheck,
                            defaultCapacity,
                            maxSize);
    }

    public ExpGem Get() => pool.Get();

    public void Release(ExpGem expGem) => pool.Release(expGem);

    protected ExpGem CreatePooledItem()
    {
        ExpGem expGem = Instantiate(prefab, transform).GetComponent<ExpGem>();
        expGem.Init(entityManager, player);
        return expGem;
    }

    protected void OnTakeFromPool(ExpGem expGem) => expGem.gameObject.SetActive(true);

    protected void OnReturnedToPool(ExpGem expGem) => expGem.gameObject.SetActive(false);

    protected void OnDestroyPooledItem(ExpGem expGem) => Destroy(expGem.gameObject);
}