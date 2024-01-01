using System;
using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
    protected ChestBlueprint chestBlueprint;
    protected EntityManager entityManager;
    protected Player player;
    protected ZPositioner zPositioner;
    protected Transform chestItemParent;
    protected SpriteRenderer spriteRenderer;
    protected bool opened = false;

    public void Init(EntityManager entityManager, Player player, Transform chestItemParent)
    {
        this.entityManager = entityManager;
        this.player = player;
        this.chestItemParent = chestItemParent;
        (zPositioner = gameObject.AddComponent<ZPositioner>()).Init(player.transform);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Setup(ChestBlueprint chestBlueprint)
    {
        this.chestBlueprint = chestBlueprint;
        transform.localScale = Vector3.one;
        spriteRenderer.sprite = chestBlueprint.closedChest;
        opened = false;
        StartCoroutine(Appear());
    }

    private void SpawnLoot(Loot<GameObject> loot, bool openedByPlayer = true)
    {
        GameObject item = Instantiate(loot.item, chestItemParent);
        item.transform.position = transform.position;
        transform.position += Vector3.back * 0.001f;
        Collectable collectable = item.GetComponent<Collectable>();
        collectable.Init(entityManager, player);
        Coin coin = collectable as Coin;
        if (coin != null)
        {
            coin.Setup(transform.position, loot.coinType, true, true);
        }
        else
        {
            collectable.Setup(true, true);
        }
        
        if(openedByPlayer) collectable.Collect(CollectionMode.FromChest);
    }

    public void OpenChest(bool openedByPlayer = true)
    {
        if (!opened)
        {
            opened = true;
            StartCoroutine(Open());
        }
    }
    
    // НА ПАДУМАТЬ
    private IEnumerator Open(bool openedByPlayer = true)
    {
        spriteRenderer.sprite = chestBlueprint.openingChest;
        bool spawnLoot = !chestBlueprint.abilityChest || !entityManager.AbilitySelectionDialog.HasAvailableAbilities();
        
        if (spawnLoot) SpawnLoot(chestBlueprint.lootTable.DropLootObject(), openedByPlayer);

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.sprite = chestBlueprint.openChest;
        
        if(!spawnLoot) entityManager.AbilitySelectionDialog.Open(false);

        yield return new WaitForSeconds(0.15f);

        float t = 0;

        while (t < 1.0f)
        {
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, EasingUtils.EaseOutQuart(t));
            t += Time.deltaTime;
            yield return null;
        }
        
        entityManager.DespawnChest(this);
    }

    private IEnumerator Appear()
    {
        GetComponent<Collider2D>().enabled = false;
        float t = 0;
        while (t < 1.0f)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, EasingUtils.EaseInQuart(t));
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject == player.gameObject) OpenChest();
    }
}