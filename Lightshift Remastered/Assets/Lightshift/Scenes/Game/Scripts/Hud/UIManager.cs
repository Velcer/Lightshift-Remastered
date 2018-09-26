using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

    public Lifebar lifebar;

    void Awake()
    {

        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }
}
