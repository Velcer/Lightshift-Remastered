using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private void Start()
    {
        Destroy(gameObject, range);
    }

    public string teamId;
    public float range;
    public float damage;

    void OnCollisionEnter(Collision hit)
    {
        var entity = hit.gameObject.GetComponent<Entity>();
        if (entity == null)
            return;

        if (entity.Team == teamId)
            return;

        Destroy(this, range);
    }
}
