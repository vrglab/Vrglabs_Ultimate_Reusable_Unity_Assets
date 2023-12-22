using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public WorldObject ItemType;

    private bool isClientInReach;

    public void Start()
    {
        GetComponent<SpriteRenderer>().sprite = ItemType.GetImage();
    }

    public void Update()
    {
        if (isClientInReach)
        {
            if (InputManager.Instance.GetKeyDown("pl_interaction"))
            {
                PlayerInventory.Instance.Inventory.Put(ItemType);
                Debug.Log($"Added item named: {ItemType.name} into inventory");
                Destroy(gameObject);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isClientInReach = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isClientInReach = false;
        }
    }
}
