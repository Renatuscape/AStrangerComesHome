using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base template for items of various types. Each itemtype needs its own script with additional data and CreateAssetMenu in this space

[System.Serializable]
public class Item
{
    public string objectID;
    public string name;
    public int basePrice; //automatically calculated from type, rarity and ID
    public int maxStack = 99;
    public ItemType type; //retrieve from ID
    public ItemRarity rarity; //retrieve from ID
    public Texture2D image; //retrieve from ID + folder
    public Sprite sprite;
    public List<Texture2D> animationFrames; //for animation frames
    public string description;
    public bool notBuyable; //from ID, second to last letter N/B (not/buyable)
    public bool notSellable; //from ID, last letter N/S (not/sellable)

    //SEEDS
    public string outputID; //the ID of the object this can turn into
    public Sprite stage1; //seed growth1, set by ID
    public Sprite stage2; //seed growth2, set by ID
    public Sprite stage3; //seed growth3, set by ID
    public int health; //Adds to price and growth time
    public int yield; //Adds to price and growth time

    public void AddToPlayer(int amount = 1)
    {
        Player.AddDynamicObject(this, amount, "Item");
    }
    public int GetCountPlayer()
    {
        return Player.GetCount(objectID, "Item");
    }

    public Item GetOutput()
    {
        return Items.FindByID(outputID);
    }
}

public static class Items
{
    public static List<Item> all = new();

    public static void DebugList()
    {
        Debug.LogWarning("Items.DebugList() called");

        foreach (Item item in all)
        {
            Debug.Log($"Item ID: {item.objectID}\tItem Name: {item.name}");
        }
    }

    public static void DebugAllItems()
    {
        foreach (Item item in all)
        {
            item.AddToPlayer(5);
            item.AddToPlayer(10);
            item.AddToPlayer(25);
            item.AddToPlayer(100);
        }
    }

    public static Item FindByID(string searchWord, bool debug = true, string caller = "unknown")
    {
        if (string.IsNullOrWhiteSpace(searchWord))
        {
            Debug.LogWarning($"Search term {searchWord} was null or white-space. Returned null. Ensure correct ID in calling script.");
            return null;
        }
        foreach (Item item in all)
        {
            if (item.objectID.Contains(searchWord))
            {
                return item;
            }
        }

        if (debug)
        {
            Debug.LogWarning("Caller " + caller + " found no item with ID containing this search term: " + searchWord);
        }

        return null;
    }

    public static int GetMax(string searchWord)
    {
        foreach (Item item in all)
        {
            if (item.objectID.Contains(searchWord))
            {
                return item.maxStack;
            }
        }
        return 99;
    }
}