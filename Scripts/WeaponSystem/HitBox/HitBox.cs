using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private Melee melee;

    PolygonCollider2D polygonCollider2;

    private Transform playerPosition;

    public bool finished;
    private void Start()
    {
        polygonCollider2 = GetComponent<PolygonCollider2D>();
        polygonCollider2.isTrigger = true;
        transform.rotation = PlayerInventory.Instance.transform.rotation;
        try
        {
            Utils.StartCoroutine(Utils.TimedExecution(melee.attackDelayIntervall, () =>
            {
                DestroyImmediate(gameObject);
            }), melee.attackDelayIntervall + 0.1f);
        } catch(Exception)
        {

        }
    }
    private void Update()
    {
        try 
        {
            transform.position = playerPosition.position;
            transform.rotation = playerPosition.rotation;
        } catch(Exception)
        {

        }
    }

    public void SetPlayerPosition(Transform playerPosition)
    {
        this.playerPosition = playerPosition;
    } 

    public void SetColliderType(Sprite hitboxShape)
    {
        polygonCollider2 = GetComponent<PolygonCollider2D>();
        Utils.UpdatePolygonCollider2D(polygonCollider2, hitboxShape);
    }

    public void SetMelee(Melee melee)
    {
        this.melee = melee;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Creature>() && !finished)
        {
            collision.gameObject.GetComponent<Creature>().TakeDamage(melee.damageAmnt);
            finished = true;
        }
    }
}
