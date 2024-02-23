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
}