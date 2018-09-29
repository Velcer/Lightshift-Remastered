using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemStack : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public int id;
    public string displayName;
    public int amount;

    [SerializeField]
    Image displayImage;

    [SerializeField]
    TextMeshProUGUI displayCount;

    [SerializeField]
    GameObject toolTipPrefab;
    InventoryTooltip toolTip;


    private void ShowToolTip()
    {
        if (toolTip == null)
        {
            toolTip = Instantiate(toolTipPrefab, transform).GetComponent<InventoryTooltip>();
            toolTip.title.text = displayName;
            toolTip.description.text = "Teessttt";
        }
        else toolTip.gameObject.SetActive(true);       
    }

    private void HideToolTip() => toolTip?.gameObject.SetActive(false);

    public void OnPointerEnter(PointerEventData eventData) => ShowToolTip();

    public void OnPointerExit(PointerEventData eventData) => HideToolTip();
}
