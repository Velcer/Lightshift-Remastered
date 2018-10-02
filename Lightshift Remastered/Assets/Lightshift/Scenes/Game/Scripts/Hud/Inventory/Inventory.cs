using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : InventoryBehavior {

    public List<InventorySlot> Slots = new List<InventorySlot>();

    [SerializeField]
    GameObject toolTipPrefab;

    [SerializeField]
    GameObject itemStackPrefab;

    InventoryTooltip toolTip;

    public InventorySlot selectedSlot;

    public ItemStack heldItemStack;

    private void Start()
    {
        var generators = GetComponentsInChildren<SlotGenerator>();
        foreach (var generator in generators)
            Slots.AddRange(generator.Slots);

        foreach (var slot in Slots)
        {
            slot.onClick += OnSlotClicked;
            slot.onMouseLeave += OnSlotMouseLeave;
            slot.onMouseEnter += OnSlotMouseEnter;
        }
    }

    protected override void NetworkStart()
    {
        base.NetworkStart();

        if (networkObject.IsServer)
            StartCoroutine(AddItems());
    }

    private void Update()
    {
        if (networkObject.IsOwner && heldItemStack !=null)
            heldItemStack?.DragItem();
    }

    IEnumerator AddItems()
    {
        yield return new WaitForEndOfFrame();

        AddItemToInventory("test", 25, new ItemAttributeObject());
        AddItemToInventory("test2", 13, new ItemAttributeObject());
        AddItemToInventory("test", 30, new ItemAttributeObject());
        AddItemToInventory("test2", 30, new ItemAttributeObject());
        AddItemToInventory("test", 80, new ItemAttributeObject());
        AddItemToInventory("test2", 130, new ItemAttributeObject());
        AddItemToInventory("test", 3, new ItemAttributeObject());
        AddItemToInventory("test2", 300, new ItemAttributeObject());
        AddItemToInventory("test", 25, new ItemAttributeObject());
        AddItemToInventory("test2", 803, new ItemAttributeObject());
        AddItemToInventory("test", 60, new ItemAttributeObject());
        AddItemToInventory("test2", 300, new ItemAttributeObject());
    }

    private void SetItem(int slotId, string key, int amount, ItemAttributeObject attributeObj = null)
    {
        var slot = Slots.First(s => s.id == slotId);
        if (slot == null)
            return;

        if (slot.itemStack != null)
        {
            Destroy(slot.itemStack.gameObject);
        }

        print($"We're checking key {key}");
        if (key == "" || key == " ")
            return;

        var obj = Instantiate(itemStackPrefab, slot.transform);
        var itemStack = obj.GetComponent<ItemStack>();

        itemStack.Amount = amount;

        itemStack.SetItemMeta(key, attributeObj);

        slot.itemStack = itemStack;
    }

    private void SetHoldingItem(string key, int amount)
    {
        if (heldItemStack != null)
            Destroy(heldItemStack.gameObject);

        if (key == "" || key == " ")
            return;

        var obj = Instantiate(itemStackPrefab, transform);
        var itemStack = obj.GetComponent<ItemStack>();

        itemStack.Amount = amount;

        itemStack.SetItemMeta(key);

        heldItemStack = itemStack;
    }

    private void OnSlotMouseEnter(InventorySlot slot)
    {
        if (slot.itemStack != null)
            ShowToolTip(slot.itemStack);
    }

    private void OnSlotMouseLeave(InventorySlot slot)
    {
        HideToolTip();
    }

    private void OnSlotClicked(InventorySlot slot)
    {
        print("we clickkkeddddd");
        networkObject.SendRpc(RPC_SLOT_LEFT_CLICKED, Receivers.Server, slot.id);
    }

    private void ShowToolTip(ItemStack item)
    {
        if (toolTip == null)
            toolTip = Instantiate(toolTipPrefab, transform).GetComponent<InventoryTooltip>();
        else toolTip.gameObject.SetActive(true);

        toolTip.SetTitle(item.meta.displayName);
        toolTip.SetDescription(item.meta.description, true);
        toolTip.SetDescription("+50 Health");
        toolTip.SetDescription("+20 Shield");
        toolTip.SetDescription("+30 Agility");
        toolTip.SetDescription("Equippable");
        toolTip.Refresh();
        toolTip.transform.position = item.transform.position + new Vector3(145, -60);

    }

    private void HideToolTip() => toolTip?.gameObject.SetActive(false);

    public override void SetSlotItem(RpcArgs args)
    {
        var slotId = args.GetNext<int>();
        var itemKey = args.GetNext<string>();
        var amount = args.GetNext<int>();
        var byteArray = args.GetNext<byte[]>();
        //var attributes = ItemAttributeObject.Desserialize(byteArray);
        var attributes = new ItemAttributeObject();
        SetItem(slotId, itemKey, amount, attributes);
        print("set item");
    }

    public override void SetHeldItem(RpcArgs args)
    {
        var itemKey = args.GetNext<string>();
        var amount = args.GetNext<int>();

        SetHoldingItem(itemKey, amount);
    }

    public override void SlotRightClicked(RpcArgs args)
    {
        var slotId = args.GetNext<int>();
        var slot = Slots.First(s => s.id == slotId);
        if (slot == null)
            return;

        var itemStack = slot.itemStack;

        print($"Here: {heldItemStack?.meta?.displayName}");
        // If holding an item
        if (heldItemStack != null)
        {
            //If slot is empty
            if (slot.itemStack == null)
            {
                if (heldItemStack.Amount > 0 && slot.slotType.ToString() == heldItemStack.meta.type.ToString())
                {
                    itemStack.Amount++;
                    heldItemStack.Amount--;
                    SendSetItem(slot.id, heldItemStack.meta.key, 1, heldItemStack.meta.itemAttributeObject);

                    if (heldItemStack.Amount == 0)
                        SetHandEmpty();
                    else SendHoldingItem(heldItemStack);
                }
            }
            else
            {
                //If both item stacks are the same key
                if (slot.itemStack.meta.key == heldItemStack.meta.key)
                {

                    if (heldItemStack.Amount > 0 && itemStack.Amount < itemStack.meta.maxStack)
                    {
                        itemStack.Amount++;
                        heldItemStack.Amount--;
                        SendSetItem(slot.id, itemStack);

                        if (heldItemStack.Amount == 0)
                            SetHandEmpty();
                        else SendHoldingItem(heldItemStack);
                    }
                }
            }
        }
    }

    public override void SlotLeftClicked(RpcArgs args)
    {
        var slotId = args.GetNext<int>();
        var slot = Slots.First(s => s.id == slotId);
        if (slot == null)
            return;

        var itemStack = slot.itemStack;

        // If holding an item
        if (heldItemStack != null)
        {
            print("heldItem not null");
            // If the slot clicked is empty
            if (slot.itemStack == null/* && slot.slotType.ToString() == heldItemStack.meta.type.ToString()*/)
            {
                SendSetItem(slot.id, heldItemStack);
                SetHandEmpty();
                print("Should have placed");
            }
            // If the slot clicked is NOT empty
            else
            {
                print("ahh..");
                // If held item and clicked item are the same key
                if (slot.itemStack.meta.key == heldItemStack.meta.key)
                {
                    var slotAmount = slot.itemStack.Amount;
                    var heldAmount = heldItemStack.Amount;
                    var maxStack = heldItemStack.meta.maxStack;
                    if (slotAmount + heldAmount > maxStack)
                    {
                        heldItemStack.Amount = (slotAmount + heldAmount) - maxStack;
                        itemStack.Amount = maxStack;
                        SendHoldingItem(heldItemStack);
                        SendSetItem(slot.id, itemStack);
                    }
                    else
                    {
                        itemStack.Amount = slotAmount + heldAmount;
                        SetHandEmpty();
                        SendSetItem(slot.id, itemStack);
                    }
                }
                //If they are NOT the same key, swap positions
                else
                {
                    //if (slot.slotType.ToString() != heldItemStack.meta.type.ToString())
                    //    return;
                    SendSetItem(slot.id, heldItemStack);
                    SendHoldingItem(itemStack);
                    print("here");
                }
            }
        }
        //Pickup clicked item
        else if (itemStack !=null)
        {
            SetSlotEmpty(slot.id);
            SendHoldingItem(itemStack);
        }
    }

    private void SendSetItem(int slotId, ItemStack itemStack)
    {
        if (itemStack == null)
            networkObject.SendRpc(RPC_SET_SLOT_ITEM, Receivers.ServerAndOwner, slotId, " " , 0, new ItemAttributeObject().Serialize());
        else networkObject.SendRpc(RPC_SET_SLOT_ITEM, Receivers.ServerAndOwner, slotId, itemStack.meta.key, itemStack.Amount, itemStack.meta.itemAttributeObject.Serialize());
    }

    private void SendSetItem(int slotId, string key, int amount, ItemAttributeObject itemAttributeObject)
    {
        networkObject.SendRpc(RPC_SET_SLOT_ITEM, Receivers.ServerAndOwner, slotId, key, amount, itemAttributeObject.Serialize());
    }

    private void SetSlotEmpty(int slotId) => SendSetItem(slotId, null);
    private void SetHandEmpty() => SendHoldingItem(null);
    private void SendHoldingItem(ItemStack itemStack)
    {
        if (itemStack == null)
            networkObject.SendRpc(RPC_SET_HELD_ITEM, Receivers.ServerAndOwner, " ", 0, new ItemAttributeObject().Serialize());
        else networkObject.SendRpc(RPC_SET_HELD_ITEM, Receivers.ServerAndOwner, itemStack.meta.key, itemStack.Amount, itemStack.meta.itemAttributeObject.Serialize());
    }

    public void AddItemToInventory(string key, int amount, ItemAttributeObject itemAttributeObject)
    {
        var slot = Slots.First(s => s.itemStack == null && s.id < 500);
        SendSetItem(slot.id, key, amount, itemAttributeObject);
    }
}
