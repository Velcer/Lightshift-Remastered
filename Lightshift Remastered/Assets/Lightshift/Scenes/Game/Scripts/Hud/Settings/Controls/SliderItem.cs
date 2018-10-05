using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderItem : MonoBehaviour {

    public TextMeshProUGUI label;
    public TextMeshProUGUI sliderText;
    public Slider slider;
    public string saveCode;

    public void OnValueChanged()
    {
        sliderText.text = ((int)slider.value).ToString();
    }

    public string Result
    {
        get { return slider.value.ToString(); }
    }
}
