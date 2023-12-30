using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    private Dictionary<CollectableType, InventorySlot> inventorySlotsByType;
    
    public void Init()
    {
        inventorySlotsByType = new Dictionary<CollectableType, InventorySlot>();

        for (var i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot inventorySlot = inventorySlots[i];
            inventorySlot.Init();
            inventorySlotsByType[inventorySlot.CollectableType] = inventorySlot;
        }
    }

    public bool RoomInInventory(Collectable item)
    {
        if (inventorySlotsByType.TryGetValue(item.collectableType, out InventorySlot inventorySlot))
            return !inventorySlot.IsFull();
        
        return false;
    }

    public bool TryGetInventorySlot(Collectable item, out InventorySlot inventorySlot)
    {
        if (inventorySlotsByType.TryGetValue(item.collectableType, out inventorySlot))
        {
            return true;
        }

        return false;
    }

    public InventorySlot AddItem(Collectable item)
    {
        if (inventorySlotsByType.ContainsKey(item.collectableType) &&
            !inventorySlotsByType[item.collectableType].IsFull())
        {
            inventorySlotsByType[item.collectableType].AddItem(item);
            return inventorySlotsByType[item.collectableType];
        }

        return null;
    }
    
    // public List<WeaponSlot> playerWeapons;
    // public List<EquipmentSlot> playerEquipments;
    // public int count;
    //
    // public UnityEvent<WeaponSlot> onWeaponAdded;
    // public UnityEvent<EquipmentSlot> onEquipmentAdded;
    //
    // public void EquipWeapon(Weapon weapon)
    // {
    //     WeaponSlot existingWeapon = playerWeapons.Find(weaponSlot => weaponSlot.item == weapon);
    //     
    //     if (playerWeapons.Count >= count) return;
    //     
    //     if (existingWeapon != null)
    //     {
    //         if (existingWeapon.level <= existingWeapon.item.maxItemLevel)
    //         {
    //             existingWeapon.IncreaseLevel(1);
    //
    //             onWeaponAdded?.Invoke(existingWeapon);
    //         }
    //     }
    //     else
    //     {
    //         WeaponSlot newWeapon = new WeaponSlot(weapon, 1);
    //         playerWeapons.Add(newWeapon);
    //
    //         onWeaponAdded?.Invoke(newWeapon);
    //     }
    // }
    //
    // public void EquipEquipment(Equipment equipment)
    // {
    //     EquipmentSlot existingEquipment = playerEquipments.Find(equipmentSlot => equipmentSlot.item == equipment);
    //     
    //     if(playerEquipments.Count >= count) return;
    //
    //     if (existingEquipment != null)
    //     {
    //         if (existingEquipment.level <= existingEquipment.item.maxItemLevel)
    //         {
    //             existingEquipment.IncreaseLevel(1);
    //             
    //             onEquipmentAdded?.Invoke(existingEquipment);
    //         }
    //     }
    //     else
    //     {
    //         EquipmentSlot newEquipment = new EquipmentSlot(equipment, 1);
    //         playerEquipments.Add(newEquipment);
    //         
    //         onEquipmentAdded?.Invoke(newEquipment);
    //     }
    // }
}