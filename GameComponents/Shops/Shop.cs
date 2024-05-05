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
        float judgementBonus = newPrice * judgement / 75;
        float rhetoricsBonus = newPrice * rhetorics / 75;

        newPrice = newPrice + prosperityBonus + judgementBonus + rhetoricsBonus;

        return (int)Mathf.Ceil(newPrice);
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

    public List<Item> GetSellList()
    {
        if (sellInventory == null || sellInventory.Count == 0)
        {
            sellInventory = new();

            if (customSellList.Count > 0)
            {
                foreach (string objectID in customSellList)
                {
                    Item item = Items.all.FirstOrDefault(c => c.name == objectID);
                    if (item != null)
                    {
                        sellInventory.Add(item);
                    }
                }

                if (sellsCustomOnly)
                {
                    return sellInventory;
                }
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

    public List<Item> GetBuyList()
    {
        if (buyInventory == null || buyInventory.Count == 0)
        {
            buyInventory = new List<Item>();

            if (!buysCustomOnly)
            {
                buyInventory = GetSellList();
            }
            if (customBuyList.Count > 0)
            {
                foreach (string objectID in customBuyList)
                {
                    buyInventory.Add(Items.FindByID(objectID));
                }
            }
        }
        return buyInventory;
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