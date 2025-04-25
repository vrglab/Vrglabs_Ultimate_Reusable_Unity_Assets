using UnityEngine;

public class PlayerWeaponModule : ModuleSystem
{
    private class WeaponInstance
    {
        public Weapon baseWeaponType {  get; private set; }

        public DataHolder InstanceData { get; private set; }


        private float fireCooldownTimer = 0f;


        public WeaponInstance(Weapon base_weapon_type)
        {
            this.baseWeaponType = base_weapon_type;
            this.InstanceData = base_weapon_type.GetDataHolderObject();
        }

        public void TryFire(Transform firePoint, Directions facing)
        {
            if (baseWeaponType == null || InstanceData == null) return;

            // Fire cooldown check
            float fireRate = InstanceData.GetData<float>("fire_rate");
            if (fireCooldownTimer > 0f) return;

            // Ammo check
            bool hasUnlimitedAmmo = InstanceData.GetData<bool>("has_unlimited_ammo");
            int currentAmmo = InstanceData.GetData<int>("current_magazine_amount");

            if (!hasUnlimitedAmmo && currentAmmo <= 0)
            {
                // TODO: Optional: trigger dry-fire sound/effect
                return;
            }

            // Consume ammo if needed
            if (!hasUnlimitedAmmo)
            {
                InstanceData.SetData("current_magazine_amount", currentAmmo - 1);
            }

            // Apply cooldown
            fireCooldownTimer = fireRate;

            // Resolve projectile
            string projectileId = InstanceData.GetData<string>("projectile_id");
            if (string.IsNullOrEmpty(projectileId)) return;

            Projectile projectileSO = WeaponRegistry.Instance.ResolveProjectile(projectileId);
            if (projectileSO == null) return;

            GameObject projectilePrefab = WeaponRegistry.Instance.ResolveProjectilePrefab(projectileId);
            if (projectilePrefab == null) return;

            DataHolder projectileData = projectileSO.GetDataHolderObject();

            // Instantiate and initialize projectile
            GameObject proj = Object.Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            var runtime = proj.GetComponent<ProjectileRuntimeInstance>();
            if (runtime != null)
            {
                runtime.Initialize(
                    projectileData,
                    facing.ParseDirection(),
                    projectileSO,
                    PlayerController.Instance.gameObject.GetInstanceID()
                );
            }
        }

        public void TickCooldown(float deltaTime)
        {
            fireCooldownTimer -= deltaTime;
            if (fireCooldownTimer < 0f) fireCooldownTimer = 0f;
        }

    }


    private WeaponInstance weapon_instance;

    private Transform transform;
    private Functions functions_registery;


    public void PickupWeapon(Weapon weapon)
    {
        DropActiveWeapon();
        weapon_instance = new WeaponInstance(weapon);
    }

    public void DropActiveWeapon()
    {
        if(weapon_instance != null)
        {
            functions_registery.Call("RemoveWeapon", weapon_instance.baseWeaponType);
            LevelObjectSpawner.Instance.Spawn(weapon_instance.baseWeaponType, transform.position, additionalGameObjectData: (Obj) => {
                Obj.layer = LayerMask.NameToLayer("Weapon");
                BoxCollider2D trigger_area = Obj.AddComponent<BoxCollider2D>();
                trigger_area.size = new Vector2(1, 1);
                Rigidbody2D rigidbody2D = Obj.AddComponent<Rigidbody2D>();
            }); 
            weapon_instance = null;
        }
    }


    public override DataHolder InitModule(DataHolder dataHolders, Components components, Functions functions)
    {
        functions.Bind("PickupWeapon", (Weapon w) => PickupWeapon(w));
        functions.Bind("DropWeapon", () => DropActiveWeapon());

        functions.Bind("TryFire", () =>
        {
            if (weapon_instance != null)
            {
                Transform firePoint = transform;
                weapon_instance.TryFire(firePoint, dataHolders.GetData<Directions>("player_facing"));
            }
        });

        functions.Bind("ReloadWeapon", () =>
        {
            if (weapon_instance == null) return;

            int maxAmmo = weapon_instance.InstanceData.GetData<int>("max_magazine_amount");
            weapon_instance.InstanceData.SetData("current_magazine_amount", maxAmmo);
        });

        return dataHolders;
    }

    public override DataHolder UpdateModule(DataHolder dataHolders, Components components, Functions functions)
    {
        transform = (Transform)components["transform"];
        functions_registery = functions;

        weapon_instance?.TickCooldown(Time.deltaTime);


        if (InputManager.Instance.GetKeyDown("drop_weapon"))
        {
            functions.Call("DropWeapon");
        }

        if(InputManager.Instance.GetKeyDown("shoot_weapon"))
        {
            functions.Call("TryFire");
        }
        return dataHolders;
    }

    public override DataHolder UpdatePhysicsModule(DataHolder dataHolders, Components components, Functions functions)
    {
        return dataHolders;
    }
}
