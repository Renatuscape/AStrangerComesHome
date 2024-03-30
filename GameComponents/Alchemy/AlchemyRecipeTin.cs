using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<GameObject> recipeCardPrefabs;
    public List<Recipe> recipes;
    public int recipeIndex;
    public bool toggleOpen;
    public bool isStocked;

    private void Start()
    {
        nextRecipe.onClick.AddListener(() => NextRecipe());
    }
    private void OnEnable()
    {
        isStocked = false;
        pinnedRecipeCard.gameObject.SetActive(false);
        Debug.Log($"Total of {Recipes.all.Count} recipes found in the game.");

        foreach (Recipe rx in Recipes.all.Where(r => !r.hidden))
        {
            recipes.Add(rx);
        }

        recipes.OrderBy(rx => rx.name);
        //StockTin();
    }
    private void OnDisable()
    {
        EmptyTin();
    }

    void StockTin()
    {
        isStocked = true;
        recipeIndex = 0;
        float spawnDelay = 0;

        foreach (Recipe rx in recipes)
        {
            StartCoroutine(InstantiateRecipePrefab(rx, spawnDelay));
            spawnDelay += 0.2f;

            if (recipeIndex >= 8)
            {
                break;
            }
        }

        if (recipes.Count <= 8)
        {
            nextRecipe.gameObject.SetActive(false);
        }
        else
        {
            nextRecipe.gameObject.SetActive(true);
        }
    }

    void EmptyTin()
    {
        lid.transform.localPosition = new Vector3(0, 0, 0);
        tin.transform.localPosition = new Vector3(0, 0, 0);
        toggleOpen = false;

        foreach (GameObject prefab in recipeCardPrefabs)
        {
            Destroy(prefab);
        }

        recipeCardPrefabs.Clear();
        recipeIndex = 0;
        recipes.Clear();
        isStocked = false;
    }

    IEnumerator InstantiateRecipePrefab(Recipe rx, float delay)
    {
        yield return new WaitForSeconds(delay);

        var rxPrefab = Instantiate(recipeCardPrefab);
        recipeCardPrefabs.Add(rxPrefab);
        rxPrefab.transform.SetParent(recipeContainer.transform);

        var rxScript = rxPrefab.GetComponent<AlchemyRecipePrefab>();

        rxScript.recipe = rx;
        rxScript.pinnedRecipeCard = pinnedRecipeCard;
        recipeIndex++;

        rxPrefab.name = rx.name;
        AudioManager.PlayUISound("draw");
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

            if (!isStocked)
            {
                StockTin();
            }
        }
    }

    public void NextRecipe()
    {
        GameObject firstChild = recipeContainer.transform.GetChild(recipeContainer.transform.childCount - 1).gameObject;

        if (recipeCardPrefabs.Contains(firstChild))
        {
            Debug.Log($"{firstChild.name} was found in prefab list.");
            recipeCardPrefabs.Remove(firstChild);
            Destroy(firstChild);
        }
        else
        {
            Debug.Log($"{firstChild.name} was not found in prefab list.");
        }

        if (recipeIndex >= recipes.Count)
        {
            recipeIndex = 0;
        }

        InstantiateRecipePrefab(recipes[recipeIndex], 0);
    }
}
