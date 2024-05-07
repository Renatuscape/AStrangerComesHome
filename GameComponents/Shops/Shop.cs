using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Shop
{
    public string name;
    public string objectID;
    public string shopkeeperID;
    public string backgroundGraphic;
    public string containerGraphic;
    public int profitMargin = 25;
    public float buyPenalty = 0.9f; // lower sell-price further with this value.
    public List<ShopCategory> categories;
    public int saleDay; // Set to -1 to never hold sales
    public int closedDay = -1; // Set to -1 to never close

    public bool doesNotSell;
    public bool buysItems;
    public bool sellsCustomOnly;
    public bool buysCustomOnly;

    public RequirementPackage checks;
    public List<Item> sellInventory;
    public List<Item> buyInventory;

    [TextArea(5, 20)]
    public string shopDescription;

    public List<string> customSellList = new();
    public List<string> customBuyList = new();

    public void Initialise()
    {
        foreach (var category in categories)
        {
            category.Parse();
        }

        if (profitMargin < 0)
        {
            profitMargin = 0;
        }
    }
    public Character GetShopkeeper()
    {
        return Characters.all.FirstOrDefault(c => c.name == shopkeeperID);
    }

    public int CalculateSellFromInventoryPrice(Item item)
    {
        var prosperity = Player.GetCount("ATT006", name);
        var judgement = Player.GetCount("ATT002", name);
        var rhetorics = Player.GetCount("MAG002", name);

        var buyPrice = CalculateBuyFromShopPrice(item);

        float newPrice = buyPrice * 0.5f;
        float prosperityBonus = newPrice * prosperity / 50;
        float judgementBonus = newPrice * judgement / 50;
        float rhetoricsBonus = newPrice * rhetorics / 50;

        newPrice = newPrice + prosperityBonus + judgementBonus + rhetoricsBonus;

        return (int)Mathf.Ceil(newPrice * buyPenalty);
    }

    public int CalculateBuyFromShopPrice(Item item)
    {
        //CALCULATE SHOP RATE
        var judgement = Player.GetCount("ATT002", name);
        var rhetorics = Player.GetCount("MAG002", name);

        float newItemPrice = item.basePrice + (item.basePrice * profitMargin / 100);

        if (saleDay == (int)TransientDataScript.GetWeekDay())
        {
            var percent25 = newItemPrice / 4;
            newItemPrice = newItemPrice - percent25;
        }

        float tax = newItemPrice;

        if (judgement > 0)
        {
            float judgementDiscount = newItemPrice * judgement / 75;
            tax = tax - judgementDiscount;
        }

        if (rhetorics > 0)
        {
            float rhetoricsDiscount = newItemPrice * rhetorics / 75;
            tax = tax - rhetoricsDiscount;
        }

        newItemPrice += tax;

        return (int)Mathf.Ceil(newItemPrice);
    }

    public List<Item> GetSellFromShopList()
    {
        Debug.Log("Called SellFromShopList for " + name);

        if (sellInventory == null || sellInventory.Count == 0)
        {
            sellInventory = new();

            if (customSellList.Count > 0)
            {
                foreach (string objectID in customSellList)
                {
                    Item item = Items.all.FirstOrDefault(c => c.objectID.Contains(objectID));

                    if (item != null)
                    {
                        sellInventory.Add(item);
                    }
                }

                Debug.Log("At least one custom item was added. Returning sellInventory with count " + sellInventory.Count);
            }

            if (!sellsCustomOnly)
            {
                var curatedItems = Items.all.Where(c =>
                {
                    if (c.notBuyable)
                    {
                        return false;
                    }
                    else
                    {
                        foreach (var category in categories)
                        {
                            if (c.type == category.type && category.rarities.Contains(c.rarity))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                }).ToList();

                foreach (var item in curatedItems)
                {
                    sellInventory.Add(item);
                }
            }
        }

        // Debug.Log("Shop returned sell inventory with count " + sellInventory.Count);
        return sellInventory;
    }

    public List<Item> GetBuyFromPlayerList(bool excludeItemsNotInInventory)
    {
        if (buyInventory == null || buyInventory.Count == 0)
        {
            buyInventory = new();


            if (customBuyList.Count > 0)
            {
                foreach (string objectID in customBuyList)
                {
                    buyInventory.Add(Items.FindByID(objectID));
                }
            }

            if (!buysCustomOnly)
            {
                var curatedItems = Items.all.Where(c =>
                {
                    if (c.notBuyable)
                    {
                        return false;
                    }
                    else
                    {
                        foreach (var category in categories)
                        {
                            if (c.type == category.type && category.rarities.Contains(c.rarity))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                }).ToList();

                foreach (var item in curatedItems)
                {
                    buyInventory.Add(item);
                }
            }
        }

        if (excludeItemsNotInInventory)
        {
            var filteredList = buyInventory.Where(i => Player.GetCount(i.objectID, "Shop") > 0).ToList();
            // Debug.Log("Filtered list has " + filteredList.Count + " items, reduced from " + buyInventory.Count);
            return filteredList;
        }
        else
        {
            return buyInventory;
        }
    }

    public bool CheckRequirements()
    {
        return RequirementChecker.CheckPackage(checks);
    }
}

[System.Serializable]
public class ShopCategory
{
    public string typeTag;
    public ItemType type;

    public List<string> rarityTags;
    public List<ItemRarity> rarities;

    public void Parse()
    {
        type = Items.GetItemType(typeTag.ToUpper());

        foreach (string tag in rarityTags)
        {
            rarities.Add(Items.GetItemRarity(tag.ToUpper()));
        }
    }
}