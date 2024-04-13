using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void AddToPlayer(int amount = 1, bool doNotLog = false)
    {
        Player.AddDynamicObject(this, amount, doNotLog, "Item");
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
            item.AddToPlayer(100, true);
        }
    }

    public static Item FindByID(string searchWord)
    {
        return all.FirstOrDefault(i => i.objectID.Contains(searchWord));
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

    public static ItemRarity GetItemRarity(string rarityCode)
    {
        if (rarityCode == "COM")
        {
            return ItemRarity.Common;
        }
        else if (rarityCode == "UNC")
        {
            return ItemRarity.Uncommon;
        }
        else if (rarityCode == "RAR")
        {
            return ItemRarity.Rare;
        }
        else if (rarityCode == "EXT")
        {
            return ItemRarity.Extraordinary;
        }
        else if (rarityCode == "MYT")
        {
            return ItemRarity.Mythical;
        }
        else if (rarityCode == "UNI")
        {
            return ItemRarity.Unique;
        }
        return ItemRarity.Junk;
    }
}