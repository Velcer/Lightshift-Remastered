using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public int id;
    public float refire;
    public float force;
    public float range;
    public float damage;
    public string teamId;

    public GameObject gunPoint;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
            Fire();


            MouseAim();

        OnUpdate();
    }

    private void MouseAim()
    {
        var mouse_pos = Input.mousePosition;
        mouse_pos.z = -20;
        var object_pos = Camera.main.WorldToScreenPoint(transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        var angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + -90);
    }

    public virtual void OnUpdate() { }

    public virtual void Fire() { }
}
