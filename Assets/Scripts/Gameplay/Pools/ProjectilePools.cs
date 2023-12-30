using UnityEngine.Pool;
using UnityEngine;

public class ProjectilePool : Pool
{
    protected ObjectPool<Projectile> pool;

    public override void Init(EntityManager entityManager, Player player, GameObject prefab, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
    {
        base.Init(entityManager, player, prefab, collectionCheck, defaultCapacity, maxSize);
        pool = new ObjectPool<Projectile>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPooledItem, collectionCheck, defaultCapacity, maxSize);
    }

    public Projectile Get() => pool.Get();

    public void Release(Projectile projectile) => pool.Release(projectile);

    protected Projectile CreatePooledItem()
    {
        Projectile projectile = Instantiate(prefab, transform).GetComponent<Projectile>();
        projectile.Init(entityManager, player);
        return projectile;
    }

    protected void OnTakeFromPool(Projectile projectile) => projectile.gameObject.SetActive(true);

    protected void OnReturnedToPool(Projectile projectile) => projectile.gameObject.SetActive(false);

    protected void OnDestroyPooledItem(Projectile projectile) => Destroy(projectile.gameObject);
}