using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity : EntityBehavior
{
    [Header("Network")]
    public float SendInterval = 0.1f; // 10 messages per second

    public Rigidbody _rigidbody;
    public SyncBuffer _syncBuffer;
    public Transform _transform;

    public float _timeSinceLastSync;
    public Vector3 _lastSentVelocity;
    public Vector3 _lastSentPosition;

    public Action<float> shieldChanged;
    public Action<float> healthChanged;
    public Action<float> maxHealthChanged;
    public Action<float> maxShieldChanged;
    public Action<string> teamChanged;
    public Action<string> nameChanged;
    public Action<float> speedChanged;
    public Action<float> accelerationChanged;
    public Action<float> agilityChanged;
    public Action<string> modelChanged;
    public Action<Module> moduleUpdated;

    public Action networkReady;

    private EntityUI entityUI { get; set; }

    protected override void NetworkStart()
    {
        base.NetworkStart();
        networkReady?.Invoke();
    }

    private int _oldHorizontalInput;
    private int _horizontalInput;
    public int HorizontalInput
    {
        get { return _horizontalInput; }
        set
        {
            _horizontalInput = value;

            if (networkObject.IsOwner && _oldHorizontalInput != value)
                networkObject.SendRpc(RPC_UPDATE_HORIZONTAL_INPUT, Receivers.Server, value);

            _oldHorizontalInput = value;
        }
    }

    public override void UpdateHorizontalInput(RpcArgs args)
    {
        if (!networkObject.IsOwner)
            HorizontalInput = args.GetNext<int>();
    }

    private int _oldVerticalInput;
    private int _verticalInput;
    public int VerticalInput
    {
        get { return _verticalInput; }
        set
        {
            _verticalInput = value;
            if (networkObject.IsOwner && _oldVerticalInput != value)
                networkObject.SendRpc(RPC_UPDATE_VERTICAL_INPUT, Receivers.Server, value);

            _oldVerticalInput = value;
        }
    }

    public override void UpdateVerticalInput(RpcArgs args)
    {
        if (!networkObject.IsOwner)
            VerticalInput = args.GetNext<int>();
    }

    public List<Module> modules = new List<Module>();
    public void SetModule(ModuleBluePrint m)
    {
        if (m == null)
            return;

        if (networkObject.IsServer)
            networkObject.SendRpc(RPC_UPDATE_MODULE, Receivers.All, m.Id, m.key, m.speed, m.acceleration, m.agility, m.health, m.shield, m.flipped);
    }

    public override void UpdateModule(RpcArgs args)
    {
        int id = args.GetNext<int>();
        string key = args.GetNext<string>();
        float speed = args.GetNext<float>();
        float acceleration = args.GetNext<float>();
        float agility = args.GetNext<float>();
        float health = args.GetNext<float>();
        float shield = args.GetNext<float>();
        bool flipped = args.GetNext<bool>();

        var module = modules.FirstOrDefault(m => m.Id == id);
        if (module != null)
        {
            modules.Remove(module);
            Destroy(module.gameObject);
        }

        var obj = ModuleManager.Instance.GetModule(key);
        obj = Instantiate(obj, transform);

        module = obj.GetComponent<Module>();

        module.key = key;
        module.Id = id;

        module.health = health;
        module.shield = shield;
        module.speed = speed;
        module.acceleration = acceleration;
        module.agility = agility;
        module.flipped = flipped;

        modules.Add(module);

        RefreshModules();
    }

    private void RefreshModules()
    {
        float health = 0;
        float shield = 0;
        float agility = 0;
        float acceleration = 0;
        float speed = 0;

        foreach (var mod in modules)
        {
            health += mod.health;
            shield += mod.shield;
            agility += mod.agility;
            acceleration += mod.acceleration;
            speed += mod.speed;

            var hullObj = modules.FirstOrDefault(m => m.GetType() == typeof(Hull));
            if (hullObj == null)
                continue;

            var hull = hullObj.GetComponent<Hull>();

            Vector3 newPos = new Vector3();

            if (mod.GetType() == typeof(Wing))
                newPos = hull.wingPosition[0];
            else if (mod.GetType() == typeof(Engine))
                newPos = hull.enginePosition[0];
            else if (mod.GetType() == typeof(Turret))
                newPos = hull.turretPosition[0];

            if (mod.flipped)
            {
                mod.Flip();
                mod.transform.localPosition = new Vector3(-newPos.x, newPos.y, newPos.z);
            }
            else mod.transform.localPosition = newPos;
        }

        if (networkObject.IsServer)
        {
            MaxHealth = health;
            MaxShield = shield;
            Agility = agility;
            Acceleration = acceleration;
            Speed = speed;
        }

        if (Health > MaxHealth)
            Health = MaxHealth;

        if (Shield > MaxShield)
            Shield = MaxShield;
    }

    private string _teamId;
    public string Team
    {
        get
        {
            return _teamId;
        }
        set
        {
            _teamId = value;

            if (networkObject.IsServer)
                networkObject.SendRpc(RPC_UPDATE_TEAM, Receivers.Others, value);
        }
    }

    public override void UpdateTeam(RpcArgs args)
    {
        if (!networkObject.IsServer)
            Team = args.GetNext<string>();
        teamChanged?.Invoke(Team);
    }

    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;

            if (networkObject.IsServer)
                networkObject.SendRpc(RPC_UPDATE_NAME, Receivers.Others, value);
        }
    }

    public override void UpdateName(RpcArgs args)
    {
        if (!networkObject.IsServer)
            Name = args.GetNext<string>();
        nameChanged?.Invoke(Name);
    }

    private float _speed;
    public float Speed
    {
        get
        {
            return _speed;
        }
        set
        {
            _speed = value;

            if (networkObject.IsServer)
                networkObject.SendRpc(RPC_UPDATE_SPEED, Receivers.Others, value);
        }
    }

    public override void UpdateSpeed(RpcArgs args)
    {
        if (!networkObject.IsServer)
            Speed = args.GetNext<float>();
        speedChanged?.Invoke(Speed);
    }

    private float _acceleration;
    public float Acceleration
    {
        get
        {
            return _acceleration;
        }
        set
        {
            if (networkObject.IsServer)
                networkObject.SendRpc(RPC_UPDATE_ACCELERATION, Receivers.Others, value);
            _acceleration = value;
        }
    }

    public override void UpdateAcceleration(RpcArgs args)
    {
        if (!networkObject.IsServer)
            Acceleration = args.GetNext<float>();
        accelerationChanged?.Invoke(Acceleration);
    }

    private float _agility;
    public float Agility
    {
        get
        {
            return _agility;
        }
        set
        {
            _agility = value;

            if (networkObject.IsServer)
                networkObject.SendRpc(RPC_UPDATE_AGILITY, Receivers.Others, value);
        }
    }

    public override void UpdateAgility(RpcArgs args)
    {
        if (!networkObject.IsServer)
            Agility = args.GetNext<float>();
        agilityChanged?.Invoke(Agility);
    }

    private float _health;
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;

            if (networkObject.IsServer)
                networkObject.SendRpc(RPC_UPDATE_HEALTH, Receivers.Others, value);
        }
    }

    public override void UpdateHealth(RpcArgs args)
    {
        if (!networkObject.IsServer)
            Health = args.GetNext<float>();
        healthChanged?.Invoke(Health);
    }

    private float _maxHealth;
    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;

            if (networkObject.IsServer)
                networkObject.SendRpc(RPC_UPDATE_MAX_HEALTH, Receivers.Others, value);
        }
    }

    public override void UpdateMaxHealth(RpcArgs args)
    {
        if (!networkObject.IsServer)
            MaxHealth = args.GetNext<float>();
        maxHealthChanged?.Invoke(MaxHealth);
    }

    private float _shield;
    public float Shield
    {
        get
        {
            return _shield;
        }
        set
        {
            _shield = value;

            if (networkObject.IsServer)
                networkObject.SendRpc(RPC_UPDATE_SHIELD, Receivers.Others, value);
        }
    }

    public override void UpdateShield(RpcArgs args)
    {
        if (!networkObject.IsServer)
            Shield = args.GetNext<float>();
        shieldChanged?.Invoke(Shield);
    }

    private float _maxShield;
    public float MaxShield
    {
        get
        {
            return _maxShield;
        }
        set
        {
            _maxShield = value;

            if (networkObject.IsServer)
                networkObject.SendRpc(RPC_UPDATE_MAX_SHIELD, Receivers.Others, value);
        }
    }

    public override void UpdateMaxShield(RpcArgs args)
    {
        if (!networkObject.IsServer)
            MaxShield = args.GetNext<float>();
        maxShieldChanged?.Invoke(MaxShield);
    }

    void Awake()
    {
        EntityManager.Instance.Entities.Add(this);
        _rigidbody = GetComponent<Rigidbody>();
        _syncBuffer = GetComponent<SyncBuffer>();
        _transform = transform;

        var obj = Instantiate(PrefabManager.Instance.entityUIPrefab, transform);
        entityUI = obj.GetComponent<EntityUI>();
    }

    public virtual void FixedStep() { }
    public virtual void Step() { }

    void Update()
    {
        Step();

        if (!networkObject.IsServer)
        {
            if (_syncBuffer.HasKeyframes)
            {
                _syncBuffer.UpdatePlayback(Time.deltaTime);
                _transform.position = _syncBuffer.Position;
                _transform.rotation = _syncBuffer.Rotation;
            }
        }

        if (networkObject.IsServer)
        {
            /* Handle Rotation */
            Quaternion rotation = _rigidbody.rotation * Quaternion.Euler(0, 0, HorizontalInput * Agility * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(rotation);

            /* Handle Movement */
            _rigidbody.AddForce((rotation * Vector3.up) * VerticalInput * 1000.0f * Acceleration * Time.fixedDeltaTime);

            if (_rigidbody.velocity.magnitude > Speed * 1000.0f)
                _rigidbody.velocity = _rigidbody.velocity.normalized * Speed * 1000.0f;
        }
    }

    void FixedUpdate()
    {
        FixedStep();
        if (networkObject.IsServer)
        {
            /* Add to buffer */
            _timeSinceLastSync += Time.deltaTime;

            if (_rigidbody.velocity != _lastSentVelocity || _rigidbody.position != _lastSentPosition)
            {
                if (Mathf.Approximately(_timeSinceLastSync, SendInterval) || _timeSinceLastSync > SendInterval)
                {
                    SendSyncMessage();
                }
            }
        }
    }

    public void SendSyncMessage()
    {
        networkObject.SendRpcUnreliable(RPC_SYNC, Receivers.Others, _timeSinceLastSync, _rigidbody.position, _rigidbody.rotation, _rigidbody.velocity);
        _lastSentVelocity = _rigidbody.velocity;
        _lastSentPosition = _rigidbody.position;
        _timeSinceLastSync = 0f;
    }

    public override void Sync(RpcArgs args)
    {
        _syncBuffer.AddKeyframe(args.GetNext<float>(), args.GetNext<Vector3>(), args.GetNext<Quaternion>(), args.GetNext<Vector3>());
    }

    public void RefreshEntity()
    {
        Health = Health;
        MaxHealth = MaxHealth;
        Shield = Shield;
        MaxShield = MaxShield;
        Speed = Speed;
        Acceleration = Acceleration;
        Agility = Agility;
        Team = Team;
        Name = Name;
        var list = new List<Module>();
        for (int i = 0; i < modules.Count; i++)
        {
            list.Add(modules[i]);
        }

        foreach (var m in list)
        {
            SetModule(m?.BluePrint());
        }
            
    }

    public void SetShieldBar(float shield, float max) => entityUI.RefreshShield(shield, max);
    public void SetHealthBar(float health, float max) => entityUI.RefreshHealth(health, max);
    public void SetNameTag(string name) => entityUI.SetUsername(name);

    public virtual void OnInit() { }
    public override void Init(RpcArgs args)
    {
        OnInit();
    }

    public void SendInitRPC() => networkObject.SendRpc(RPC_INIT, Receivers.Others);
}
