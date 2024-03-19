using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Ports;

public class RecipeManager : MonoBehaviour
{
    public List<Recipe> debugItemList = Recipes.all;
    public bool allObjecctsLoaded = false;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad = 1;

    void Start()
    {
        LoadFromJson();
    }

    [System.Serializable]
    public class ItemsWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Recipe[] recipes;
    }

    public void LoadFromJson()
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Recipes/Recipes.json";

        if (File.Exists(jsonPath))
        {
            string jsonData = File.ReadAllText(jsonPath);
            ItemsWrapper dataWrapper = JsonUtility.FromJson<ItemsWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.recipes != null)
                {
                    foreach (Recipe recipe in dataWrapper.recipes)
                    {
                        InitialiseRecipe(recipe, Recipes.all);
                    }
                    filesLoaded++;
                    if (filesLoaded == numberOfFilesToLoad)
                    {
                        allObjecctsLoaded = true;
                        Debug.Log("All ITEMS successfully loaded from Json.");
                    }
                }
                else
                {
                    Debug.LogError("Recipes array is null in JSON data. Check that the list has a wrapper with the \'recipes\' tag and that the object class is serializable.");
                }
            }
            else
            {
                Debug.LogError("JSON data is malformed. No wrapper found?");
                Debug.Log(jsonData); // Log the JSON data for inspection
            }
        }
        else
        {
            Debug.LogError("JSON file not found: " + jsonPath);
        }
    }

    public static void InitialiseRecipe(Recipe recipe, List<Recipe> recipeList)
    {
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
            var rarityModifier = (int)recipe.rarity + 6;

            recipe.workload = recipe.baseWorkload * rarityModifier * rarityModifier * recipe.yield[0].amount;
        }

        ParseID(recipe);
        SetRequiredLevel(recipe);

        recipeList.Add(recipe);
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
        int rarityMultiplier = 1;

        if ((int)yieldItem.rarity+1 >= 0)
        {
            rarityMultiplier = (int)yieldItem.rarity + 1;
        }

        return yieldItem.basePrice * recipe.yield[0].amount * 10 * rarityMultiplier;
    }
}
