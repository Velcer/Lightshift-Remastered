using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemStack : MonoBehaviour {

    public int id;

    public ItemMeta meta;

    private Vector3 Offset = new Vector3(20, -20, 0);

    public void DragItem()
    {
        transform.position = Input.mousePosition + Offset;
    }

    public void SetItemMeta(string key, ItemAttributeObject attributeObj = null)
    {
        if (meta != null)
            Destroy(meta);

        var obj = Instantiate(ItemManager.Instance.GetItem(key), transform);
        meta = obj.GetComponent<ItemMeta>();

        print(meta.displayName);

        var image = GetComponentInChildren<Image>();
        if (image == null)
            return;

        image.sprite = meta.icon;
    }

    private int _amount;
    public int Amount
    {
        get
        {
            return _amount;
        }
        set
        {
            var text = GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                if (value == 1)
                    text.text = "";
                else text.text = value.ToString();
            }
            _amount = value;
        }
    }
}
