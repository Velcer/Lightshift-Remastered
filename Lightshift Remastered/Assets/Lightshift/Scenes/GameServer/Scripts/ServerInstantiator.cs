using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ServerInstantiator : MonoBehaviour {

    public Text titleText;
    // Use this for initialization
    void Start() {
        string args = "";
        if (Environment.GetCommandLineArgs().Length > 1)
        {
            args = Environment.GetCommandLineArgs()[1];
            var data = args.Split('-');

            var serverId = data[0];
            var authToken = data[1];

            PlayerIONetwork.net.ConnectToMaster("gameserver", authToken, serverId);

            titleText.text = $"GameServer \n#{serverId}";
        }
        else Application.Quit();
	}
}
