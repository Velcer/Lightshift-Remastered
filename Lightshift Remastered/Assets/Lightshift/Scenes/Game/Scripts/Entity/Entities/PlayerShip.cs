using BeardedManStudios.Forge.Networking.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : Entity {

    private Lifebar lifebar => Lifebar.Instance;

    public void Start()
    {
        shieldChanged += (value) => UpdateLifeBar();
        maxShieldChanged += (value) => UpdateLifeBar();
        healthChanged += (value) => UpdateLifeBar();
        maxHealthChanged += (value) => UpdateLifeBar();
        nameChanged += (value) => SetNameTag(value);

        networkReady += () => OnInit();
    }

    public override void OnInit()
    {
        UpdateLifeBar();

        if (networkObject.IsOwner)
        {
            /* Camera Follow */
            CameraFollow.Instance.Target = gameObject;
            SetHealthBar(0, 0);
            SetShieldBar(0, 0);
            SetNameTag("");
        }
        else SetNameTag(Name);
    }

    private void UpdateLifeBar()
    {
        if (networkObject.IsOwner)
            lifebar.SetLife(Health, MaxHealth, Shield, MaxShield);
        else
        {
            SetShieldBar(Shield, MaxShield);
            SetHealthBar(Health, MaxHealth);
        }
    }

    public override void Step()
    {
        if (Settings.KeysLocked)
            return;

        if (networkObject.IsOwner)
        {
            VerticalInput = InputManager.Instance.VerticalModifier;
            HorizontalInput = InputManager.Instance.HorizontalModifier;
        }
    }
}
