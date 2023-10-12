using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base inventory class
/// </summary>
/// <typeparam name="t">The type of Item to be contained</typeparam>
public class Inventory<t>
{
    private List<Slot<t>> slots = new List<Slot<t>>();
    public bool Full { get; private set; } = false;

    public Inventory(int SlotAmount)
    {
        initSlots(SlotAmount, typeof(t));
    }

    /// <summary>
    /// Initializes the slot sets to be used by the inventory
    /// </summary>
    /// <param name="SlotAmount">The amount slots to generate</param>
    /// <param name="DefaultItemType">The item type which the slot will contain</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    protected void initSlots(int SlotAmount, Type DefaultItemType)
    {
        for (int i = 0; i < SlotAmount; i++)
        {
            slots.Add(new Slot<t>(DefaultItemType));
        }
    }

    /// <summary>
    /// Put's the item into a inventory slot
    /// </summary>
    /// <param name="item">The item to put in a slot</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Put(t item)
    {
        foreach (var slot in slots)
        {
            if (slot.Full)
            {
                if (item.GetType().IsInstanceOfType(slot.ItemTypeToAccept))
                {
                    slot.Push(item);
                    return;
                }
            }
            else
            {
                slot.Push(item);
                return;
            }
        }
        Full = true;
    }

    /// <summary>
    /// Removes the item from the requested slot
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <param name="slot">The index of the slot to remove the item from</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Remove(t item, int slot)
    {
        if (item.GetType().IsInstanceOfType(slots[slot].ItemTypeToAccept))
        {
            slots[slot].Remove(item);
        }
        Full = false;
    }
}
