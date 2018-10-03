using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public int id;
    public ItemStack itemStack;

    public SlotType slotType;

    public enum SlotType
    {
        Everything,
        Equip,
        Weapon,
        Hull,
        Wing,
        Engine
    }


    [SerializeField]
    GameObject content;

    public Action<InventorySlot> onLeftClick;
    public Action<InventorySlot> onRightClick;
    public Action<InventorySlot> onMouseLeave;
    public Action<InventorySlot> onMouseEnter;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onMouseEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onMouseLeave?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            onLeftClick?.Invoke(this);
        if (eventData.button == PointerEventData.InputButton.Right)
            onRightClick?.Invoke(this);
    }
}
