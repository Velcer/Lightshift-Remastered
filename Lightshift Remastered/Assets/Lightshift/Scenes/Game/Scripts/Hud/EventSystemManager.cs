using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour {

    public static EventSystemManager Instance;

    private EventSystem _eventSystem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        _eventSystem = GetComponent<EventSystem>();
    }

    public void SetSelectedObject(GameObject obj)
    {
        _eventSystem.SetSelectedGameObject(obj);
    }
}
