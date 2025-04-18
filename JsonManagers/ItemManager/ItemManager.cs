using UnityEngine;

// Once items are loaded, set up the correct data here
public class ItemManager : MonoBehaviour
{
    public static void Initialise(Item item)
    {
        // Set base object values
        item.objectType = ObjectType.Item;
        item.SetupTags();

        // Set item-specific values
        ItemIDReader(ref item);

        if (item.type != ItemType.Misc && item.type != ItemType.Script)
        {
            item.basePrice = CalculatePrice(ref item);
        }

        if (item.maxValue == 0)
        {
            if (item.rarity == ItemRarity.Unique)
            {
                item.maxValue = 1;
            }
            else if (item.type == ItemType.Book)
            {
                item.maxValue = StaticGameValues.maxBookValue;
            }
            else
            {
                item.maxValue = StaticGameValues.maxItemValue;
            }
        }

        item.objectID = item.objectID.Split('-')[0];
        item.sprite = SpriteFactory.GetItemSprite(item.objectID);
    }

    public static void ItemIDReader(ref Item item)
    {
        item.type = TypeFinder(ref item);
        item.rarity = RarityFinder(ref item);

        if (item.objectID[11] == 'N')
        {
            item.notBuyable = true;
        }
        else if (item.objectID[11] != 'B')
        {
            Debug.LogError($"{item.objectID} ID was not formatted correctly. Could not find N/B at index[11]");
        }
        if (item.objectID[12] == 'N')
        {
            item.notSellable = true;
        }
        else if (item.objectID[12] != 'S')
        {
            Debug.LogError($"{item.objectID} ID was not formatted correctly. Could not find N/S at index[12]");
        }
    }

    public static ItemType TypeFinder(ref Item item)
    {
        var objectID = item.objectID;
        if (objectID.Contains("PLA"))
        {
            return ItemType.Plant;
        }
        else if (objectID.Contains("SEE"))
        {
            return ItemType.Seed;
        }
        else if (objectID.Contains("CAT"))
        {
            return ItemType.Catalyst;
        }
        else if (objectID.Contains("BOO"))
        {
            return ItemType.Book;
        }
        else if (objectID.Contains("MAT"))
        {
            return ItemType.Material;
        }
        else if (objectID.Contains("TRA"))
        {
            return ItemType.Trade;
        }
        else if (objectID.Contains("TRE"))
        {
            return ItemType.Treasure;
        }
        else if (objectID.Contains("SCR"))
        {
            return ItemType.Script;
        }
        else
        {
            return ItemType.Misc;
        }
    }

    public static ItemRarity RarityFinder(ref Item item)
    {
        if (item.objectID.Contains("EXT"))
        {
            return ItemRarity.Extraordinary;
        }
        else if (item.objectID.Contains("MYT"))
        {
            return ItemRarity.Mythical;
        }
        else if (item.objectID.Contains("RAR"))
        {
            return ItemRarity.Rare;
        }
        else if (item.objectID.Contains("UNC"))
        {
            return ItemRarity.Uncommon;
        }
        else if (item.objectID.Contains("UNI"))
        {
            return ItemRarity.Unique;
        }
        else if (item.objectID.Contains("JUN"))
        {
            return ItemRarity.Junk;
        }
        else if (item.objectID.Contains("SCR"))
        {
            return ItemRarity.Script;
        }
        else
        {
            return ItemRarity.Common;
        }
    }

    public static int CalculatePrice(ref Item item)
    {
        return ValueIndex.CalculateItemPrice(ref item);
    }
}

public static class ValueIndex
{
    public static int CalculateItemPrice(ref Item item)
    {
        float price = 5;

        int seedPrice = 1;
        int plantPrice = 5;
        int materialPrice = 7;
        int catalystPrice = 20;
        int tradePrice = 35;
        int bookPrice = 50;
        int treasurePrice = 80;

        switch (item.type)
        {
            case ItemType.Seed:
                if (item.health != 0 && item.yield != 0)
                {
                    price = seedPrice * (item.health + item.yield);
                }
                break;
            case ItemType.Plant:
                price = plantPrice;
                break;
            case ItemType.Material:
                price = materialPrice;
                break;
            case ItemType.Catalyst:
                price = catalystPrice;
                break;
            case ItemType.Trade:
                price = tradePrice;
                break;
            case ItemType.Book:
                price = bookPrice;
                break;
            case ItemType.Treasure:
                price = treasurePrice;
                break;
            default:
                break;
        }

        float rarityMultiplier = 0.3f;

        if ((int)item.rarity >= 0)
        {
            rarityMultiplier = (((float)item.rarity * (float)item.rarity) + 1) * 10;
        }

        price = price * rarityMultiplier;

        // Create pricing variety
        int itemNumber = ExtractItemNumberFromID(item.objectID);
        float numberModifier = itemNumber * 3;
        float uniquePrice = price + numberModifier;

        // Debug.Log($"{item.type} {item.name} ({item.rarity}) priced at " + (int)uniquePrice);
        return (int)uniquePrice;

        static int ExtractItemNumberFromID(string itemID)
        {
            // Extract the three numbers from index 5
            string numberPart = itemID[5].ToString();
            int itemNumber;
            int.TryParse(numberPart, out itemNumber);
            return itemNumber;
        }
    }
}