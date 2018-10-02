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
        Equip,
        Module,
        Weapons,
        Material
    }


    [SerializeField]
    GameObject content;

    public Action<InventorySlot> onClick;
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
        onClick?.Invoke(this);
    }
}
