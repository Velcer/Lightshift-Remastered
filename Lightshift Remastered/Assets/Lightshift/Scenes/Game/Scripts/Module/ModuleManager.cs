using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleManager : MonoBehaviour {

    public static ModuleManager Instance;

    private List<GameObject> _modules = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        var objects = Resources.LoadAll("Prefabs/Module/Engine", typeof(GameObject));
        foreach (GameObject x in objects)
            _modules.Add(x);

        objects = Resources.LoadAll("Prefabs/Module/Hull", typeof(GameObject));
        foreach (GameObject x in objects)
            _modules.Add(x);

        objects = Resources.LoadAll("Prefabs/Module/Wing", typeof(GameObject));
        foreach (GameObject x in objects)
            _modules.Add(x);

        objects = Resources.LoadAll("Prefabs/Module/Turret", typeof(GameObject));
        foreach (GameObject x in objects)
            _modules.Add(x);
    }

    public GameObject GetModule(string id)
    {
        return _modules.FirstOrDefault(w => w.GetComponent<Module>().key == id);
    }
}
