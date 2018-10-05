using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    [SerializeField]
    GameObject settingsMenuPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    GameObject settingsMenu { get; set; }
    public void ToggleSettingsMenu()
    {
        if (settingsMenu == null)
            settingsMenu = Instantiate(settingsMenuPrefab);
        else Destroy(settingsMenu);
    }

    void Update()
    {
        if (!Settings.KeysLocked)
        {
            if (Input.GetKeyDown(Settings.SettingsMenuKey))
                ToggleSettingsMenu();
        }
    }
}
