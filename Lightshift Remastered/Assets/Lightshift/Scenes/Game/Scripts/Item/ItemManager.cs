using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    public static ItemManager Instance;

    [SerializeField]
    List<GameObject> _items = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != null)
            Destroy(gameObject);
    }

    private void Start()
    {
        var objects = Resources.LoadAll("Prefabs/Item/Module", typeof(GameObject));
        foreach (GameObject x in objects)
            _items.Add(x);

        objects = Resources.LoadAll("Prefabs/Item/Equip", typeof(GameObject));
        foreach (GameObject x in objects)
            _items.Add(x);

        objects = Resources.LoadAll("Prefabs/Item/Material", typeof(GameObject));
        foreach (GameObject x in objects)
            _items.Add(x);

        objects = Resources.LoadAll("Prefabs/Item/Weapon", typeof(GameObject));
        foreach (GameObject x in objects)
            _items.Add(x);

        print(_items.Count);
    }

    public GameObject GetItem(string id)
    {
        return _items.FirstOrDefault(w => w.GetComponent<ItemMeta>().key == id);
    }
}
