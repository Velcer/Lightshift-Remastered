using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour {

    public int id;
    public ItemStack itemStack;

    public Action<InventorySlot> OnInventoryClick;
}
