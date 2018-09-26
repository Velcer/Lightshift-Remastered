using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public static InputManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }


    public int HorizontalModifier
    {
        get
        {
            int val = 0;
            if (Input.GetKey(Settings.LeftKey))
            {
                val = 1;
            }
            else if (Input.GetKey(Settings.RightKey))
            {
                val = -1;
            }
            else val = 0;

            return val;
        }
    }

    public int VerticalModifier
    {
        get
        {
            int val = 0;
            if (Input.GetKey(Settings.UpKey))
            {
                val = 1;
            }
            else if (Input.GetKey(Settings.DownKey))
            {
                val = -1;
            }
            else val = 0;

            return val;
        }
    }
}
