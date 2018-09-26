using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseAI : MonoBehaviour {

    [Header("Target Range")]
    public float AgroRange = 100;
    public Entity ship { get; set; }
    private Vector2 input;

    [Header("AI Behavior Tags")]
    public string[] TargetTag = new string[1] { "Friendly" };
    public string[] IgnoreTag = new string[1] { "Enemy" };

    public void FindNearestEnemy()
    {

    }
}
