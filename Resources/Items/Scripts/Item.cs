using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base template for items of various types. Each itemtype needs its own script with additional data and CreateAssetMenu in this space
public enum ItemType
{
    Seed,
    Plant,     //made from seeds
    Catalyst,  //made from plants
    Material,  //made from ingredients and catalysts
    Treasure,  //made from materials and catalysts
    Trade,     //has no function besides buy/sell at market
    Misc       //modular and unique items, quest items, whatever. Not sold in any stores
}


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