using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    private string localConnectionId { get; set; }

    public List<Player> Players = new List<Player>();

    public static PlayerManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public Player GetLocalPlayer()
    {
        return Players.FirstOrDefault(p => p.connectUserId == localConnectionId);
    }

    public Player GetPlayer(string key)
    {
        return Players.FirstOrDefault(p => p.connectUserId == key);
    }

    public void CreatePlayer(Player player)
    {
        Debug.Log($"Player connected: [{player.networkObject.Owner.NetworkId}]");

        foreach (Entity e in EntityManager.Instance.Entities)
            e.RefreshEntity();

        Players.Add(player);
    }

    public void RemovePlayer(NetworkingPlayer connection)
    {
        var player = Players.FirstOrDefault(p => p.networkObject.Owner.NetworkId == connection.NetworkId);

        if (player != null && Players.Contains(player))
        {
            Debug.Log($"Player disconnected: [{connection?.NetworkId}]");
            player?.ship?.networkObject?.Destroy();
            player?.networkObject?.Destroy();

            Players.Remove(player);
        }
    }

    public void DisconnectPlayer(NetworkingPlayer connection)
    {
        ((IServer)NetworkManager.Instance.Networker).Disconnect(connection, true);
        ((IServer)NetworkManager.Instance.Networker).CommitDisconnects();
    }
}
