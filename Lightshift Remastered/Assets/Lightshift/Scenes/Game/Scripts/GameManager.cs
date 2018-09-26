using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The singleton Game object which oversees the whole game state
/// </summary>
/// 

public class GameManager : GameManagerBehavior
{
    PlayerManager PlayerManager => PlayerManager.Instance;

    public static GameManager Instance;

    public Player localClient { get; set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        sceneReady = true;
    }

    public bool sceneReady { get; set; }
    protected override void NetworkStart()
    {
        base.NetworkStart();

        if (!networkObject.IsServer)
            return;
        //Debug.Log("NetworkStart");
        ///* Handle Connection */
        //NetworkManager.Instance.Networker.playerAccepted += (player, sender) => MainThreadManager.Run(() => 
        //{
        //    Debug.Log("Creating a player ship for the newly connected client");
        //    //PlayerManager.CreatePlayer(player)
        //    PlayerShip ship = NetworkManager.Instance.InstantiateEntity() as PlayerShip;

        //    ship.networkReady += () =>
        //    {
        //        ship.Health = 500;
        //        ship.MaxHealth = 750;
        //        ship.Shield = 350;
        //        ship.MaxShield = 350;
        //        ship.Model = "BwsNyV0rfq";

        //        ship.Acceleration = 0.5f;
        //        ship.Speed = 0.2f;
        //        ship.Agility = 180;

        //        ship.networkObject.AssignOwnership(player);
        //    };
        //});

        /* Handle Disconnect */
        NetworkManager.Instance.Networker.playerDisconnected += (player, sender) => MainThreadManager.Run(() => PlayerManager.RemovePlayer(player));      
       
       
    }
}