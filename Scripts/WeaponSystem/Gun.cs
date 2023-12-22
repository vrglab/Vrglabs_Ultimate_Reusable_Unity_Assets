using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Weapons/Gun")]
public class Gun : WorldObject
{

    public float ShootingIntervalInS, fireDelayIntervall;
    public CameraShakeData shakedata;
    public int MaxAmmo;
    public int AmmoUseAmntPerShot;

    public override void OnUse(params object[] args)
    {
        
    }

    public override void OnUserInteract()
    {
        
    }

    public override void OnWorldInteract()
    {
        
    }

    public void Shoot(Projectiles projectileToShoot, Vector2 atPosition, Transform parent, Vector2 InDir, GameObject caster, UnityEvent<GameObject, GameObject> onCollision = default, UnityEvent<GameObject> onStart = default, UnityEvent<GameObject> onUpdate = default)
    {
        projectileToShoot.Shoot(atPosition, parent, InDir, caster, onCollision, onStart, onUpdate);
    }
}

/// <summary>
/// Handles everything related to a gun
/// </summary>
public class GunHandler
{
    //Gun stat data (max ammo, ammo use amnt ...)
    private GunData gd = new GunData();

    private Gun _CurentGun;
    private bool CanShoot = true, isAttack = false;
    private float ShootInterval, MultipleShotAmount;
    public int CurrentAmmoAmnt;

    //p_WeaponMannager; variables
    private Transform GunMuzzel, ObjParents, transform;
    private UnityEvent<GameObject, GameObject> OnProjectileShotHit;
    private GameObject gameObject;

    public GunHandler(Transform gunMuzzel, Transform objParents, Transform transform, UnityEvent<GameObject, GameObject> onProjectileShotHit, GameObject gameObject)
    {
        GunMuzzel = gunMuzzel;
        ObjParents = objParents;
        this.transform = transform;
        OnProjectileShotHit = onProjectileShotHit;
        this.gameObject = gameObject;
    }

    public string getAmmoString()
    {
        return CurrentAmmoAmnt + "/" + gd.maxAmmo + " " + LocalizationManager.Instance.GetEntry("ammo");
    }

    public void SetShootState()
    {
        if (0 <= (CurrentAmmoAmnt - gd.ammoUseAmount))
        {
            if (ShootInterval <= 0)
            {
                CanShoot = true;
            }
            else
            {
                CanShoot = false;
                ShootInterval -= Time.deltaTime;
            }
        }
        else
        {
            CanShoot = false;
        }
    }

    public void Shoot(Vector2 dir, bool effects = true)
    {
        if (CanShoot)
        {
            CurrentAmmoAmnt = (CurrentAmmoAmnt - gd.ammoUseAmount);
            Utils.StartCoroutine(Utils.NumberedExecution(gd.ammoUseAmount, gd.fireDelayIntervall, () =>
            {
                _CurentGun.Shoot(gd.currentProjectile, GunMuzzel.position, ObjParents, dir, gameObject, OnProjectileShotHit);
            }), gd.shootInterval);
            if (effects)
            {
                CameraEffects.Instance.Shake(_CurentGun.shakedata.Duration, _CurentGun.shakedata.Magnitued);
            }
            ShootInterval = gd.shootInterval;
            CanShoot = false;
        }
    }

    public void SetGun(Gun gun, Projectiles projectile)
    {
        _CurentGun = gun;
        gd = new GunData
        {
            currentProjectile = projectile,
            maxAmmo = gun.MaxAmmo,
            shootInterval = gun.ShootingIntervalInS,
            ammoUseAmount = gun.AmmoUseAmntPerShot,
            fireDelayIntervall = gun.fireDelayIntervall
        };
        CurrentAmmoAmnt = gd.maxAmmo;
    }

    public void SetGun(Gun gun)
    {
        _CurentGun = gun;
        gd = new GunData
        {
            currentProjectile = gd.currentProjectile,
            maxAmmo = gun.MaxAmmo,
            shootInterval = gun.ShootingIntervalInS,
            ammoUseAmount = gun.AmmoUseAmntPerShot,
            fireDelayIntervall = gun.fireDelayIntervall
        };
        CurrentAmmoAmnt = gd.maxAmmo;
    }

    public void SetProjectile(Projectiles projectile)
    {
        gd.currentProjectile = projectile;
    }

    public void IncreaseAmmoBy(int amnt)
    {
        if ((CurrentAmmoAmnt + amnt) > gd.maxAmmo)
        {
            CurrentAmmoAmnt = gd.maxAmmo;
        }
        else
        {
            CurrentAmmoAmnt = (CurrentAmmoAmnt + amnt);
        }
    }

    public Gun GetCurrentGun()
    {
        return _CurentGun;
    }

    public Projectiles GetCurentProjectile()
    {
        return gd.currentProjectile;
    }

    public void IncreaseShootInterval(float amnt)
    {
        gd.shootInterval = (gd.shootInterval + amnt);
    }

    public void DecreaseShootInterval(float amnt)
    {
        gd.shootInterval = (gd.shootInterval - amnt);
    }

    public void IncreaseMaxAmmo(int amnt)
    {
        gd.maxAmmo = (gd.maxAmmo + amnt);
    }

    public void DecreaseMaxAmmo(int amnt)
    {
        gd.maxAmmo = (gd.maxAmmo - amnt);
    }

    public void setShootInterval(float amnt)
    {
        gd.shootInterval = amnt;
    }

    public void setMaxAmmo(int amnt)
    {
        gd.maxAmmo = amnt;
    }

    public void IncreaseAmmoUseAmount(int amnt)
    {
        gd.ammoUseAmount = (gd.ammoUseAmount + amnt);
    }

    public void DecreaseAmmoUseAmount(int amnt)
    {
        gd.ammoUseAmount = (gd.ammoUseAmount - amnt);
    }

    public void setAmmoUseAmount(int amnt)
    {
        gd.ammoUseAmount = amnt;
    }

    public void setAmmoUseDelay(int shootIntervall)
    {
        gd.shootInterval = shootIntervall;
    }
    public void SetIsAttack(bool isAttack)
    {
        this.isAttack = isAttack;
    }

    public bool GetIsAttack()
    {
        return this.isAttack;
    }

    public float GetShootInterval()
    {
        return ShootInterval;
    }


    public struct GunData
    {
        public int ammoUseAmount;
        public int maxAmmo;
        public float shootInterval;
        public float fireDelayIntervall;
        public Projectiles currentProjectile;
    }
}