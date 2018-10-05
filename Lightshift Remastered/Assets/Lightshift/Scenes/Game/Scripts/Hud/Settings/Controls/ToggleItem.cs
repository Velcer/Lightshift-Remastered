using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleItem : MonoBehaviour {

    public TextMeshProUGUI label;
    public Toggle toggle;
    public string saveCode;

    public string Result
    {
        get { return toggle.isOn.ToString(); }
    }
}
