using System;
using System.Collections.Generic;

public class Stack<t> where t : ItemType
{
    private Type containedItem = default;
    public int maxSize { get; private set; } = 100;
    public int currentStackSize { get; private set; } = 0;
    public List<t> Items { get; private set; } = new List<t>();

    public Stack(Type containedItem)
    {
        this.containedItem = containedItem;
    }

    public Stack(Type containedItem, int maxSize) : this(containedItem)
    {
        this.maxSize = maxSize;
    }

    private bool check_type(t item)
    {
        return item.GetType().IsChildOf(containedItem) || item.GetType() == containedItem;
    }


    public bool CanBePushed(t item)
    {
        if (currentStackSize < item.StackSize)
        {
            if (check_type(item))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Put's the given item into the stack if there is space for it
    /// </summary>
    /// <param name="item">The item to put in the stack</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Put(t item)
    {
        if (currentStackSize < item.StackSize)
        {
            if (check_type(item))
            {
                currentStackSize++;
                item.OnAddedToInventory(containedItem, item.StackSize, currentStackSize, Items);
                Items.Add(item);
            }
        }
    }

    public void Put(t item, params object[] args)
    {
        if (currentStackSize < item.StackSize)
        {
            if (check_type(item))
            {
                currentStackSize++;
                item.OnAddedToInventory(containedItem, item.StackSize, currentStackSize, Items, args);
                Items.Add(item);
            }
        }
    }

    /// <summary>
    /// Removes the given item from the stack
    /// </summary>
    /// <param name="item">the item to be removed</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Remove(t item)
    {
        if (check_type(item))
        {
            currentStackSize--;
            item.OnRemovedToInventory(containedItem, maxSize, currentStackSize, Items);
            Items.Remove(item);
        }
    }
}
