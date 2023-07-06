using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop", menuName = "Scriptable Object/Shop")]
public class Shop : ScriptableObject
{
    private TransientDataScript transientData; 
    public string shopName;
    public GameObject externalPrefab; //WORLD OBJECT
    public GameObject internalPrefab; //CANVAS OBJECT
    public Character shopKeeper;
    public ItemRarity itemRarityA;
    public ItemRarity itemRarityB;
    public ItemRarity itemRarityC;
    public DayOfWeek saleDay;
    public MotherObject specialItemA;
    public MotherObject specialItemB;
    public MotherObject specialItemC;
    public MotherObject specialItemD;
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

    public List<MotherObject> shopInventory;

    public void SetupShop()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        shopKeeper.NameSetup();
        shopInventory.Clear();
        
        foreach (MotherObject x in transientData.objectIndex)
        {
            if (x.rarity == itemRarityA || x.rarity == itemRarityB || x.rarity == itemRarityC)
            {
                if (sellsUpgrades == true)
                    if (x is Upgrade)
                    {
                        if (shopInventory.Contains(x) != true)
                            shopInventory.Add(x);
                    }

                if (x is Item)
                {
                    var itemCheck = (Item)x;

                    if (!itemCheck.hideFromShops)
                    {
                        if (sellsSeeds == true)
                            if (x is Seed)
                            {
                                if (shopInventory.Contains(x) != true)
                                    shopInventory.Add(x);
                            }
                        if (sellsPlants == true)
                            if (x is Plant)
                            {
                                if (shopInventory.Contains(x) != true)
                                    shopInventory.Add(x);
                            }
                        if (sellsCatalysts == true)
                            if (x is Catalyst)
                            {
                                if (shopInventory.Contains(x) != true)
                                    shopInventory.Add(x);
                            }
                        if (sellsTreasures == true)
                            if (x is Treasure)
                            {
                                if (shopInventory.Contains(x) != true)
                                    shopInventory.Add(x);
                            }
                        if (sellsMaterials == true)
                            if (x is Material)
                            {
                                if (shopInventory.Contains(x) != true)
                                    shopInventory.Add(x);
                            }
                        if (sellsTrade == true)
                            if (x is Trade)
                            {
                                if (shopInventory.Contains(x) != true)
                                    shopInventory.Add(x);
                            }
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
