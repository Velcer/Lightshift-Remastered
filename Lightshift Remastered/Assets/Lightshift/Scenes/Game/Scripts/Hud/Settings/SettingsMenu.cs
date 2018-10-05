using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour {

    public List<KeyItem> keybindings = new List<KeyItem>();
    public List<ToggleItem> toggleOptions = new List<ToggleItem>();
    public List<SliderItem> sliderOptions = new List<SliderItem>();

    public GameObject contentPanel;
    public GameObject keyBindingPrefab;
    public GameObject toggleItemPrefab;
    public GameObject sliderItemPrefab;

    void Start()
    {
        Settings.KeysLocked = true;
        CreateSliderItem("Music Volume", "musicVolume");
        CreateSliderItem("Sound Volume", "soundEffectVolume");
        CreateToggleItem("Mouse Aim", "mouseAim");
        CreateKeyBinding("Turn Left", "turnLeft");
        CreateKeyBinding("Turn Right", "turnRight");
        CreateKeyBinding("Forward", "forward");
        CreateKeyBinding("Down", "down");
        CreateKeyBinding("Toggle Inventory", "inventoryKey");
        CreateKeyBinding("Cruise", "cruiseKey");
        CreateKeyBinding("System Map", "mapKey");
        CreateKeyBinding("Chat", "chatKey");
        CreateKeyBinding("Chat 2", "chatKey2");
        CreateToggleItem("Fire With Weapon Hotkeys", "weaponHotkeys");
        CreateKeyBinding("Weapon 1", "shootKey1");
        CreateKeyBinding("Weapon 2", "shootKey2");
        CreateKeyBinding("Weapon 3", "shootKey3");
        CreateKeyBinding("Weapon 4", "shootKey4");
        CreateKeyBinding("Weapon 5", "shootKey5");
    }

    public void CreateToggleItem(string desc, string saveCode)
    {
        var item = Instantiate(toggleItemPrefab, contentPanel.transform);
        var script = item.GetComponent<ToggleItem>();
        script.label.text = desc;
        script.saveCode = saveCode;
        var val = Boolean.Parse(PlayerPrefs.GetString(saveCode, "False"));
        script.toggle.isOn = val;

        toggleOptions.Add(script);
    }

    public void CreateSliderItem(string desc, string saveCode)
    {
        var item = Instantiate(sliderItemPrefab, contentPanel.transform);
        var script = item.GetComponent<SliderItem>();
        script.label.text = desc;
        script.saveCode = saveCode;
        var val = PlayerPrefs.GetInt(saveCode, 100);
        script.slider.value = val;

        sliderOptions.Add(script);
    }

    public void CreateKeyBinding(string desc, string saveCode)
    {
        var item = Instantiate(keyBindingPrefab, contentPanel.transform);
        var script = item.GetComponent<KeyItem>();
        script.label.text = desc;
        script.saveCode = saveCode;
        var btnText = PlayerPrefs.GetString(saveCode, "---");
        script.btnText.text = btnText;
        script.keyCode = btnText;

        keybindings.Add(script);
    }

    public void Save()
    {
        foreach (var item in keybindings)
            PlayerPrefs.SetString(item.saveCode, item.keyCode);

        foreach (var item in toggleOptions)
            PlayerPrefs.SetString(item.saveCode, item.Result);

        foreach (var item in sliderOptions)
            PlayerPrefs.SetString(item.saveCode, item.Result);

        PlayerPrefs.Save();

        Settings.RefreshControls();
        Settings.KeysLocked = false;
        Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(Settings.SettingsMenuKey))
            Exit();
    }

    public void Exit()
    {
        Settings.KeysLocked = false;
        Destroy(this.gameObject);
    }
}
