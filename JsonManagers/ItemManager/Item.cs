using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Item : BaseObject
{
    public int basePrice; //automatically calculated from type, rarity and ID
    public ItemType type; //retrieve from ID
    public ItemRarity rarity; //retrieve from ID
    public Sprite sprite;
    public bool notBuyable; //from ID, second to last letter N/B (not/buyable)
    public bool notSellable; //from ID, last letter N/S (not/sellable)

    //SEEDS
    public string outputID; //the ID of the object this can turn into
    public int health; //Adds to price and growth time
    public int yield; //Adds to price and growth time

    public Item GetOutput()
    {
        if (string.IsNullOrEmpty(outputID))
        {
            return null;
        }
        else
        {
            return Items.FindByID(outputID);
        }
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
            Player.Add(item.objectID, 50, true);
        }
    }

    public static Item FindByID(string searchWord)
    {
        if (searchWord.Contains('-'))
        {
            var baseID = searchWord.Split('-')[0];
            searchWord = baseID;
        }

        return all.FirstOrDefault(i => i.objectID.Contains(searchWord));
    }

    public static int GetMax(string searchWord)
    {
        foreach (Item item in all)
        {
            if (item.objectID.Contains(searchWord))
            {
                return item.maxValue;
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

    public static ItemType GetItemType(string typeCode)
    {
        if (typeCode == "PLA")
        {
            return ItemType.Plant;
        }
        else if (typeCode == "CAT")
        {
            return ItemType.Catalyst;
        }
        else if (typeCode == "MIS")
        {
            return ItemType.Misc;
        }
        else if (typeCode == "MAT")
        {
            return ItemType.Material;
        }
        else if (typeCode == "TRA")
        {
            return ItemType.Trade;
        }
        else if (typeCode == "BOO")
        {
            return ItemType.Book;
        }
        else if (typeCode == "SEE")
        {
            return ItemType.Seed;
        }
        else if (typeCode == "TRE")
        {
            return ItemType.Treasure;
        }
        return ItemType.Script;
    }

    public static void PrintFloatEmbellishedItem(Item item, bool printPrice, bool printRarity, bool printType, bool printSeedData)
    {
        TransientDataScript.PrintFloatText(GetEmbellishedItemText(item, printPrice, printRarity, printType, printSeedData));
    }

    public static string GetItemSeedData(Item item)
    {
        return !string.IsNullOrEmpty(item.outputID) ? $"\nHealth {item.health} | Max yield {item.yield}\nEstimated grow time: {100 * (item.health + item.yield)}" : "Yields nothing";
    }

    public static string GetEmbellishedItemText(Item item, bool printPrice, bool printRarity, bool printType, bool printSeedData)
    {
        string hexColour = "635550";
        if (item.rarity == ItemRarity.Common)
        {
            hexColour = "63411b";
        }
        else if (item.rarity == ItemRarity.Uncommon)
        {
            hexColour = "00766a";
        }
        else if (item.rarity == ItemRarity.Rare)
        {
            hexColour = "2551a9";
        }
        else if (item.rarity == ItemRarity.Extraordinary)
        {
            hexColour = "772084";
        }
        else if (item.rarity == ItemRarity.Mythical)
        {
            hexColour = "b72b66";
        }
        else if (item.rarity == ItemRarity.Unique)
        {
            hexColour = "7d9c00";
        }

        string value = printPrice ? $"\nValue: {MoneyExchange.CalculateSellPrice(item)}" : "";
        string rarity = printRarity ? $"\nRarity: {item.rarity}" : "";
        string type = printType ? $"\nType: {item.type}" : "";
        string seedData = printSeedData ? $"{(item.type == ItemType.Seed ? GetItemSeedData(item) : "")}" : "";
        string content = $"<color=#{hexColour}>{item.name}{value}{rarity}{type}</color>{seedData}";

        return content;
    }
}