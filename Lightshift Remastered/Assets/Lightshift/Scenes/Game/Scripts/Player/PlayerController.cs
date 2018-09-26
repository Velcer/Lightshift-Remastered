using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public static PlayerController instance { get; set; }

    private void Awake()
    {
        instance = this;
    }

    public bool IsActive
    {
        get { return false; }
        //get
        //{
        //    if (game.player.Ship == null)
        //        return false;
        //    else return true;
        //}
    }

    void Start() => lastTurretUpdate = DateTime.Now;

    public void Update()
    {

        /* Check if Keys are disabled */
        if (Settings.KeysLocked || !IsActive)
            return;

        Debug.Log("here");

        /* Handle Mouse Aim */
        var currentAngle = 14;/* game.player.gunPoint.rotation - 45;*/
        var targetAngle = 45 + Mathf.Atan2(Input.mousePosition.y - Screen.height * 0.5f, Input.mousePosition.x - Screen.width * 0.5f) * 57.29578f;
        var angleDiff = currentAngle - targetAngle;

        if (angleDiff > 180)
            angleDiff -= 360;
        else if (angleDiff < -180)
            angleDiff += 360;
        if (angleDiff > 5)
            MouseInput = -1;
        else if (angleDiff < -5)
            MouseInput = 1;
        else
            MouseInput = 0;


        Debug.Log("here2");

        bool shooting = false;
        /* Check shoot input */
        if (Input.GetKey(Settings.ShootKey1))
        {
            selectedWeapon = 1;
            if (Settings.FireWithWeaponHotKeys)
                shooting = true;
        }
        if (Input.GetKey(Settings.ShootKey2))
        {
            selectedWeapon = 2;
            if (Settings.FireWithWeaponHotKeys)
                shooting = true;
        }
        if (Input.GetKey(Settings.ShootKey3))
        {
            selectedWeapon = 3;
            if (Settings.FireWithWeaponHotKeys)
                shooting = true;
        }
        if (Input.GetKey(Settings.ShootKey4))
        {
            selectedWeapon = 4;
            if (Settings.FireWithWeaponHotKeys)
                shooting = true;
        }
        if (Input.GetKey(Settings.ShootKey5))
        {
            selectedWeapon = 5;
            if (Settings.FireWithWeaponHotKeys)
                shooting = true;
        }

        if (Input.GetKey(Settings.ShootKey))
        {
            shooting = true;
        }

        if (shooting)
            ShootingInput = selectedWeapon;
        if (!shooting)
            ShootingInput = 0;


        /* Check horizontal input */
        if (Input.GetKey(Settings.LeftKey))
        {
            HorizontalInput = 1;
        }
        else if (Input.GetKey(Settings.RightKey))
        {
            HorizontalInput = -1;
        }
        else HorizontalInput = 0;

        /* Check vertical input */
        if (Input.GetKey(Settings.CruiseKey))
            VerticalInput = 2;
        else if (Input.GetKey(Settings.UpKey))
            VerticalInput = 1;
        else if (Input.GetKey(Settings.DownKey))
            VerticalInput = -1;
        else VerticalInput = 0;
    }



    float _verticalInput;
    public float VerticalInput
    {
        get { return _verticalInput; }
        set
        {
            if (value != _verticalInput)
            {
                _verticalInput = value;
            }
        }
    }

    float _horizontalInput;
    public float HorizontalInput
    {
        get { return _horizontalInput; }
        set
        {
            if (value != _horizontalInput)
            {
                _horizontalInput = value;
            }
        }
    }

    float selectedWeapon = 0;
    float _shooting;
    public float ShootingInput
    {
        get { return _shooting; }
        set
        {
            if (value != _shooting)
            {
                _shooting = value;
            }
        }
    }

    DateTime lastTurretUpdate;
    float _mouseInput;
    public float MouseInput
    {
        get { return _mouseInput; }
        set
        {
            if (value != _mouseInput && (DateTime.Now - lastTurretUpdate).TotalMilliseconds > 10)
            {
                lastTurretUpdate = DateTime.Now;
                _mouseInput = value;
            }
        }
    }
}
