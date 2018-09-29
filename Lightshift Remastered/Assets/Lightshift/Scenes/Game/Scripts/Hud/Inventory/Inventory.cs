using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public List<InventorySlot> Slots = new List<InventorySlot>();

    public int SlotCount;
    public bool generateInventorySlots = true;

    [SerializeField]
    GameObject itemSlotPrefab;

    [SerializeField]
    GameObject inventoryPanel;

    private void Awake()
    {
        if (generateInventorySlots)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                var obj = Instantiate(itemSlotPrefab, inventoryPanel.transform);
                var slot = obj.GetComponentInChildren<InventorySlot>();
                Slots.Add(slot);
                slot.OnInventoryClick += OnInventoryClick;
            }
        }
    }

    private void OnInventoryClick(InventorySlot slot)
    {
        throw new NotImplementedException();
    }
}
