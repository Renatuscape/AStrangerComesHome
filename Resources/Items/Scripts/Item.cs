using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base template for items of various types. Each itemtype needs its own script with additional data and CreateAssetMenu in this space


public class Item : MotherObject
{
    public ItemType type;
    public bool hideFromShops;
    public Recipe recipe;

    void Awake()
    {
        objectType = ObjectType.Item;
    }
}