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
        ClearPrefabs();
        gameObject.SetActive(true);
        recipeTitle.text = recipe.name;

        foreach (IdIntPair ingredient in recipe.ingredients)
        {
            var prefab = BoxFactory.CreateItemRow(Items.FindByID(ingredient.objectID), ingredient.amount);
            prefab.transform.SetParent(prefabContainer.transform, false);
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

        prefabs.Clear();
    }

    private void OnDisable()
    {
        ClearPrefabs();
    }
}
