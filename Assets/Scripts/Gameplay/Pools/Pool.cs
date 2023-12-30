using UnityEngine;

public class Pool : MonoBehaviour
{
    protected EntityManager entityManager;
    protected Player player;
    protected GameObject prefab;
    protected bool collectionCheck = true;
    protected int defaultCapacity = 10;
    protected int maxSize = 10000;

    public virtual void Init(EntityManager entityManager, Player player, GameObject prefab, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
    {
        this.entityManager = entityManager;
        this.player = player;
        this.prefab = prefab;
        this.collectionCheck = collectionCheck;
        this.defaultCapacity = defaultCapacity;
        this.maxSize = maxSize;
    }
}