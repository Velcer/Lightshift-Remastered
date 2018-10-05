using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {

    public static KeyCode DownKey { get; set; }
    public static KeyCode UpKey { get; set; }
    public static KeyCode LeftKey { get; set; }
    public static KeyCode RightKey { get; set; }
    public static KeyCode panKey { get; set; }
    public static KeyCode ShootKey1 { get; set; }
    public static KeyCode ShootKey2 { get; set; }
    public static KeyCode ShootKey3 { get; set; }
    public static KeyCode ShootKey4 { get; set; }
    public static KeyCode ShootKey5 { get; set; }
    public static KeyCode RespawnKey { get; set; }
    public static KeyCode CruiseKey { get; set; }
    public static KeyCode ChatKey { get; set; }
    public static KeyCode ChatKey2 { get; set; }
    public static KeyCode MapKey { get; set; }
    public static KeyCode SettingsMenuKey { get; set; }
    public static KeyCode PlayerMenuKey { get; set; }
    public static KeyCode InventoryKey { get; set; }
    public static KeyCode ShootKey { get; set; }
    public static bool MouseAim { get; set; }
    public static bool KeysLocked { get; set; }
    public static bool FireWithWeaponHotKeys { get; set; }

    void Start()
    {
        SetupControls();
        RefreshControls();
    }

    public void SetupControls()
    {
        if (!PlayerPrefs.HasKey("mouseAim"))
            PlayerPrefs.SetString("mouseAim", "False");

        if (!PlayerPrefs.HasKey("weaponHotkeys"))
            PlayerPrefs.SetString("weaponHotkeys", "True");
        
        if (!PlayerPrefs.HasKey("mapKey"))
            PlayerPrefs.SetString("mapKey", "M");

        if (!PlayerPrefs.HasKey("down"))
            PlayerPrefs.SetString("down", "DownArrow");

        if (!PlayerPrefs.HasKey("forward"))
            PlayerPrefs.SetString("forward", "UpArrow");

        if (!PlayerPrefs.HasKey("turnLeft"))
            PlayerPrefs.SetString("turnLeft", "LeftArrow");

        if (!PlayerPrefs.HasKey("turnRight"))
            PlayerPrefs.SetString("turnRight", "RightArrow");

        if (!PlayerPrefs.HasKey("shootKey"))
            PlayerPrefs.SetString("shootKey", "Space");

        if (!PlayerPrefs.HasKey("shootKey1"))
            PlayerPrefs.SetString("shootKey1", "Alpha1");

        if (!PlayerPrefs.HasKey("shootKey2"))
            PlayerPrefs.SetString("shootKey2", "Alpha2");

        if (!PlayerPrefs.HasKey("shootKey3"))
            PlayerPrefs.SetString("shootKey3", "Alpha3");

        if (!PlayerPrefs.HasKey("shootKey4"))
            PlayerPrefs.SetString("shootKey4", "Alpha4");

        if (!PlayerPrefs.HasKey("shootKey5"))
            PlayerPrefs.SetString("shootKey5", "Alpha5");

        if (!PlayerPrefs.HasKey("cruiseKey"))
            PlayerPrefs.SetString("cruiseKey", "C");

        if (!PlayerPrefs.HasKey("respawnKey"))
            PlayerPrefs.SetString("respawnKey", "Space");

        if (!PlayerPrefs.HasKey("chatKey"))
            PlayerPrefs.SetString("chatKey", "KeypadEnter");

        if (!PlayerPrefs.HasKey("chatKey2"))
            PlayerPrefs.SetString("chatKey2", "Return");

        if (!PlayerPrefs.HasKey("settingsMenuKey"))
            PlayerPrefs.SetString("settingsMenuKey", "H");

        if (!PlayerPrefs.HasKey("inventoryKey"))
            PlayerPrefs.SetString("inventoryKey", "I");

        if (!PlayerPrefs.HasKey("playerMenuKey"))
            PlayerPrefs.SetString("playerMenuKey", "P");

        PlayerPrefs.Save();
    }

    public static void RefreshControls()
    {
        try
        {
            DownKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("down", "DownArrow"));
            UpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("forward", "UpArrow"));
            LeftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("turnLeft", "LeftArrow"));
            RightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("turnRight", "RightArrow"));
            ShootKey2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shootKey", "Space"));
            ShootKey1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shootKey1", "Alpha1"));
            ShootKey2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shootKey2", "Alpha2"));
            ShootKey3 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shootKey3", "Alpha3"));
            ShootKey4 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shootKey4", "Alpha4"));
            ShootKey5 = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shootKey5", "Alpha5"));
            CruiseKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("cruiseKey", "C"));
            RespawnKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("respawnKey", "Space"));
            ChatKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("chatKey", "KeypadEnter"));
            ChatKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("chatKey2", "Return"));
            MapKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("mapKey", "M"));
            SettingsMenuKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("settingsMenuKey", "H"));
            PlayerMenuKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("playerMenuKey", "P"));
            InventoryKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inventoryKey", "I"));
            MouseAim = Boolean.Parse(PlayerPrefs.GetString("mouseAim", "False"));
            FireWithWeaponHotKeys = Boolean.Parse(PlayerPrefs.GetString("weaponHotkeys", "True"));

        }
        catch { }
    }
}
