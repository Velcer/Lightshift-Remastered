using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyItem : MonoBehaviour {

    public string saveCode;
    public string keyCode;
    public TextMeshProUGUI label;
    public Button btn;
    public TextMeshProUGUI btnText;

    private DateTime lastKeyAssign;

	void Update () {
        if (_keyCheck)
        {
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    var key = kcode.ToString();
                    keyCode = key;
                    btnText.text = key;
                    _keyCheck = false;
                    lastKeyAssign = DateTime.Now;
                }
            }
        }
	}

    bool _keyCheck;
    public void StartKeyCheck()
    {
        if ((DateTime.Now - lastKeyAssign).TotalMilliseconds > 500)
        {
            btnText.text = "<PRESS ANY KEY>";
            _keyCheck = true;
        }
    }
}
