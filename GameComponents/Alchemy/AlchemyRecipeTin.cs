using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AlchemyRecipeTin : MonoBehaviour, IPointerClickHandler
{
    public GameObject lid;
    public GameObject tin;
    public GameObject recipeContainer;
    public GameObject recipeCardPrefab;
    public AlchemyPinnedRecipe pinnedRecipeCard;
    public Button nextRecipe;
    public Button previousRecipe;
    public List<GameObject> recipeCards;
    public List<Recipe> recipes;
    public int recipeIndex;
    public bool toggleOpen;
    private void OnEnable()
    {
        pinnedRecipeCard.gameObject.SetActive(false);
        Debug.Log($"Total of {Recipes.all.Count} recipes found in the game.");
        foreach (Recipe rx in Recipes.all)
        {
            recipes.Add(rx);
        }

        StockTin();
    }

    void StockTin()
    {
        int i = 0;

        foreach (Recipe rx in recipes)
        {
            var rxPrefab = Instantiate(recipeCardPrefab);
            recipeCards.Add(rxPrefab);
            rxPrefab.transform.SetParent(recipeContainer.transform);

            var rxScript = rxPrefab.GetComponent<AlchemyRecipePrefab>();

            rxScript.recipe = rx;
            rxScript.pinnedRecipeCard = pinnedRecipeCard;
            i++;

            rxPrefab.name = rx.name;

            if (i >= 10)
            {
                break;
            }
        }
    }
    private void OnDisable()
    {
        EmptyTin();
    }

    void EmptyTin()
    {
        foreach (GameObject prefab in recipeCards)
        {
            Destroy(prefab);
        }
        recipeCards = new();
        recipeIndex = 0;
        recipes = new();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (toggleOpen)
        {
            lid.transform.localPosition = new Vector3(0, 0, 0);
            tin.transform.localPosition = new Vector3(0, 0, 0);
            toggleOpen = false;
        }
        else
        {
            lid.transform.localPosition = new Vector3(0, 140, 0);
            tin.transform.localPosition = new Vector3(0, -350, 0);
            toggleOpen = true;
        }
    }
}
