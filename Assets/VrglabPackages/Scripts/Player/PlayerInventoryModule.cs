using FMODUnity;
using NUnit.Framework.Internal.Execution;
using UnityEngine;

public class PlayerInventoryModule : ModuleSystem
{
    public class StaticModifiers : CoreModifier
    {
        public Inventory<WorldObject> core_inventory { 
            get { return (Inventory<WorldObject>)this["inventory"]; } 
            private set { this["inventory"] = value; } 
        }

        public Inventory<WorldObject> hotbar_inventory
        {
            get { return (Inventory<WorldObject>)this["hotbar_inventory"]; }
            private set { this["hotbar_inventory"] = value; }
        }

        public Slot<Weapon> weapon_slot { 
            get { return (Slot<Weapon>)this["weapon_slot"]; } 
            private set { this["weapon_slot"] = value; }
        } 

        public float pullSpeed { 
            get
            {
                return (float)this["pullSpeed"];
            } 
            private set
            {
                this["pullSpeed"] = value;
            }
        }

        public float pullRange
        {
            get
            {
                return (float)this["pullRange"];
            }
            private set
            {
                this["pullRange"] = value;
            }
        }

        public float pickUpRange
        {
            get
            {
                return (float)this["pickUpRange"];
            }
            private set
            {
                this["pickUpRange"] = value;
            }
        }

        public float weaponPickUpRange
        {
            get
            {
                return (float)this["pickUpRange"];
            }
            private set
            {
                this["pickUpRange"] = value;
            }
        }


        public StaticModifiers(int slots, int hotbar_slots, float pullSpeed = 9, float pullRange = 4, float pickUpRange = 1f, float weaponPickUpRange = 2f)
        {
            core_inventory = new Inventory<WorldObject>(slots);
            hotbar_inventory = new Inventory<WorldObject>(hotbar_slots);

            weapon_slot = new Slot<Weapon>(typeof(Weapon), 1);

            this.pullSpeed = pullSpeed;
            this.pullRange = pullRange;
            this.pickUpRange = pickUpRange;
            this.weaponPickUpRange = weaponPickUpRange;

        }
    }


    public bool IsFull {  get { return staticModifiers.core_inventory.Full && staticModifiers.hotbar_inventory.Full; } }


    private StaticModifiers staticModifiers;

    public PlayerInventoryModule(StaticModifiers modifiers)
    {
        staticModifiers = modifiers;
    }

    private bool CanBePickedUp<T>(T item) where T : WorldObject
    {
        return staticModifiers.core_inventory.CanBeAdded(item) && staticModifiers.hotbar_inventory.CanBeAdded(item);
    }
    private bool CanPickupWeapon(WorldItem worldItem)
    {
        return (typeof(Weapon).IsChildOf(worldItem.ItemType.GetType()) || worldItem.ItemType.GetType() == typeof(Weapon));
    }


    public void placeInCore<T>(T item) where T : WorldObject
    {
        staticModifiers.core_inventory.Put(item);
    }

    public void placeInCore<T>(T item, DataHolder dataHolders) where T : WorldObject
    {
        staticModifiers.core_inventory.Put(item, dataHolders);
    }

    public void placeInHotbar<T>(T item) where T : WorldObject
    {
        staticModifiers.hotbar_inventory.Put(item);
    }

    public void EquipWeapon<T>(T weapon) where T : Weapon
    {
        staticModifiers.weapon_slot.Push(weapon);
    }


    public override DataHolder UpdateModule(DataHolder dataHolders, Components components, Functions functions)
    {
        PullNearbyItems((Transform)components["transform"], dataHolders);
        PickWeaponInRange((Transform)components["transform"], dataHolders, functions);
        return dataHolders;
    }

    public override DataHolder UpdatePhysicsModule(DataHolder dataHolders, Components components, Functions functions)
    {
        return dataHolders;
    }

    public override DataHolder InitModule(DataHolder dataHolders, Components components, Functions functions)
    {

        functions.Bind("RemoveWeapon", (Weapon w) => {
            staticModifiers.weapon_slot.Remove(w); }
        );
        return dataHolders;
    }

    /// <summary>
    /// Pulls nearby items towards the player within a certain range
    /// </summary>
    private void PullNearbyItems(Transform playerTransform, DataHolder dataHolders)
    {
        Collider2D[] itemsInRange = Physics2D.OverlapCircleAll(playerTransform.position, staticModifiers.pullRange);

        foreach (var collider in itemsInRange)
        {
            if (collider.TryGetComponent(out WorldItem worldItem))
            {
                if(CanBePickedUp(worldItem.ItemType) && !CanPickupWeapon(worldItem))
                {
                    // Move item towards player
                    Vector2 direction = (playerTransform.position - worldItem.transform.position).normalized;
                    worldItem.transform.position += (Vector3)direction * staticModifiers.pullSpeed * Time.deltaTime;

                    // Check if item is within pickup range
                    if (Vector2.Distance(worldItem.transform.position, playerTransform.position) <= staticModifiers.pickUpRange)
                    {
                        if (!IsFull)
                        {
                            placeInCore(worldItem.ItemType, dataHolders);
                            MonoBehaviour.Destroy(worldItem.gameObject);
                            LevelManager.Instance.CurrentLevel.LevelData.GetData<DataHolder>(DataIds.getIUWCoreDataKey(worldItem.ItemType.name)).SetData(DataIds.getIUWPickupStateKey(worldItem.ItemType.name), true);
                            if(!LevelObjectSpawner.Instance.isItemOriginHere(worldItem.ItemType))
                            {
                                LevelManager.Instance.CurrentLevel.ActiveItemsInLevel[IUWRegistery.Instance.ResolveId(worldItem.ItemType)].SetData(DataIds.getIUWPickupStateKey(worldItem.ItemType.name), true);
                            }
                        }
                    }
                }
            }
        }
    }

    private void PickWeaponInRange(Transform playerTransform, DataHolder dataHolders, Functions func)
    {
        bool pickup_requested = InputManager.Instance.GetKeyDown("pickup_weapon");
        Collider2D[] itemsInRange = Physics2D.OverlapCircleAll(playerTransform.position, staticModifiers.pullRange);
        foreach (var collider in itemsInRange)
        {
            if (collider.TryGetComponent(out WorldItem worldItem))
            {
                if(CanPickupWeapon(worldItem))
                {
                    // Check if item is within pickup range
                    if (Vector2.Distance(worldItem.transform.position, playerTransform.position) <= staticModifiers.weaponPickUpRange && pickup_requested)
                    {
                        if (!IsFull)
                        {
                            func.Call("PickupWeapon", (Weapon)worldItem.ItemType);
                            EquipWeapon((Weapon)worldItem.ItemType);
                            MonoBehaviour.Destroy(worldItem.gameObject);
                            LevelManager.Instance.CurrentLevel.LevelData.GetData<DataHolder>(DataIds.getIUWCoreDataKey(worldItem.ItemType.name)).SetData(DataIds.getIUWPickupStateKey(worldItem.ItemType.name), true);
                            if (!LevelObjectSpawner.Instance.isItemOriginHere(worldItem.ItemType))
                            {
                                LevelManager.Instance.CurrentLevel.ActiveItemsInLevel[IUWRegistery.Instance.ResolveId(worldItem.ItemType)].SetData(DataIds.getIUWPickupStateKey(worldItem.ItemType.name), true);
                            }
                        }
                    }
                }
            }
        }
    }
}
