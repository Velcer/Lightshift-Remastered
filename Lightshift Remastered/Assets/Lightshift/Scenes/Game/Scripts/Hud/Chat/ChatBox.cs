using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ChatBox : MonoBehaviour
{

    public static ChatBox Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    List<TextMeshProUGUI> _messages = new List<TextMeshProUGUI>();
    
    [SerializeField]
    private GameObject ChatInputPanel;
    [SerializeField]
    private TMP_InputField ChatInputField;
    [SerializeField]
    private GameObject Chatbox;
    [SerializeField]
    private TextMeshProUGUI MessagePrefab;
    [SerializeField]
    private ScrollRect scroll;

    private void Start()
    {
        ChatInputPanel.SetActive(false);
        ChatInputField.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(Settings.ChatKey) || Input.GetKeyDown(Settings.ChatKey2))
        {
            if (ChatInputPanel.activeInHierarchy)
            {
                Settings.KeysLocked = false;

                var msg = ChatInputField.text;
                if (msg != "")
                    SendChatMessage(msg);
                ChatInputPanel.SetActive(false);
                ChatInputField.text = "";
            }
            else
            {
                Settings.KeysLocked = true;
                ChatInputPanel.SetActive(true);
                ChatInputField.ActivateInputField();
            }
        }
    }

    public void SendChatMessage(string m)
    {
        GameManager.Instance.localClient.SendChatMessage(m);
    }

    void LateUpdate()
    {
        scroll.verticalNormalizedPosition = 0;
    }

    public void AddMessage(string msg)
    {
        /* Add Chat Message */
        var m = Instantiate(MessagePrefab, Chatbox.transform);
        m.text = msg;

        /* Check Chat Length */
        _messages.Add(m);
        if (_messages.Count > 50)
        {
            var e = _messages[0];
            _messages.Remove(e);
            Destroy(e.gameObject);
        }

    }

    public void AddSmiley(string smiley)
    {
        ChatInputField.text += $"<size=24>{smiley}</size>";
        EventSystemManager.Instance.SetSelectedObject(ChatInputField.gameObject);
    }
}
