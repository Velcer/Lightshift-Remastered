using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour {

    [SerializeField]
    TextMeshPro nameText;

    [SerializeField]
    Slider shieldBar;

    [SerializeField]
    Slider healthBar;

    Quaternion rotation;

    private Vector3 Offset = new Vector3(0, 2, 0);

    void Awake()
    {
        rotation = transform.rotation;
    }

    public void SetUsername(string name)
    {
        nameText.text = name;
    }

    public void RefreshHealth(float health, float maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    public void RefreshShield(float shield, float maxShield)
    {
        shieldBar.maxValue = maxShield;
        shieldBar.value = shield;
    }

    void Update()
    {
        /* Rotation */
        transform.rotation = rotation;

        /* Offset */
        transform.position = transform.parent.transform.position;
        Vector3 _offset = Offset;
        transform.position += _offset;
    }
}
