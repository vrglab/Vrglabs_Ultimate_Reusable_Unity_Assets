using Ink.Parsed;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PersistantSingleton<PlayerController>, IWeaponTarget
{
    public static readonly string creature_id = "player";




    /* PRIVATE VARIABLES */
    private PlayerMovementModule _playerMovementModule;
    private PlayerInventoryModule _playerInventoryModule;
    private PlayerWeaponModule _playerWeaponModule;
    private PlayerLifeModule _playerLifeModule;

    private Components _componentsWrapper;
    private Functions _functions;
    private DataHolder _coreDataHolder, _pos = new DataHolder();

    public void Awake()
    {
        base.Awake();
        _coreDataHolder = LoadOrCreatePlayerData();

        _componentsWrapper = new Components();
        _componentsWrapper["transform"] = this.transform;
        _componentsWrapper["rigidbody"] = GetComponent<Rigidbody2D>();

        _functions = new Functions();


        _playerMovementModule = new PlayerMovementModule(
            new PlayerMovementModule.StaticModifiers(9f, 14, 15f, 11f, 1.1f, 14f, 25f, 0.2f),
            new PlayerMovementModule.ConditionModifiers(0.1f, 0.2f, 0.2f, 0.3f)
            );
        _playerInventoryModule = new PlayerInventoryModule(new PlayerInventoryModule.StaticModifiers(20, 6));
        _playerWeaponModule = new PlayerWeaponModule();
        _playerLifeModule = new PlayerLifeModule(new PlayerLifeModule.StaticModifiers(20));  
    }

    public void Start()
    {
        _coreDataHolder = _playerMovementModule.InitModule(_coreDataHolder, _componentsWrapper, _functions);
        _coreDataHolder = _playerInventoryModule.InitModule(_coreDataHolder, _componentsWrapper, _functions);
        _coreDataHolder = _playerWeaponModule.InitModule(_coreDataHolder, _componentsWrapper, _functions);
        _coreDataHolder = _playerLifeModule.InitModule(_coreDataHolder, _componentsWrapper, _functions);
    }

    public void Update()
    {
        _coreDataHolder = _playerMovementModule.UpdateModule(_coreDataHolder, _componentsWrapper, _functions);
        _coreDataHolder = _playerInventoryModule.UpdateModule(_coreDataHolder, _componentsWrapper, _functions);
        _coreDataHolder = _playerWeaponModule.UpdateModule(_coreDataHolder, _componentsWrapper, _functions);
        _coreDataHolder = _playerLifeModule.UpdateModule(_coreDataHolder, _componentsWrapper, _functions);

        _pos.SerializeVector3(transform.position);

        _coreDataHolder.SetData(DataIds.getCreaturePositionKey(creature_id), _pos);

        GameSerializer.Instance.CurrentlyActiveSlot.saved_data[DataIds.getCreatureCoreDataKey(creature_id)] = _coreDataHolder;
    }

    public void FixedUpdate()
    {
        _coreDataHolder = _playerMovementModule.UpdatePhysicsModule(_coreDataHolder, _componentsWrapper, _functions);
        _coreDataHolder = _playerInventoryModule.UpdatePhysicsModule(_coreDataHolder, _componentsWrapper, _functions);
        _coreDataHolder = _playerWeaponModule.UpdatePhysicsModule(_coreDataHolder, _componentsWrapper, _functions);
        _coreDataHolder = _playerLifeModule.UpdatePhysicsModule(_coreDataHolder, _componentsWrapper, _functions);
    }

    public void OnHit(int dammage)
    {
        _functions.Call("TakeDamage", dammage);
    }

    public DataHolder LoadOrCreatePlayerData()
    {
        return new DataHolder();
    }
}