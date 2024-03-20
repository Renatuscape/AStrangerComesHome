using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public string objectID;
    public string name; // custom or set by yield item
    public string description;
    public ItemRarity rarity; //set by yield item
    public int baseWorkload = 10; // 10 is default. This is multiplied by type and rarity. Reduce or increase in JSON only when an item deviates from standard.

    public int maxStack = 1;
    public int workload;
    public int manaDrainRate = 1;
    public int basePrice;
    public int requiredLevel;
    public bool notBuyable;
    public bool notResearchable;
    public bool hidden = false; // Will never be displayed in any recipe list

    public List<IdIntPair> yield = new();
    public List<IdIntPair> ingredients;

    public bool CheckCraftability(int alchemySkill)
    {
        Debug.Log($"Checking craftability of {objectID}");
        bool isUnlocked = Player.GetCount(objectID, objectID) > 0;

        if (!hidden && (!notResearchable || isUnlocked))
        {
            {
                if (requiredLevel > alchemySkill)
                {
                    Debug.Log("Required alchemy level was too low");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        else
        {
            if (hidden) {
                Debug.Log("Recipe was hidden");
            }
            if (notResearchable && !isUnlocked)
            {
                Debug.Log("Recipe is not researchable and was not unlocked");
            }
            Debug.Log("Recipe is hidden from discovery");
            return false;
        }
    }
    public bool CheckIngredients(List<IdIntPair> ingredientsIn)
    {
        if (ingredientsIn.Count != ingredients.Count) // ensures no extra ingredients are present and stops early
        {
            return false;
        }

        foreach (IdIntPair recipeEntry in ingredients)
        {
            var match = ingredientsIn.FirstOrDefault(inputEntry => inputEntry.objectID == recipeEntry.objectID && inputEntry.amount == recipeEntry.amount);

            if (match == null)
            {
                return false;
            }
        }

        return true;
    }

    public void SetWorkload()
    {
        if (yield != null && yield.Count > 0 && yield[0] != null)
        {
            int rarityModifier = (int)rarity + 6;
            int typeModifier = 1;
            Item yieldItem = Items.FindByID(yield[0].objectID);

            if (yieldItem != null)
            {
                typeModifier+= (int)yieldItem.type;
            }

            workload = (baseWorkload + rarityModifier + yield[0].amount) * typeModifier;
        }
        else
        {
            Debug.Log($"yield for {objectID} {name} was null.");
        }
    }

    public void AddYieldToPlayer()
    {
        foreach (var entry in yield)
        {
            Player.Add(entry.objectID, entry.amount);
        }
    }
}

public static class Recipes
{
    public static List<Recipe> all = new();

    public static void DebugAllRecipes()
    {
        foreach (Recipe recipe in all)
        {
            Player.Add(recipe.objectID, 1);
            Player.Add(recipe.objectID, 5);
        }
    }

    public static Recipe FindByID(string objectID)
    {
        return all.FirstOrDefault(r => r.objectID == objectID);
    }

    public static Recipe AttemptAlchemy(List<IdIntPair> ingredientsIn, out bool isSuccess, bool isDebugging = false)
    {
        int alchemySkill = Player.GetCount("ALC000", "Recipes");

        if (isDebugging)
        {
            Debug.Log("Alchemy skill treated as 10 for debugging purposes.");
            alchemySkill = 10;
        }

        foreach (Recipe rx in  all)
        {
            if (rx.CheckCraftability(alchemySkill))
            {
                if (rx.CheckIngredients(ingredientsIn))
                {
                    isSuccess = true;
                    return rx;
                }
            }
        }

        isSuccess = false;
        return FindByID("REC001-NN");
    }

    public static List<string> AttemptSecretAlchemy(Recipe attemptedRecipe, List<ItemIntPair> ingredientsIn)
    {
        List<string> alchemyLog = new();
        List<ItemIntPair> recipeIngredients = new List<ItemIntPair>();
        bool isAcolyte = false;
        bool isJourneyman = false;
        bool isExpert = false;
        bool isMaster = false;
        int secretAlchemy = Player.GetCount("ALC001", "Recipes, AttemptSecretAlchemy()");

        if (secretAlchemy >= 8)
        {
            isMaster = true;
        }
        if (secretAlchemy >= 6)
        {
            isExpert = true;
        }
        if (secretAlchemy >= 4)
        {
            isJourneyman = true;
        }
        if (secretAlchemy >= 2)
        {
            isAcolyte = true;
        }

        foreach (var entry in attemptedRecipe.ingredients)
        {
            Item item = Items.FindByID(entry.objectID);

            recipeIngredients.Add(new() { item = item, amount = entry.amount });
        }

        ItemIntPair catalystIn = ingredientsIn.Where(entry => entry.item.type == ItemType.Catalyst).FirstOrDefault();
        ItemIntPair catalystRecipe = recipeIngredients.Where(entry => entry.item.type == ItemType.Catalyst).FirstOrDefault();

        ItemIntPair plantIn = ingredientsIn.Where(entry => entry.item.type == ItemType.Plant).FirstOrDefault();
        ItemIntPair plantRecipe = recipeIngredients.Where(entry => entry.item.type == ItemType.Plant).FirstOrDefault();

        var materialIngredientsIn = ingredientsIn.Where(i => i.item.type != ItemType.Catalyst && i.item.type != ItemType.Plant).ToList();
        var materialIngredientsRecipe = recipeIngredients.Where(i => i.item.type != ItemType.Catalyst && i.item.type != ItemType.Plant).ToList();

        if (catalystIn != null && catalystIn.item.objectID == catalystRecipe.item.objectID)
        {

            if (catalystIn.amount > catalystRecipe.amount)
            {
                alchemyLog.Add("The amount of catalyst is too high.");
            }
            else if (catalystIn.amount < catalystRecipe.amount)
            {
                alchemyLog.Add("The amount of catalyst is too low.");
            }
        }
        else
        {
            alchemyLog.Add("The catalyst appears to be incorrect.");
        }

        if (plantIn != null && plantIn.item.objectID == plantRecipe.item.objectID)
        {
            if (isAcolyte)
            {
                if (plantIn.amount > plantRecipe.amount)
                {
                    alchemyLog.Add("I need less plant matter for the infusion.");
                }
                else if (plantIn.amount < plantRecipe.amount)
                {
                    alchemyLog.Add("I need more plant matter for the infusion.");
                }
            }
        }
        else
        {
            alchemyLog.Add("The choice of plant for the infusion appears to be incorrect.");
        }

        if (isAcolyte && !isMaster)
        {
            foreach (var material in materialIngredientsIn)
            {
                if (!materialIngredientsRecipe.Contains(material))
                {
                    alchemyLog.Add("At least one ingredient does not match the recipe.");
                    break;
                }
            }
        }

        if (isJourneyman)
        {
            if (ingredientsIn.Count > recipeIngredients.Count)
            {
                alchemyLog.Add("There are too many types of ingredients.");
            }
            else if (ingredientsIn.Count < recipeIngredients.Count)
            {
                alchemyLog.Add("There are too few types of ingredients.");
            }

            foreach (var material in materialIngredientsIn)
            {
                if (materialIngredientsRecipe.Contains(material))
                {
                    alchemyLog.Add("At least one ingredient matches the recipe.");
                    break;
                }
            }
        }

        if (isExpert)
        {

            List<ItemType> wrongTypes = new();

            foreach (var material in materialIngredientsIn)
            {
                var type = material.item.type;

                if (materialIngredientsRecipe.Where(m => m.item.type == type).ToList().Count == 0)
                {
                    wrongTypes.Add(type);
                }
            }

            foreach (var type in wrongTypes)
            {
                Debug.Log($"This recipe does not require any materials of type {type}.");
            }
        }

        if (isMaster)
        {
            int count = 0;
            List<ItemIntPair> matchedMaterial = new();

            foreach (var material in materialIngredientsIn)
            {
                if (!materialIngredientsRecipe.Contains(material))
                {
                    count++;
                }
                else
                {
                    matchedMaterial.Add(material);
                }
            }
            if (count == 1)
            {
                alchemyLog.Add($"{count} material did not match.");
            }
            if (count > 1)
            {
                alchemyLog.Add($"{count} materials were not a match.");
            }

            foreach (var material in matchedMaterial)
            {
                var recipeMatch = materialIngredientsRecipe.Where(m => m.item.objectID == material.item.objectID).FirstOrDefault();

                if (material.amount > recipeMatch.amount)
                {
                    alchemyLog.Add($"There was too much of one material.");
                }
                else if (material.amount < recipeMatch.amount)
                {
                    alchemyLog.Add($"There was too little of one material.");
                }
            }
        }

        if (alchemyLog.Count == 0)
        {
            alchemyLog.Add($"I found no mistakes in the recipe. Either the formula is correct, or I am not skilled enough at Secret Alchemy.");
        }
        return alchemyLog;
    }
}