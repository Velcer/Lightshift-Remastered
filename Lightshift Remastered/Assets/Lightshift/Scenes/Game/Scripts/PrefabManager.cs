using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefabManager : MonoBehaviour {

    public static PrefabManager Instance;

    [Header("Entity World UI Object")]
    public GameObject entityUIPrefab;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != null)
            Destroy(gameObject);
    }


}
