using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class AlchemyRecipePrefab : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Recipe recipe;
    public TextMeshProUGUI recipeTitle;
    public GameObject recipeContent;
    public AlchemyPinnedRecipe pinnedRecipeCard;
    public float xOffset;

    private void Start()
    {
        recipeTitle.text = recipe.name.Replace(" Recipe", "");
        xOffset = Random.Range(-7, 7);
        BringToBack();
    }

    void OffsetContent(float offset)
    {
        recipeContent.transform.localPosition = new Vector3(offset, recipeContent.transform.position.y, 0);
    }

    public void BringToFront()
    {
        transform.SetAsLastSibling();
        OffsetContent(0);
    }

    public void BringToBack()
    {
        transform.SetAsFirstSibling();
        OffsetContent(xOffset);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        pinnedRecipeCard.gameObject.transform.position = Input.mousePosition;
        pinnedRecipeCard.Initialise(recipe);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        recipeContent.transform.localPosition = new Vector3(recipeContent.transform.localPosition.x, -40, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        recipeContent.transform.localPosition = new Vector3(recipeContent.transform.localPosition.x, -70, 0);
    }
}
