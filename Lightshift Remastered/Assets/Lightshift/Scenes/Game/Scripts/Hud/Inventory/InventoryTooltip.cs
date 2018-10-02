using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryTooltip : MonoBehaviour {

    private TextMeshProUGUI displayText;
    private RectTransform rectTransform;
    private List<string> textList = new List<string>();
    private void Awake()
    {
        displayText = GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetTitle(string title)
    {
        textList.Clear();
        textList.Add($"<b><color=#FFD864>{title}</color></b>");
    }

    public void SetDescription(string description, bool isLore = false)
    {
        if (isLore)
            textList.Add($"<i><color=#CB9626>{description}</i></color>");
        else textList.Add(description);
    }

    public void Refresh()
    {
        string display = "";
        foreach (var s in textList)
        {
            display += $"{s}\n";
        }

        displayText.text = display;
        rectTransform.sizeDelta = new Vector2(250, (textList.Count * displayText.fontSize) + 4);
    }
}
