using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public CollectableType CollectableType;

    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image itemImage;
    [SerializeField] private Transform itemUseCooldownTransform;
    [SerializeField] private float itemUseCooldownTimer = 3;
    private float timer;
    
    private List<Collectable> items;
    private FastList<Collectable> itemsBeingAdded;
    
    private Color White = Color.white;
    private Color Cody = new Color(1, 1, 1, 0.5f);

    private void Update()
    {
        timer -= Time.deltaTime;
        Cooldown();
    }

    public void Init()
    {
        items = new List<Collectable>();
        itemsBeingAdded = new FastList<Collectable>();
        itemImage.color = Cody;
        
        Cooldown();
    }

    public void AddItemBeingCollected(Collectable item) => itemsBeingAdded.Add(item);

    public void FinalizeAddItemBeingCollected(Collectable item)
    {
        if(itemsBeingAdded.Contains(item)) itemsBeingAdded.Remove(item);
        AddItem(item);
    }

    public void AddItem(Collectable item)
    {
        items.Add(item);
        Cooldown();

        itemImage.color = White;
    }

    public void UseItem()
    {
        if (items.Count > 0)
        {
            if (timer <= 0)
            {
                items[0].Use();
                items.RemoveAt(0);
                Cooldown();
                timer = itemUseCooldownTimer;
            
                if (items.Count == 0)
                {
                    countText.text = string.Empty;
                    itemImage.color = Cody;
                }
            }

            Debug.Log($"До отката: {timer}");
        }

        Debug.Log("Вещички нет:%(");
    }

    public bool IsFull() => items.Count + itemsBeingAdded.Count >= CollectableType.inventoryStackSize;

    public void Cooldown()
    {
        countText.text = items.Count == 0 ? string.Empty : items.Count.ToString();
        
        float state = timer;
        state /= itemUseCooldownTimer;

        if (state < 0) state = 0;

        itemUseCooldownTransform.localScale = new Vector3(state, state, 1);
    }
}