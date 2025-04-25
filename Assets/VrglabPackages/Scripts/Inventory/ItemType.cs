using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemType : ScriptableObject
{
    public new string name;
    public int StackSize = 1;

    /// <summary>
    /// Called When the item Is used
    /// </summary>
    /// <param name="args">The passed arguments</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public abstract void OnUse(params object[] args);


    public abstract void OnAddedToInventory(params object[] args);

    public abstract void OnRemovedToInventory(params object[] args);
}
