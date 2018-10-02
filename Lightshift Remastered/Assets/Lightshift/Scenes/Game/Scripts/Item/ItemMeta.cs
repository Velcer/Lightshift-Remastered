using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemMeta : MonoBehaviour {

    public string key;
    public string displayName;
    public string description;
    public int maxStack;

    public float baseSpeed;
    public float baseAcceleration;
    public float baseAgility;
    public float baseShield;
    public float baseHealth;

    public ItemAttributeObject itemAttributeObject = new ItemAttributeObject();

    public Sprite icon;
    public ItemType type;

    public enum ItemType
    {
        Equip,
        Module,
        Weapons,
        Material
    }
}
