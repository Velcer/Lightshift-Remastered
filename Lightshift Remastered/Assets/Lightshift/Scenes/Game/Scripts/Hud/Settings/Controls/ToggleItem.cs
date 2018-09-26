using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleItem : MonoBehaviour {

    public Text label;
    public Toggle toggle;
    public string saveCode;
    public string result
    {
        get { return toggle.isOn.ToString(); }
    }
}
