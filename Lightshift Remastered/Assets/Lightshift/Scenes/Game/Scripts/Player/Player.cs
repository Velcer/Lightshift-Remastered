using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class Player : PlayerBehavior
{
    public string connectUserId { get; set; }
    public string Username { get; set; }
    public string authToken { get; set; }
    public PlayerShip ship { get; set; }
    public Inventory inventory { get; set; }

    protected override void NetworkStart()
    {
        Username = $"guest-{Random.Range(1000, 9999)}";

        base.NetworkStart();

        if (networkObject.IsOwner)
        {
            networkObject.SendRpc(RPC_INIT, Receivers.Server);
            GameManager.Instance.localClient = this;
        }
    }

    public void Disconnect()
    {
        ((IServer)NetworkManager.Instance.Networker).Disconnect(networkObject.Owner, true);
        ((IServer)NetworkManager.Instance.Networker).CommitDisconnects();
        networkObject.Destroy();
    }

    public override void Init(RpcArgs args)
    {
        //Add player to list
        PlayerManager.Instance.CreatePlayer(this);

        //Create inventory
        CreateInventory();

        //Spawn ship
        Spawn();
    }

    private void CreateInventory()
    {
        inventory = NetworkManager.Instance.InstantiateInventory() as Inventory;
        inventory.networkObject.AssignOwnership(networkObject.Owner);

        inventory.networkObject.onReady += (thing) => 
        {
            //Add items here
        };
    }

    public override void Respawn(RpcArgs args) => Spawn();   

    public void Spawn()
    {
        if (ship != null)
            ship.networkObject.Destroy();

        ship = NetworkManager.Instance.InstantiateEntity() as PlayerShip;

        ship.networkObject.ownershipChanged += (thing) => ship.SendInitRPC();

        ship.networkReady += () =>
        {
            ModuleBluePrint hull = new ModuleBluePrint
            {
                Id = 0,
                key = "defaultHull",
                flipped = false,
                health = 850,
                shield = 250,
            };
            ship.SetModule(hull);

            ModuleBluePrint rightWing = new ModuleBluePrint
            {
                Id = 1,
                key = "defaultWing",
                flipped = true,
                agility = 2,
                shield = 250,
                health = 50,
            };
            ship.SetModule(rightWing);

            ModuleBluePrint leftWing = new ModuleBluePrint
            {
                Id = 2,
                key = "defaultWing",
                agility = 4,
                shield = 250,
                health = 50,
            };
            ship.SetModule(leftWing);

            ModuleBluePrint engine = new ModuleBluePrint
            {
                Id = 3,
                key = "carbideEngine",
                shield = 100,
                health = 50,
                acceleration = 0.055f,
                speed = 0.01f,
            };
            ship.SetModule(engine);

            ModuleBluePrint turret = new ModuleBluePrint
            {
                Id = 4,
                key = "defaultTurret",
                shield = 100,
                health = 50,
                acceleration = 0.055f,
                speed = 0.01f,
            };
            ship.SetModule(turret);

            ship.Team = Username;
            ship.Name = Username;
            ship.Health = ship.MaxHealth;
            ship.Shield = ship.MaxShield;

            ship.networkObject.AssignOwnership(networkObject.Owner);
        };
    }

    public void SendChatMessage(string message)
    {
        if (networkObject.IsOwner)
            networkObject.SendRpc(RPC_CHAT, Receivers.Server, message);
    }

    public override void Chat(RpcArgs args)
    {
        if (networkObject.IsServer)
        {
            string message = args.GetNext<string>();

            if (message.Length > 250)
                message.Substring(message.Length - 250);

            string format = $"<size=16>{Username}</size>: {message}";
            ChatBox.Instance.AddMessage(format);

            networkObject.SendRpc(RPC_CHAT, Receivers.Others, format);
        }
        else
        {
            string message = args.GetNext<string>();
            ChatBox.Instance.AddMessage(message);
        }
    }
}
