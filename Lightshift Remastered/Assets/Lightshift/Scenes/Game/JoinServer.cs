using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinServer : MonoBehaviour {

    public string ipAddress;
    public string portNumber;
    public string natServerHost = string.Empty;
    public ushort natServerPort = 15941;

    public GameObject networkManager = null;
    private NetworkManager mgr = null;

    public bool useMainThreadManagerForRPCs = true;

    public bool getLocalNetworkConnections = false;

    public bool useTCP = false;

    private void Start()
    {
        ipAddress = "127.0.0.1";
        portNumber = "15937";

        if (!useTCP)
        {
            // Do any firewall opening requests on the operating system
            NetWorker.PingForFirewall(ushort.Parse(portNumber));
        }

        if (useMainThreadManagerForRPCs)
            Rpc.MainThreadRunner = MainThreadManager.Instance;

        if (getLocalNetworkConnections)
        {
            NetWorker.localServerLocated += LocalServerLocated;
            NetWorker.RefreshLocalUdpListings(ushort.Parse(portNumber));
        }

        //if (clientMode == ClientMode.Client)
        //    Connect();
        //else if (clientMode == ClientMode.Server)
        //    Host();

        if (Application.isEditor)
        {
            Host();
        } else Connect();
    }

    private void LocalServerLocated(NetWorker.BroadcastEndpoints endpoint, NetWorker sender)
    {
        Debug.Log("Found endpoint: " + endpoint.Address + ":" + endpoint.Port);
    }

    public void Connect()
    {
        ushort port;
        if (!ushort.TryParse(portNumber, out port))
        {
            Debug.LogError("The supplied port number is not within the allowed range 0-" + ushort.MaxValue);
            return;
        }

        NetWorker client;

        if (useTCP)
        {
            client = new TCPClient();
            ((TCPClient)client).Connect(ipAddress, (ushort)port);
        }
        else
        {
            client = new UDPClient();
            if (natServerHost.Trim().Length == 0)
                ((UDPClient)client).Connect(ipAddress, (ushort)port);
            else
                ((UDPClient)client).Connect(ipAddress, (ushort)port, natServerHost, natServerPort);
        }

        Connected(client);
    }

    public void Host()
    {
        NetWorker server;

        if (useTCP)
        {
            server = new TCPServer(64);
            ((TCPServer)server).Connect();
        }
        else
        {
            server = new UDPServer(64);

            if (natServerHost.Trim().Length == 0)
                ((UDPServer)server).Connect(ipAddress, ushort.Parse(portNumber));
            else
                ((UDPServer)server).Connect(natHost: natServerHost, natPort: natServerPort);
        }

        server.playerTimeout += (player, sender) =>
        {
            Debug.Log("Player " + player.NetworkId + " timed out");
        };

        Connected(server);


        NetworkManager.Instance.Initialize(server);
    }

    public void Connected(NetWorker networker)
    {
        if (!networker.IsBound)
        {
            Debug.LogError("NetWorker failed to bind");
            return;
        }

        if (mgr == null && networkManager == null)
        {
            Debug.LogWarning("A network manager was not provided, generating a new one instead");
            networkManager = new GameObject("Network Manager");
            mgr = networkManager.AddComponent<NetworkManager>();
        }
        else if (mgr == null)
            mgr = Instantiate(networkManager).GetComponent<NetworkManager>();

        if (networker is IServer)
        {
            //    SceneController.ChangeScene(Scenes.Game);
            //else
                NetworkObject.Flush(networker); //Called because we are already in the correct scene!
        }
    }
}
