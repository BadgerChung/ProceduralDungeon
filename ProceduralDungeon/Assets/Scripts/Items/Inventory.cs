using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    public Item[] slots { get; private set; }

    public Action<Inventory> inventoryChanged;

    public Inventory(int slotCount)
    {
        slots = new Item[slotCount];
    }

    public bool TryAddItem(Item item)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i] == null)
            {
                slots[i] = item;
                inventoryChanged?.Invoke(this);
                return true;
            }
        }
        return false;
    }

}
