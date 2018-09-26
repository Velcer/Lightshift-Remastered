using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SpawnerManager : MonoBehaviour {

    public static SpawnerManager Instance;

    [Header("Spawner Settings")]
    public string DefaultMachineIp = "127.0.0.1";
    public string DefaultExePath = "";

    public const string authKey = "plQFdKgzOb";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Start()
    {
        PlayerIONetwork.net.ConnectToMaster("spawner", authKey, Guid.NewGuid().ToString());
    }

    public void CreateServer(string serverId)
    {
        var process = new Process
        {
            StartInfo =
              {
                  FileName = DefaultExePath,
                  Arguments = $"{serverId}-{authKey}"
              }
        };
        process.Start();
    }
}
