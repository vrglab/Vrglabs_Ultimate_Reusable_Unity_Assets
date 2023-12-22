using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Weapons/Projectile")]
public class Projectiles : WorldObject
{
    public float Speed;
    public float impactForce;
    public int LifeTimeInS;
    public int DammageAmnt;
    public GameObject ProjectileObj;

    public override void OnUse(params object[] args)
    {
    }

    public override void OnUserInteract()
    {
    }

    public override void OnWorldInteract()
    {
    }

    public void Shoot(Vector2 atPosition, Transform parent, Vector2 InDir, GameObject caster, UnityEvent<GameObject, GameObject> onCollision = default, UnityEvent<GameObject> onStart = default, UnityEvent<GameObject> onUpdate = default)
    {
       GameObject spawnedObj = Instantiate(ProjectileObj, parent, true);
       spawnedObj.transform.localPosition = atPosition;
       ProjectileObjMannager mannager = spawnedObj.AddComponent<ProjectileObjMannager>();
       mannager.ParentProjectile = this;
       mannager.goingDirection = InDir;
       mannager.Caster= caster;
       mannager.ps = spawnedObj.GetComponentInChildren<ParticleSystem>();
       mannager.onCollision = onCollision;
       mannager.onStart = onStart;
       mannager.onUpdate = onUpdate;
    }
}
