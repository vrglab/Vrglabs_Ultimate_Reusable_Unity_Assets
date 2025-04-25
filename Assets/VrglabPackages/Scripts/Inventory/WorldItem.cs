using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class for item gathering of the player
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class WorldItem : MonoBehaviour
{
    public WorldObject ItemType;

    public void Start()
    {
        GetComponent<SpriteRenderer>().sprite = ItemType.GetImage();
    }
}
