using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelcerMainMenu : MonoBehaviour
{
    //the network manager prefab, provided by Forge
    [SerializeField]
    private GameObject networkManagerPrefab;

    //the main menu canvas that should be disabled
    [SerializeField]
    private GameObject mainMenuCanvas;

    //what ip the join button should use
    [SerializeField]
    private string ipToJoin = "127.0.0.1";

    //what port the connection will be hosted & clients will connect too
    [SerializeField]
    private ushort port = 15739;

    //the network manager
    private NetworkManager networkManager;

    //the local user's networker
    private NetWorker networker;

    /// <summary>
    /// Hosts a game
    /// </summary>
    public void HostGame()
    {
        networker = new UDPServer(64);
        ((UDPServer)networker).Connect(ipToJoin, port);
        InitializeNetworker(networker);
    }

    /// <summary>
    /// Joins a game
    /// </summary>
    public void JoinGame()
    {
        networker = new UDPClient();
        ((UDPClient)networker).Connect(ipToJoin, port);
        InitializeNetworker(networker);
    }

    /// <summary>
    /// Initializes the network manager, ie the main networker for the user
    /// </summary>
    /// <param name="networker">the main network communicator</param>
    private void InitializeNetworker(NetWorker networker)
    {
        if (!networker.IsBound)
        {
            Debug.LogError("NetWorker failed to bind");
            return;
        }

        if (networkManager == null && networkManagerPrefab == null)
        {
            Debug.LogWarning("A network manager was not provided, generating a new one instead");
            networkManagerPrefab = new GameObject("Network Manager");
            networkManager = networkManagerPrefab.AddComponent<NetworkManager>();
        }
        else if (networkManager == null)
        {
            networkManager = Instantiate(networkManagerPrefab).GetComponent<NetworkManager>();
        }

        networkManager.Initialize(networker);

        //hide the canvas
        mainMenuCanvas.SetActive(false);

        if (networker is IServer)
        {
            NetworkObject.Flush(networker); 
        }
    }

}
