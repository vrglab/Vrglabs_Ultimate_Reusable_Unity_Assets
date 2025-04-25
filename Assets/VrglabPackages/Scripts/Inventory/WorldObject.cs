using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all the in game world objects (mainly items and tiles)
/// </summary>
public abstract class WorldObject : Idable, IInteractable
{
    /// <summary>
    /// Basic in world/Slot image of a item
    /// </summary>
    public Sprite Image;


    /// <summary>
    /// Gets the Objects active image
    /// </summary>
    /// <returns>Active object image</returns>
    public virtual Sprite GetImage()
    {
        return Image;
    }

    /// <summary>
    /// Gets the Objects active image
    /// </summary>
    /// <remarks>The id is a helper value passed into the method (it is not typically, actually used)</remarks>
    /// <param name="id">Guid passed on</param>
    /// <returns>Active object image</returns>
    public virtual Sprite GetImage(Guid id = new Guid())
    {
        return Image;
    }

    /// <summary>
    /// Gets the Objects active image
    /// </summary>
    /// <remarks>The id and number are helper values passed into the method (they are not typically, actually used)</remarks>
    /// <param name="id">Guid passed on</param>
    /// /// <param name="id">Number passed on</param>
    /// <returns>Active object image</returns>
    public virtual Sprite GetImage(Guid id = new Guid(), int num = 0)
    {
        return Image;
    }

    /// <summary>
    /// Gets the Objects active image
    /// </summary>
    /// <remarks>The number is a helper value passed into the method (it is not typically, actually used)</remarks>
    /// <param name="num">Number passed on</param>
    /// <returns>Active object image</returns>
    public virtual Sprite GetImage(int num = 0)
    {
        return Image;
    }


    public override bool Equals(object other)
    {
        var v1 = other as WorldObject;

        if (v1.id.Equals(id))
        {
            return true;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public abstract void OnWorldInteract();
    public abstract void OnUserInteract();
}

/// <summary>
/// Base class for all scriptable objects that can have a id
/// </summary>
public abstract class Idable : ItemType
{
    protected Guid GUid;

    /// <summary>
    /// Id of the object
    /// </summary>
    public Guid id { get => getId(); set => SetId(value); }

    private void SetId(Guid id)
    {
        GUid = id;
    }

    private Guid getId()
    {
        if (GUid == Guid.Empty)
        {
            SetId(Guid.NewGuid());
            return GUid;
        }
        else
        {
            return GUid;
        }
    }
}
