using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlchemyPinnedRecipe : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private RectTransform dragRectTransform;
    public TextMeshProUGUI recipeTitle;
    public GameObject prefabContainer;
    public List<GameObject> prefabs;

    public void Initialise(Recipe recipe)
    {
        gameObject.SetActive(false);
        Debug.Log($"Initialised pinned card with recipe {recipe.objectID}. Ingredient list has {recipe.ingredients.Count} entries.");
        ClearPrefabs();
        gameObject.SetActive(true);
        recipeTitle.text = recipe.name.Replace(" Recipe", "");

        foreach (IdIntPair ingredient in recipe.ingredients)
        {
            var item = Items.FindByID(ingredient.objectID);

            if (item != null)
            {
                var prefab = BoxFactory.CreateItemRowPlainText(item, ingredient.amount, true);
                prefab.transform.SetParent(prefabContainer.transform, false);
                prefabs.Add(prefab);
            }
            else
            {
                Debug.Log($"Searched for {ingredient.objectID}, but return was null.");
            }

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragRectTransform.anchoredPosition += eventData.delta;
    }

    public void CloseCard()
    {
        ClearPrefabs();
        gameObject.SetActive(false);
    }

    void ClearPrefabs()
    {
        foreach (var prefab in prefabs)
        {
            Destroy(prefab);
        }

        prefabs = new();
    }

    private void OnDisable()
    {
        ClearPrefabs();
    }
}
