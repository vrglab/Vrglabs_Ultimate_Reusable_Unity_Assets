using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileObjMannager : MonoBehaviour
{
    public Projectiles ParentProjectile { get; set; }
    public GameObject Caster { get; set; }
    public Vector2 goingDirection { get; set; }
    private Rigidbody2D  RGbody { get; set; }
    public ParticleSystem ps;


    public UnityEvent<GameObject, GameObject> onCollision;
    public UnityEvent<GameObject> onStart;
    public UnityEvent<GameObject> onUpdate;

    private float LifeTime;

    private void Start()
    {
        if(GetComponent<Rigidbody2D>() != null)
        {
            RGbody = GetComponent<Rigidbody2D>();
        }
        else
        {
            RGbody = gameObject.AddComponent<Rigidbody2D>();
        }
        LifeTime = ParentProjectile.LifeTimeInS;
        gameObject.GetComponent<SpriteRenderer>().sprite = ParentProjectile.GetImage();
        if(onStart != null )
        {
            onStart.Invoke(gameObject);
        }
    }


    private IEnumerator SelfDestructSafely()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        ps.textureSheetAnimation.AddSprite(gameObject.GetComponent<SpriteRenderer>().sprite);
        ps.Play();
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }

    bool hit;
    private void Update()
    {
        if (!hit)
        {
            if (LifeTime <= 0)
            {
                StartCoroutine(SelfDestructSafely());
            }
            else
            {
                LifeTime -= Time.deltaTime;
                Vector2 movePosition = Vector2.zero;

                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
                movePosition = new Vector2(goingDirection.x * ParentProjectile.Speed, goingDirection.y * ParentProjectile.Speed);
                RGbody.AddForce(movePosition * Time.deltaTime);
                
                if(onUpdate != null)
                {
                    onUpdate.Invoke(gameObject);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hit)
        {
            GameObject obj = collision.gameObject;
            if (obj != Caster && obj.tag != Caster.tag && (Utils.HasInterfaces<IProjectileInteractable>(obj) || obj.GetComponent<Creature>() != null))
            {
                if (Utils.HasInterfaces<IProjectileInteractable>(obj))
                {
                    Utils.GetInterfaces(out List<IProjectileInteractable> rtg, obj);
                    foreach (var item in rtg)
                    {
                        if (onCollision != null)
                            onCollision.Invoke(gameObject, obj);
                        item.OnProjectileHit(ParentProjectile);
                    }
                }
                if (obj.GetComponent<Creature>())
                {
                    Creature creature = obj.GetComponent<Creature>();
                    if (creature.IsDead)
                    {
                        Rigidbody2D rgBody = obj.GetComponent<Rigidbody2D>();
                        rgBody.AddForce(new Vector3(goingDirection.x * ParentProjectile.impactForce, goingDirection.y * ParentProjectile.impactForce));
                    }
                }
                StartCoroutine(SelfDestructSafely());
                hit = true;
            }
            else
            {
                if (obj != Caster)
                {
                    StartCoroutine(SelfDestructSafely());
                }
            }
            
        }
       
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
