using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInventoryUI : InventoryUI
{
    // public PlayerInventory playerInventory;
    //
    // [SerializeField] private GameObject weaponSlotUIPrefab;
    // [SerializeField] private GameObject equipmentSlotUIPrefab;
    // [SerializeField] private Transform weaponSlotContainer;
    // [SerializeField] private Transform equipmentSlotContainer;
    //
    // [SerializeField] private List<WeaponSlotUI> weaponSlots;
    // [SerializeField] private List<EquipmentSlotUI> equipmentSlots;
    //
    // private void Start()
    // {
    //     playerInventory = InputManager.instance.components.playerInventory;
    //     
    //     if (playerInventory != null)
    //     {
    //         inventory = playerInventory.inventory;
    //         
    //         inventory.onWeaponAdded.AddListener(AddWeaponSlot);
    //         inventory.onEquipmentAdded.AddListener(AddEquipmentSlot);
    //         
    //         inventory.EquipWeapon(playerInventory.startItem);
    //     }
    // }
    //
    // public void AddWeaponSlot(WeaponSlot weaponSlot)
    // {
    //     if (weaponDictionary.TryGetValue(weaponSlot, out WeaponSlotUI slot))
    //     {
    //         if(weaponSlot.level >= weaponSlot.item.maxItemLevel) return;
    //         
    //         slot.UpdateUISlot();
    //     }
    //     else
    //     {
    //         GameObject slotGO = Instantiate(weaponSlotUIPrefab, weaponSlotContainer);
    //         slotGO.name = $"{weaponSlot.item.itemName}Slot";
    //         WeaponSlotUI newSlot = slotGO.GetComponent<WeaponSlotUI>();
    //         weaponSlots.Add(newSlot);
    //         newSlot.Init(weaponSlot, this);
    //         weaponDictionary.Add(weaponSlot, newSlot);
    //     }
    // }
    //
    // public void AddUsableItems(EquipableItem equipableItem)
    // {
    //     
    // }
    //
    // public void AddEquipmentSlot(EquipmentSlot equipmentSlot)
    // {
    //     if (equipmentDictionary.TryGetValue(equipmentSlot, out EquipmentSlotUI slot))
    //     {
    //         if(equipmentSlot.level >= equipmentSlot.item.maxItemLevel) return;
    //         
    //         slot.UpdateUISlot();
    //     }
    //     else
    //     {
    //         GameObject slotGO = Instantiate(weaponSlotUIPrefab, weaponSlotContainer);
    //         slotGO.name = $"{equipmentSlot.item.itemName}Slot";
    //         EquipmentSlotUI newSlot = slotGO.GetComponent<EquipmentSlotUI>();
    //         equipmentSlots.Add(newSlot);
    //         newSlot.Init(equipmentSlot, this);
    //         equipmentDictionary.Add(equipmentSlot, newSlot);
    //     }
    // }
}