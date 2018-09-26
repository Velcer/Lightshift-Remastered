using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnerInstance : Instance
{
    public override void Start()
    {
        con.Send("init");
        con.OnMessage += (object sender, Message m) =>
        {
            Debug.Log(m);
            switch (m.Type)
            {
                case "createServer":
                    {
                        string serverId = m.GetString(0);
                        SpawnerManager.Instance.CreateServer(serverId);
                        Send("serverStarting", serverId);
                    }
                    break;
            }
        };
    }
}