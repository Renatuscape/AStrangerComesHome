using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public string objectID;
    public string name; // custom or set by yield item
    public ItemRarity rarity; //set by yield item

    public int maxStack = 1;
    public int workload;
    public int manaDrainRate = 1;
    public int basePrice;
    public bool notBuyable;
    public bool notResearchable;

    public IdIntPair yield;
    public List<IdIntPair> ingredients;

    public bool CheckIngredients(List<IdIntPair> ingredientsIn)
    {
        // Check if items exist in player inventory
        foreach (IdIntPair entry in ingredientsIn)
        {
            int amount = Player.GetCount(entry.objectID, name);

            if (amount < entry.amount)
            {
                return false;
            }
        }

        // Create hash sets for this recipe's ingredients and the other collection's ingredients
        HashSet<IdIntPair> ingredientsInSet = new HashSet<IdIntPair>(ingredientsIn);
        HashSet<IdIntPair> recipeIngredientsSet = new HashSet<IdIntPair>(ingredients);

        // Check if the hash sets are equal (i.e., contain the same elements)
        return ingredientsInSet.SetEquals(recipeIngredientsSet);
    }

    public bool PerformAlchemy(List<IdIntPair> ingredientsIn, out IdIntPair result)
    {
        result = null;

        if (CheckIngredients(ingredientsIn))
        {
            foreach (IdIntPair entry in ingredientsIn)
            {
                Player.Remove(entry.objectID, entry.amount);
            }

            Player.Add(yield.objectID, yield.amount);
            result = yield;
            return true;
        }
        else
        {
            return false;
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

    public static IdIntPair AttemptExperiment(List<IdIntPair> ingredientsIn, bool performAlchemy)
    {
        List<Recipe> viableRecipes = Recipes.all.Where(r => !r.notResearchable).ToList();

        Recipe matchingRecipe = null;
        foreach (Recipe recipe in viableRecipes)
        {
            if (recipe.CheckIngredients(ingredientsIn))
            {
                {
                    matchingRecipe = recipe; break;
                }
            }
        }

        if (matchingRecipe != null)
        {
            if (performAlchemy)
            {
                matchingRecipe.PerformAlchemy(ingredientsIn, out var yield);
            }

            return matchingRecipe.yield;
        }
        else
        {
            return new IdIntPair() { objectID = "MAT030-JUN-NN", amount = 1 };
        }
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

            recipeIngredients.Add(new() { item = item, count = entry.amount });
        }

        ItemIntPair catalystIn = ingredientsIn.Where(entry => entry.item.type == ItemType.Catalyst).FirstOrDefault();
        ItemIntPair catalystRecipe = recipeIngredients.Where(entry => entry.item.type == ItemType.Catalyst).FirstOrDefault();

        ItemIntPair plantIn = ingredientsIn.Where(entry => entry.item.type == ItemType.Plant).FirstOrDefault();
        ItemIntPair plantRecipe = recipeIngredients.Where(entry => entry.item.type == ItemType.Plant).FirstOrDefault();

        var materialIngredientsIn = ingredientsIn.Where(i => i.item.type != ItemType.Catalyst && i.item.type != ItemType.Plant).ToList();
        var materialIngredientsRecipe = recipeIngredients.Where(i => i.item.type != ItemType.Catalyst && i.item.type != ItemType.Plant).ToList();

        if (catalystIn != null && catalystIn.item.objectID == catalystRecipe.item.objectID)
        {

            if (catalystIn.count > catalystRecipe.count)
            {
                alchemyLog.Add("The amount of catalyst is too high.");
            }
            else if (catalystIn.count < catalystRecipe.count)
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
                if (plantIn.count > plantRecipe.count)
                {
                    alchemyLog.Add("I need less plant matter for the infusion.");
                }
                else if (plantIn.count < plantRecipe.count)
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

                if (material.count > recipeMatch.count)
                {
                    alchemyLog.Add($"There was too much of one material.");
                }
                else if (material.count < recipeMatch.count)
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