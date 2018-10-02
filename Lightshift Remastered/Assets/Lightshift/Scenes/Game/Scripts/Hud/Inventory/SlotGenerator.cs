using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotGenerator : MonoBehaviour {

    public int SlotCount;

    public int IdStartNumber;

    public InventorySlot.SlotType slotType;

    public List<InventorySlot> Slots = new List<InventorySlot>();

    [SerializeField]
    GameObject itemSlotPrefab;

    [SerializeField]
    GameObject inventoryPanel;

    private void Awake()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            var obj = Instantiate(itemSlotPrefab, inventoryPanel.transform);
            var slot = obj.GetComponentInChildren<InventorySlot>();
            slot.id = IdStartNumber++;
            slot.slotType = slotType;
            Slots.Add(slot);
        }
    }
}
