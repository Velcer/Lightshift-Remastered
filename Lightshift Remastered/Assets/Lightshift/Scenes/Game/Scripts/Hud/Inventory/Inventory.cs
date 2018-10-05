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

    public ItemStack heldItemStack;

    private void Awake()
    {
        ToggleInventory();
    }

    private void Start()
    {
        var generators = GetComponentsInChildren<SlotGenerator>();
        foreach (var generator in generators)
            Slots.AddRange(generator.Slots);

        foreach (var slot in Slots)
        {
            slot.onLeftClick += OnSlotLeftClicked;
            slot.onRightClick += OnSlotRightClicked;
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
        if (networkObject != null && networkObject.IsOwner && heldItemStack !=null)
            heldItemStack.DragItem();

        if (networkObject !=null && networkObject.IsOwner && Input.GetKeyDown(Settings.InventoryKey))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        var canvas = GetComponent<Canvas>();
        canvas.enabled = !canvas.enabled;
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
        AddItemToInventory("raider", 1, new ItemAttributeObject());
        AddItemToInventory("raider", 1, new ItemAttributeObject());
        AddItemToInventory("raider", 1, new ItemAttributeObject());
        AddItemToInventory("raider", 1, new ItemAttributeObject());
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

    private void OnSlotLeftClicked(InventorySlot slot)
    {
        networkObject.SendRpc(RPC_SLOT_LEFT_CLICKED, Receivers.Server, slot.id);
    }

    private void OnSlotRightClicked(InventorySlot slot)
    {
        networkObject.SendRpc(RPC_SLOT_RIGHT_CLICKED, Receivers.Server, slot.id);
    }

    private void ShowToolTip(ItemStack item)
    {
        if (toolTip == null)
            toolTip = Instantiate(toolTipPrefab, transform).GetComponent<InventoryTooltip>();
        else toolTip.gameObject.SetActive(true);

        toolTip.SetTitle(item.meta.displayName);
        toolTip.SetDescription(item.meta.description, true);
        if (item.meta.baseAcceleration != 0)
            toolTip.SetDescription($"Acceleration: {item.meta.baseAcceleration} (+2)");
        if (item.meta.baseSpeed != 0)
            toolTip.SetDescription($"Max Speed: {item.meta.baseSpeed} (+0.02)");
        if (item.meta.baseAgility != 0)
            toolTip.SetDescription($"Agility: {item.meta.baseAcceleration} (+10)");
        if (item.meta.baseHealth != 0)
            toolTip.SetDescription($"Health: {item.meta.baseHealth} (+200)");
        if (item.meta.baseShield != 0)
            toolTip.SetDescription($"Shield: {item.meta.baseShield} (+46)");

        toolTip.SetDescription($"<b>{item.meta.type.ToString()}</b>");
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
        var attributes = new ItemAttributeObject();
        SetItem(slotId, itemKey, amount, attributes);
    }

    public override void SetHeldItem(RpcArgs args)
    {
        var itemKey = args.GetNext<string>();
        var amount = args.GetNext<int>();

        SetHoldingItem(itemKey, amount);
    }

    private bool CanMoveToSlot(InventorySlot slot, ItemStack stack)
    {
        if (slot.slotType == InventorySlot.SlotType.Everything)
            return true;
        else
        {
            if (slot.slotType.ToString() == stack.meta.type.ToString())
                return true;
            else return false;
        }
    }

    public override void SlotRightClicked(RpcArgs args)
    {
        var slotId = args.GetNext<int>();
        var slot = Slots.First(s => s.id == slotId);
        if (slot == null)
            return;

        var itemStack = slot.itemStack;

        // If holding an item
        if (heldItemStack != null)
        {
            //If slot is empty
            if (slot.itemStack == null)
            {
                if (heldItemStack.Amount > 0 && CanMoveToSlot(slot, heldItemStack))
                {
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
        else if (slot.itemStack != null)
        {
            if (slot.itemStack.Amount > 1)
            {
                SetHoldingItem(slot.itemStack.meta.key, slot.itemStack.Amount / 2);
                itemStack.Amount -= slot.itemStack.Amount / 2;
                SendSetItem(slot.id, itemStack);
            }
            else if (itemStack.Amount == 1){
                SetHoldingItem(slot.itemStack.meta.key, itemStack.Amount);
                SetSlotEmpty(slot.id);
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
            // If the slot clicked is empty
            if (slot.itemStack == null && CanMoveToSlot(slot, heldItemStack))
            {
                SendSetItem(slot.id, heldItemStack);
                SetHandEmpty();
            }
            // If the slot clicked is NOT empty
            else
            {
                // If held item and clicked item are the same key
                if (slot.itemStack?.meta?.key == heldItemStack?.meta?.key)
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
                    if (!CanMoveToSlot(slot, heldItemStack))
                        return;
                    SendSetItem(slot.id, heldItemStack);
                    SendHoldingItem(itemStack);
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
