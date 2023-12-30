using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    // [SerializeField] protected Image itemSprite;
    // [SerializeField] protected Slot assignedInvenvtorySlot;
    //
    // public PlayerInventoryUI inventoryUI;
    //
    // public virtual void Init(Slot slot, PlayerInventoryUI inventory)
    // {
    //     assignedInvenvtorySlot = slot;
    //     inventoryUI = inventory;
    //     UpdateUISlot(slot);
    // }
    //
    // public void UpdateUISlot(Slot slot)
    // {
    //     if (slot.item != null)
    //     {
    //         itemSprite.sprite = slot.item.itemSprite;
    //         itemSprite.enabled = true;
    //     }
    //     else
    //     {
    //         ClearSlot();
    //     }
    // }
    //
    // public void UpdateUISlot()
    // {
    //     if(assignedInvenvtorySlot != null) UpdateUISlot(assignedInvenvtorySlot);
    // }
    //
    // private void ClearSlot()
    // {
    //     assignedInvenvtorySlot?.ClearSlot();
    //     itemSprite.enabled = false;
    //     itemSprite.sprite = null;
    // }
}