using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Melee")]
public class Melee : WorldObject
{
    public float attackDelayIntervall;
    public int damageAmnt;
    public float distance;
    public Sprite HitboxImage;

    public void ExecuteAttack(GameObject hitBox, int damageAmnt, Transform startPosition)
    {
        var hit = GameObject.Instantiate(hitBox, startPosition.position, Quaternion.identity);
        hit.GetComponent<HitBox>().SetPlayerPosition(startPosition);
        hit.GetComponent<HitBox>().SetMelee(this);
        hit.GetComponent<HitBox>().SetColliderType(HitboxImage);
        hit.AddComponent<SpriteRenderer>().sprite = GetImage();
    }

    public override void OnUse(params object[] args)
    {
    }

    public override void OnUserInteract()
    {
    }

    public override void OnWorldInteract()
    {
    }
}

public class MeleeHandler
{
    public MeleeData md = new MeleeData();

    private Melee _CurrentMelee;

    private bool CanAttack = true, isAttack = false;
    private float attackDelayIntervall, distance, lifeTime;
    public int damageAmnt;

    private Transform startPoint, objParent, transform;

    private GameObject gameObject, hitBox;


    /// <param name="startPoint">Where to spawn</param>
    /// <param name="objParent">The parent object</param>
    /// <param name="transform">Player Transform</param>
    /// <param name="gameObject">Player object</param>
    public MeleeHandler(Transform startPoint, Transform objParent, Transform transform, GameObject gameObject)
    {
        this.startPoint = startPoint;
        this.objParent = objParent;
        this.transform = transform;
        this.gameObject = gameObject;
    }

    public void SetMeleeState()
    {
        if (attackDelayIntervall <= 0)
        {
            CanAttack = true;
            isAttack = false;
        }
        else
        {
            CanAttack = false;
            isAttack = true;
            attackDelayIntervall -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        if (CanAttack)
        {
            _CurrentMelee.ExecuteAttack(hitBox, damageAmnt, startPoint);
            attackDelayIntervall = md.attackDelayIntervall;
            CanAttack = false;
        }
    }

    public void SetMeleeWeapon(Melee melee)
    {
        _CurrentMelee = melee;
        md = new MeleeData
        {
            attackDelayIntervall = melee.attackDelayIntervall,
            distance = melee.distance
        };
        hitBox = new GameObject("Hitbox");
        hitBox.transform.position = new Vector3(05069, 2023, 1517);
        hitBox.AddComponent<PolygonCollider2D>();
        hitBox.AddComponent<HitBox>();
    }

    public Melee GetMeleeWeapon()
    {
        return _CurrentMelee;
    }

    public void IncreaseAttackDelayIntervall(float increasedByAmount)
    {
        md.attackDelayIntervall += increasedByAmount;
    }

    public void DecreaseAttackDelayIntervall(float decreasedByAmount)
    {
        md.attackDelayIntervall -= decreasedByAmount;
    }

    public void IncreaseDamage(int increasedByAmount)
    {
        damageAmnt += increasedByAmount;
    }

    public void DecreaseDamage(int decreasedByAmount)
    {
        damageAmnt -= decreasedByAmount;
    }

    public void SetIsAttack(bool isAttack)
    {
        this.isAttack = isAttack;
    }

    public bool GetIsAttack()
    {
        return this.isAttack;
    }

    public float GetAttackDelayIntervall()
    {
        return attackDelayIntervall;
    }

    public struct MeleeData
    {
        public float attackDelayIntervall, distance;
        public GameObject hitBox;
        public Vector2 startPoint;
    }
}