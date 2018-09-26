using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChatSmiley : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI _smiley;

    private EventTrigger _trigger;

    private void Awake()
    {
        _trigger = gameObject.AddComponent<EventTrigger>();
    }

    bool left = true;
    private void Start()
    {
        EventTrigger.Entry onEnter = new EventTrigger.Entry();
        onEnter.eventID = EventTriggerType.PointerEnter;
        onEnter.callback.AddListener((sender) =>
        {
            if (!left)
                return;
            _smiley.fontSize += 2;
            _smiley.transform.eulerAngles = new Vector3(0, 0, _smiley.transform.eulerAngles.z -10);

            left = false;
        });

        EventTrigger.Entry onLeave = new EventTrigger.Entry();
        onLeave.eventID = EventTriggerType.PointerExit;
        onLeave.callback.AddListener((sender) =>
        {
            left = true;
            _smiley.fontSize -= 2;
            _smiley.transform.eulerAngles = new Vector3(0, 0, _smiley.transform.eulerAngles.z + 10);
        });

        _trigger.triggers.Add(onEnter);
        _trigger.triggers.Add(onLeave);
    }

    public void AddToChat()
    {
        ChatBox.Instance.AddSmiley(_smiley.text);
    }
}
