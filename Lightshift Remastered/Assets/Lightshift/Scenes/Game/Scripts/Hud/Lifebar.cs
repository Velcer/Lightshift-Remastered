using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Lifebar : MonoBehaviour {

    public static Lifebar Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    [SerializeField]
    TextMeshProUGUI HealthLabel;
    [SerializeField]
    TextMeshProUGUI ShieldLabel;
    [SerializeField]
    Slider HealthBar;
    [SerializeField]
    Slider ShieldBar;
    [SerializeField]
    GameObject Lifebox;

    private float _health;
    private float _maxHealth;
    private float _shield;
    private float _maxShield;

    public void SetLife(float health, float maxhealth, float shield, float maxshield)
    {
        if (_disabled)
            return;

        _health = health;
        _maxHealth = maxhealth;
        _shield = shield;
        _maxShield = maxshield;
        RefreshHealth();
        RefreshShield();
    }

    private void RefreshHealth()
    {
        var value = (_health / _maxHealth) * 1.0f;
        HealthBar.value = value;
        HealthLabel.text = $"{Mathf.Round(_health)} | {_maxHealth}";
    }

    private void RefreshShield()
    {
        var value = (_shield / _maxShield) * 1.0f;
        ShieldBar.value = value;
        ShieldLabel.text = $"{Mathf.Round(_shield)} | {_maxShield}";
    }


    private bool _disabled { get; set; }

    public void Disable()
    {
        _disabled = true;
        Lifebox.SetActive(false);
    }
    public void Enable() 
    {
        _disabled = false;
        Lifebox.SetActive(true);
    }
}
