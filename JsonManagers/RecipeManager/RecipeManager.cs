using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RecipeManager
{
    public static void Initialise(Recipe recipe)
    {
        recipe.objectType = ObjectType.Recipe;
        recipe.maxValue = StaticGameValues.maxRecipeValue;

        List<Item> yieldItems = new();
        foreach (IdIntPair item in recipe.yield)
        {
            var foundItem = Items.FindByID(item.objectID);
            yieldItems.Add(foundItem);
            if (foundItem == null)
            {
                Debug.Log($"{item.objectID} returned null. Check JSON data for recipe {recipe.objectID} yield.");
            }
        }

        recipe.rarity = yieldItems[0].rarity;

        recipe.basePrice = CalculatePrice(ref recipe, yieldItems[0]);

        if (string.IsNullOrEmpty(recipe.name))
        {
            recipe.name = yieldItems[0].name + " Recipe";
        }

        if (recipe.workload == 0)
        {
            recipe.SetWorkload();
        }

        ParseID(recipe);
        SetRequiredLevel(recipe);
    }

    public static void ParseID(Recipe recipe)
    {
        var dataArray = recipe.objectID.Split('-');
        var buySellData = dataArray[1];

        if (buySellData[0] == 'N')
        {
            recipe.notBuyable = true;
        }
        else if (buySellData[0] != 'B')
        {
            Debug.LogError($"{recipe.objectID} ID was not formatted correctly. Could not find N/B at index[0] after split");
        }
        if (buySellData[1] == 'N')
        {
            recipe.notResearchable = true;
        }
        else if (buySellData[1] != 'R')
        {
            Debug.LogError($"{recipe.objectID} ID was not formatted correctly. Could not find N/R at index[1] after split");
        }
    }

    public static void SetRequiredLevel(Recipe recipe)
    {
        if (recipe.rarity >= ItemRarity.Mythical)
        {
            recipe.requiredLevel = 9;
        }
        else if (recipe.rarity >= ItemRarity.Extraordinary)
        {
            recipe.requiredLevel = 7;
        }
        else if (recipe.rarity >= ItemRarity.Rare)
        {
            recipe.requiredLevel = 5;
        }
        else if (recipe.rarity >= ItemRarity.Uncommon)
        {
            recipe.requiredLevel = 3;
        }
        else
        {
            recipe.requiredLevel = 1;
        }
    }

    public static int CalculatePrice(ref Recipe recipe, Item yieldItem)
    {
        var itemPrice = MoneyExchange.CalculateSellPrice(yieldItem);

        return (100 * ((int)yieldItem.rarity + 2)) + (150 * ((int)yieldItem.type + 1)) + itemPrice;
    }
}
