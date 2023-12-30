using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CollectionMode
{
    FromGround,
    FromChest
}
public abstract class Collectable : MonoBehaviour
{
    [Header("Type")] 
    public CollectableType collectableType;

    [Header("Spawn animation")] 
    [SerializeField] protected float saSpeed = 1;
    [SerializeField] protected float saHeight = 1;
    [SerializeField] protected float saOffsetMax = 0.2f;

    [Header("Collect animation")] 
    [SerializeField] protected float juiciness = 2;
    [SerializeField] protected float lerpTime = 1;

    [Header("Attributes")] 
    [SerializeField] protected bool magnetic = false;
    protected EntityManager entityManager;
    protected Player player;
    protected ZPositioner zPositioner;
    protected new Collider2D collider;
    protected bool beingCollected;

    protected virtual void Awake()
    {
        collider = GetComponent<Collider2D>();
        zPositioner = gameObject.AddComponent<ZPositioner>();
    }

    public virtual void Init(EntityManager entityManager, Player player)
    {
        this.entityManager = entityManager;
        this.player = player;
        zPositioner.Init(player.transform);
    }

    public virtual void Setup(bool spawnAnimation = true, bool collectableDuringSpawn = true)
    {
        collider.enabled = !spawnAnimation || collectableDuringSpawn;
        beingCollected = false;

        if (magnetic) entityManager.MagneticCollectables.Add(this);

        if (spawnAnimation) StartCoroutine(SpawnAnimation());
        
        gameObject.SetActive(true);
    }

    public virtual void Collect(CollectionMode collectionMode = CollectionMode.FromGround)
    {
        // Не собирать снова, если уже собрано
        if(beingCollected) return;

        // Не собирать, если можно хранить в инвентаре и инвентарь полон
        bool storeInInventory = collectableType.inventoryStackSize > 0;
        bool hasInInventorySlot = entityManager.PlayerInventory.TryGetInventorySlot(this, out InventorySlot inventorySlot);
        
        if(storeInInventory && hasInInventorySlot && inventorySlot.IsFull()) return;

        // Отмечаем, что это собирается, чтобы оно случайно не было собрано во второй раз
        beingCollected = true;
        collider.enabled = false;
        
        if(magnetic) entityManager.MagneticCollectables.Remove(this);

        // Переносим в инвентарь игрока, если его можно сохранить в инвентаре, в противном случае отправляем самому игроку
        if (storeInInventory && hasInInventorySlot)
        {
            StartCoroutine(FlyToInventory(inventorySlot, collectionMode));
        }
        else
        {
            StartCoroutine(FlyToPlayer(collectionMode));
        }
    }

    public void Use()
    {
        OnCollected();
    }

    protected abstract void OnCollected();

    protected virtual IEnumerator FlyToPlayer(CollectionMode collectionMode = CollectionMode.FromGround)
    {
        if (collectionMode == CollectionMode.FromChest)
        {
            yield return StartCoroutine(ChestAnimation());
            zPositioner.enabled = true;
        }

        float distance = Vector2.Distance(transform.position, player.CenterTransform.position);
        
        if (distance == 0.0f) distance = Mathf.Epsilon;
        
        float c = juiciness / distance;
        float timeScale = 1.0f / (lerpTime * MathF.Sqrt(distance));
        float t = -Time.deltaTime * timeScale;
        Vector3 pickupPos = transform.position;

        while (t < 1)
        {
            t += Time.deltaTime * timeScale;
            float lerpT = EasingUtils.EaseInBack(t, c);
            
            if(lerpT >= 1) break;
            transform.position = Vector3.LerpUnclamped(pickupPos, player.CenterTransform.position, lerpT);
            yield return null;
        }

        transform.position = player.CenterTransform.position;
        yield return null;
        OnCollected();
    }

    protected virtual IEnumerator FlyToInventory(InventorySlot inventorySlot,
        CollectionMode collectionMode = CollectionMode.FromGround)
    {
        // Запускаем предмет в воздух перед отправкой в инвентарь
        // если собрано из сундука
        inventorySlot.AddItemBeingCollected(this);

        if (collectionMode == CollectionMode.FromChest) yield return StartCoroutine(ChestAnimation());

        float t = 0;
        float c = 0;
        float timeScale = 2.0f;
        t = -Time.deltaTime * timeScale;
        Vector3 pickupPos = transform.position;

        while (t < 1)
        {
            t += Time.deltaTime * timeScale;
            float lerpT = EasingUtils.EaseInBack(t, c);
            
            if(lerpT >= 1) break;

            transform.position = Vector3.LerpUnclamped(pickupPos,
                Camera.main.ScreenToWorldPoint(inventorySlot.transform.position), lerpT);
            yield return null;
        }

        transform.position = Camera.main.ScreenToWorldPoint(inventorySlot.transform.position);
        yield return null;
        gameObject.SetActive(false);
        inventorySlot.FinalizeAddItemBeingCollected(this);
    }

    protected virtual IEnumerator SpawnAnimation()
    {
        zPositioner.enabled = false;
        float t = 0;
        float horizontalSpeed = Random.Range(-saOffsetMax, saOffsetMax);
        Vector3 spawnPosition = transform.position;

        while (!beingCollected && t < 1)
        {
            transform.position = spawnPosition + Vector3.up * EasingUtils.Bounce(t, saHeight) +
                                 Vector3.right * horizontalSpeed * t;

            t += Time.deltaTime * saSpeed;
            yield return null;
        }

        collider.enabled = true;
        zPositioner.enabled = true;
    }

    protected virtual IEnumerator ChestAnimation()
    {
        float t = 0;
        float horizontalSpeed = Random.Range(-saOffsetMax, saOffsetMax);
        Vector3 spawnPosition = transform.position;
        while (t < 0.2f)
        {
            zPositioner.enabled = false;
            transform.position = spawnPosition + Vector3.up * EasingUtils.Bounce(t, saHeight) +
                                 Vector3.right * horizontalSpeed * t;

            t += Time.deltaTime * saSpeed;
            yield return null;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider == player.CollectableCollider) Collect();
    }
}