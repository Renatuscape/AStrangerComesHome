using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop", menuName = "Scriptable Object/Shop")]
public class Shop : ScriptableObject
{
    public string shopName;
    public GameObject externalPrefab; //WORLD OBJECT
    public GameObject internalPrefab; //CANVAS OBJECT
    public Character shopKeeper;
    public ItemRarity itemRarityA;
    public ItemRarity itemRarityB;
    public ItemRarity itemRarityC;
    public DayOfWeek saleDay;
    public Item specialItemA;
    public Item specialItemB;
    public Item specialItemC;
    public Item specialItemD;
    public int cellWidth = 32;

    public bool sellsUpgrades;
    public bool sellsSeeds;
    public bool sellsPlants;
    public bool sellsCatalysts;
    public bool sellsTreasures;
    public bool sellsTrade;
    public bool sellsMaterials;
    public bool buysItems;

    public string welcomeText;
    public string farewellText;
    public string sucessfulPurchaseText;
    public string notEnoguhMoneyText;
    public string maxedValueText;
    public string soldItemText;

    [TextArea(5, 20)]
    public string shopDescription;

    public List<Item> shopInventory;

    public void SetupShop()
    {
        //shopKeeper.NameSetup();
        shopInventory.Clear();
        
        foreach (Item item in Items.allItems)
        {
            if (item.rarity == itemRarityA || item.rarity == itemRarityB || item.rarity == itemRarityC)
            {
                if (!item.notSellable)
                {
                    if (sellsSeeds == true)
                        if (item.type == ItemType.Seed)
                        {
                            if (shopInventory.Contains(item) != true)
                                shopInventory.Add(item);
                        }
                    if (sellsPlants == true)
                        if (item.type == ItemType.Plant)
                        {
                            if (shopInventory.Contains(item) != true)
                                shopInventory.Add(item);
                        }
                    if (sellsCatalysts == true)
                        if (item.type == ItemType.Catalyst)
                        {
                            if (shopInventory.Contains(item) != true)
                                shopInventory.Add(item);
                        }
                    if (sellsTreasures == true)
                        if (item.type == ItemType.Treasure)
                        {
                            if (shopInventory.Contains(item) != true)
                                shopInventory.Add(item);
                        }
                    if (sellsMaterials == true)
                        if (item.type == ItemType.Material)
                        {
                            if (shopInventory.Contains(item) != true)
                                shopInventory.Add(item);
                        }
                    if (sellsTrade == true)
                        if (item.type == ItemType.Trade)
                        {
                            if (shopInventory.Contains(item) != true)
                                shopInventory.Add(item);
                        }
                }
            }
        }
    }

    public void ClearShop()
    {
        shopInventory = null;
    }
}
