using PlayerIOClient;
using System.Collections;
using System.Collections.Generic;
public class AuthInstance : Instance {

    public override void Start()
    {
        con.Send("init");
        con.OnMessage += (object sender, Message m) =>
        {
            switch (m.Type)
            {
                case "connect":
                    {
                        string IPAddress = m.GetString(0);
                        int Port = m.GetInt(1);
                        string authToken = m.GetString(2);

                        PlayerIONetwork.net.clientAuthToken = authToken;
                    }
                    break;
            }
        };
    }
}
