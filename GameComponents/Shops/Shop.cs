using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Shop
{
    public string name;
    public string objectID;
    public string shopkeeper;
    public int profitMargin = 25;
    public int clearanceMargin = 10;
    public ItemRarity itemRarityA;
    public ItemRarity itemRarityB;
    public ItemRarity itemRarityC;
    public DayOfWeek saleDay;
    public DayOfWeek closedDay;
    public string specialItemA;
    public string specialItemB;
    public string specialItemC;
    public string specialItemD;

    public bool sellsCustomOnly;
    public bool sellsSeeds;
    public bool sellsPlants;
    public bool sellsTreasures;
    public bool sellsTrade;
    public bool sellsMaterials;
    public bool sellsBooks;
    public bool buysItems;

    public string welcomeText;
    public string farewellText;
    public string sucessfulPurchaseText;
    public string notEnoguhMoneyText;
    public string maxedValueText;
    public string soldItemText;

    [TextArea(5, 20)]
    public string shopDescription;

    public List<string> customInventory = new();

    public Character GetShopkeeper()
    {
        DefineEnums();
        return Characters.all.FirstOrDefault(x => x.name == shopkeeper);
    }
    public List<Item> GetInventory()
    {
        DefineEnums();
        List<Item> shopInventory = new List<Item>();

        if (customInventory.Count > 0)
        {
            foreach (string itemName in customInventory)
            {
                Item item = Items.all.FirstOrDefault(x => x.name == itemName);
                if (item != null)
                {
                    shopInventory.Add(item);
                }
            }

            if (sellsCustomOnly)
            {
                return shopInventory;
            }
        }

        var curatedItems = Items.all.Where(x => x.rarity == itemRarityA || x.rarity == itemRarityB || x.rarity == itemRarityC);
        curatedItems = curatedItems.Where(x => !x.notBuyable && x.rarity != ItemRarity.Script);

        foreach (var item in curatedItems)
        {
            if (sellsSeeds && item.type == ItemType.Seed)
            {
                shopInventory.Add(item);
            }
            if (sellsPlants && item.type == ItemType.Plant)
            {
                shopInventory.Add(item);
            }
            if (sellsTreasures && item.type == ItemType.Treasure)
            {
                shopInventory.Add(item);
            }
            if (sellsTrade && item.type == ItemType.Trade)
            {
                shopInventory.Add(item);
            }
            if (sellsMaterials && item.type == ItemType.Material)
            {
                shopInventory.Add(item);
            }
            if (sellsBooks && item.type == ItemType.Book)
            {
                shopInventory.Add(item);
            }
        }

        return shopInventory;
    }

    public void DefineEnums()
    {
        var dataString = objectID.Split('-');

        if (dataString.Length > 3)
        {
            itemRarityA = Items.GetItemRarity(dataString[1]);
            itemRarityB = Items.GetItemRarity(dataString[2]);
            itemRarityC = Items.GetItemRarity(dataString[3]);

            if (dataString.Length > 4 && int.TryParse(dataString[4], out int dayOfWeekNumberSale))
            {
                saleDay = (DayOfWeek)(dayOfWeekNumberSale);
            }
            if (dataString.Length > 5 && int.TryParse(dataString[5], out int dayOfWeekNumberClosed))
            {
                closedDay = (DayOfWeek)(dayOfWeekNumberClosed);
            }
        }
        else
        {
            Debug.Log($"Something was wrong with objectID ({objectID}) for shop ({name}). Enums could not be defined.");
        }
    }
}
