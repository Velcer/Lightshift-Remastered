using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour {

    public string key;

    public bool flipped;

    public int Id;
    public Vector3 offset = new Vector3();

    public float agility { get; set; }
    public float acceleration { get; set; }
    public float speed { get; set; }
    public float health { get; set; }
    public float shield { get; set; }

    public ModuleBluePrint BluePrint()
    {
        return new ModuleBluePrint
        {
            health = health,
            shield = shield,
            agility = agility,
            acceleration = acceleration,
            flipped = flipped,
            Id = Id,
            key = key,
            speed = speed,
        };
    }

    public void Flip()
    {
        transform.localScale = new Vector3(-1, 1, 1);
    }
}
