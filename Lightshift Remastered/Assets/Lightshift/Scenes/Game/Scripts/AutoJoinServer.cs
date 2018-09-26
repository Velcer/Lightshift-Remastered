using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoJoinServer : MonoBehaviour {

    [SerializeField]
    private GameObject networkManagerPrefab;

    [SerializeField]
    private GameObject UICanvas;

    [SerializeField]
    private string ipToJoin = "127.0.0.1";

    [SerializeField]
    private ushort port = 15739;

    private NetworkManager networkManager;

    private NetWorker networker;

    public static AutoJoinServer Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(this);
    }


    private void Start()
    {
        Rpc.MainThreadRunner = MainThreadManager.Instance;
    }
    public void HostGame()
    {

        networker = new UDPServer(64);
        ((UDPServer)networker).Connect(ipToJoin, port);
        InitializeNetworker(networker);

    }

    public void JoinGame()
    {

        networker = new UDPClient();
        ((UDPClient)networker).Connect(ipToJoin, port);
        InitializeNetworker(networker);
        networkManager.InstantiatePlayer();

        //SceneManager.sceneLoaded += InstantiatePlayer;
        //SceneManager.LoadScene("_GAME_");
    }
    private void InstantiatePlayer(Scene scene,LoadSceneMode loadSceneMode )
    {
        SceneManager.sceneLoaded -= InstantiatePlayer;

    }

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
        if (networker is IServer)
        {
            NetworkObject.Flush(networker);
        }
        UICanvas.SetActive(false);

    }

}
