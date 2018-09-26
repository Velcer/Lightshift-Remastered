using PlayerIOClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInstance : Instance
{
    public override void Start()
    {
        con.Send("init");
        con.OnMessage += (object sender, Message m) =>
        {
            switch (m.Type)
            {
                case "userVerified":
                    {
                        string userId = m.GetString(0);
                    }
                    break;
                case "invalidAuth":
                    {
                        string userId = m.GetString(0);
                    }
                    break;
            }
        };
    }
}
