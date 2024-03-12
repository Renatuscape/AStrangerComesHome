using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
                        InitialiseItem(recipe, Recipes.all);
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

    public static void InitialiseItem(Recipe recipe, List<Recipe> recipeList)
    {
        var yieldItem = Items.FindByID(recipe.yield.objectID);

        recipe.rarity = yieldItem.rarity;

        recipe.basePrice = CalculatePrice(ref recipe, yieldItem);

        if (string.IsNullOrEmpty(recipe.name))
        {
            recipe.name = yieldItem.name + " Recipe";
        }

        if (recipe.workload == 0)
        {
            recipe.workload = 100 * ((int)recipe.rarity + 1);
        }

        ParseID(recipe);

        recipeList.Add(recipe);
    }

    public static void ParseID(Recipe recipe)
    {
        var dataArray = recipe.objectID.Split('-');
        var rarityData = dataArray[1];
        var buySellData = dataArray[2];

        recipe.rarity = Items.GetItemRarity(rarityData);

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

    public static int CalculatePrice(ref Recipe recipe, Item yieldItem)
    {
        int rarityMultiplier = 1;

        if ((int)yieldItem.rarity >= 0)
        {
            rarityMultiplier = (int)yieldItem.rarity + 1;
        }

        return yieldItem.basePrice * recipe.yield.amount * 10 * rarityMultiplier;
    }
}
