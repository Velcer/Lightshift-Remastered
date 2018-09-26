using PlayerIOClient;
using PlayerIOConnect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIONetwork : MonoBehaviour {

    private Client client;
    private string gameId = "purple-pineapple-ckh49ythusbzsofd8xgyw";
    public string clientAuthToken { get; set; }

    public bool TestServer;
    public static PlayerIONetwork net;

    private Instance instance;

    private void Awake()
    {
        net = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RequestGameServer()
    {
        if (instance?.con != null && instance.con.Connected)
            instance.con.Disconnect();

        client.Multiplayer.JoinRoom("X", null, delegate (Connection con)
        {
            instance = new AuthInstance();
            instance.con = con;
            instance.client = client;
            instance.Start();
        });
    }

    public void Authenticate(string email, string password, Action<bool, string> callback)
    {
        PlayerIO.QuickConnect.SimpleConnect(gameId, email, password, null, delegate (Client client)
        {
            this.client = client;
            if (TestServer)
                client.Multiplayer.DevelopmentServer = new ServerEndpoint("127.0.0.1", 8184);

            RequestGameServer();

            callback.Invoke(true, null);

        }, delegate (PlayerIOError error)
        {
            callback.Invoke(false, error.Message);
        });
    }

    public void Register(string email, string password, Action<bool, string> callback)
    {
        PlayerIO.QuickConnect.SimpleRegister(gameId, Guid.NewGuid().ToString(), password, email, null, null, null, null, null, delegate (Client client)
        {
            if (TestServer)
                client.Multiplayer.DevelopmentServer = new ServerEndpoint("127.0.0.1", 8184);

            this.client = client;
            callback.Invoke(true, null);
        }, delegate (PlayerIORegistrationError error)
        {
            Debug.Log(error);

            callback.Invoke(false, error.EmailError);
        });
    }

    public void ForgotPassword(string email, Action callback)
    {
        PlayerIO.QuickConnect.SimpleRecoverPassword(gameId, email, delegate 
        {
            callback.Invoke();
        });
    }

    public void ConnectToMaster(string type, string authToken, string id)
    {
        Dictionary<string, string> auth = new Dictionary<string, string>();

        string date = DateTime.UtcNow.ToString();
        auth["auth"] = PlayerIOAuth.Create(id, authToken, date);
        auth["userId"] = id;

        PlayerIO.Authenticate(gameId, "server", auth, null, delegate (Client client)
        {
            this.client = client;

            if (TestServer)
                client.Multiplayer.DevelopmentServer = new ServerEndpoint("127.0.0.1", 8184);

            auth["type"] = type;
            auth["date"] = date;

            client.Multiplayer.CreateJoinRoom("X", "Master", true, null, auth, delegate (Connection con)
            {

                if (type == "gameserver")
                    instance = new ServerInstance();
                else instance = new SpawnerInstance();

                instance.con = con;
                instance.client = client;
                instance.Start();

            }, delegate (PlayerIOError e) 
            {
                Debug.Log(e);
            });

        }, delegate (PlayerIOError e) 
        {
            Debug.Log(e);
        });
    }
}
