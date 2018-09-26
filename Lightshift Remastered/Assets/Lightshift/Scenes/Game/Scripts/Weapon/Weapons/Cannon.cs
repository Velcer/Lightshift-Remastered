using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Weapon {

    [SerializeField]
    GameObject bulletPrefab;

    private float _refire;

    public override void Fire()
    {
        if (_refire >= refire)
        {
            _refire = 0;
            Shoot();
        }
    }

    public override void OnUpdate()
    {
        _refire += Time.deltaTime;
    }

    private void Shoot()
    {
        var bullet = Instantiate(
            bulletPrefab, gunPoint.transform.position, gunPoint.transform.rotation);

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * force;

        var projectile = bullet.GetComponent<Projectile>();

        projectile.range = range;
        projectile.damage = damage;
        projectile.teamId = teamId;
    }
}
