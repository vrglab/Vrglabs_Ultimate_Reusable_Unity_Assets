using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot<t>
{
    private Stack<t> stackHandler;
    public bool Full { get; private set; } = false;
    public Type ItemTypeToAccept { get; private set; }
    public Type ActiveItemType { get; private set; }

    public Slot(Type itemType)
    {
        stackHandler = new Stack<t>(itemType);
        ItemTypeToAccept = itemType;
    }

    public Slot(Type itemType, int maxStackSize)
    {
        stackHandler = new Stack<t>(itemType, maxStackSize);
        ItemTypeToAccept = itemType;
    }

    /// <summary>
    /// Push's the given item into the stack if it can
    /// </summary>
    /// <param name="item">The item to push</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Push(t item)
    {
        if (stackHandler.currentStackSize < stackHandler.maxSize)
        {
            stackHandler.Put(item);
            ActiveItemType = item.GetType();
            Full = true;
        }  
    }

    /// <summary>
    /// Remove's the given item from the stack if it can
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Remove(t item)
    {
        stackHandler.Remove(item);
        if (Full && stackHandler.currentStackSize <= 0)
        {
            Full = false;
        }
    }
}
