// using System;
// using TMPro;
// using UnityEngine;
// using UnityEngine.Serialization;
// using UnityEngine.UI;
//
// public class UsableItemSlot : SlotUI
// {
//     [Header("UI Components")]
//     [SerializeField] private TextMeshProUGUI quickSlotText;
//     [SerializeField] private Transform quickSlotCooldownTransform;
//
//     [SerializeField] private float itemCooldown = 3;
//     private float timer;
//     
//     private void Start()
//     {
//         UpdateQuickSlot();
//     }
//
//     private void Update()
//     {
//         timer -= Time.deltaTime;
//         UpdateQuickSlot();
//     }

    // public override void Init(Slot slot, PlayerInventoryUI inventory)
    // {
    //     base.Init(slot, inventory);
    // }
    //
    // public void AddItem(EquipableItem newItem)
    // {
    //     if (assignedInvenvtorySlot.item != null)
    //     {
    //         assignedInvenvtorySlot.level++;
    //         UpdateQuickSlot();
    //     }
    //     else
    //     {
    //         assignedInvenvtorySlot.item = newItem;
    //         assignedInvenvtorySlot.level = 1;
    //         UpdateQuickSlot();
    //     }
    // }
    //
    // public void UseItem()
    // {
    //     if (assignedInvenvtorySlot.item != null)
    //     {
    //         if (assignedInvenvtorySlot.level != 0)
    //         {
    //             if (timer <= 0)
    //             {
    //                 assignedInvenvtorySlot.item.UseItemEffect();
    //                 assignedInvenvtorySlot.level--;
    //                 UpdateQuickSlot();
    //                 timer = itemCooldown;
    //             }
    //         }
    //     }
    //     
    // }
    //
    // public void UpdateQuickSlot()
    // {
    //     quickSlotText.text = assignedInvenvtorySlot.level >= 1 ? assignedInvenvtorySlot.level.ToString() : string.Empty;
    //
    //     itemSprite.color = assignedInvenvtorySlot.level == 0 ? Color.gray : Color.white;
    //     
    //     // Cooldown
    //     float state = timer;
    //     state /= itemCooldown;
    //
    //     if (state < 0) state = 0;
    //
    //     quickSlotCooldownTransform.localScale = new Vector3(state, state, 1);
    // }
// }