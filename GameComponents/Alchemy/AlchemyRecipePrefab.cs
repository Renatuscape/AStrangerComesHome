using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class AlchemyRecipePrefab : MonoBehaviour, IPointerClickHandler
{
    public Recipe recipe;
    public TextMeshProUGUI recipeTitle;
    public Anim_BobLoop bobAnimation;
    public GameObject recipeContent;
    public Action<Recipe> pinRecipe;
    public float xOffset;

    void Start()
    {
        recipeTitle.text = recipe.name.Replace(" Recipe", "");
        xOffset = Random.Range(-7, 7);
        BringToBack();
    }

    void OffsetContent(float offset)
    {
        recipeContent.transform.position = new Vector3(offset, recipeContent.transform.position.y, 0);
    }

    public void BringToFront()
    {
        transform.SetAsLastSibling();
        OffsetContent(0);
        bobAnimation.enabled = true;
    }

    public void BringToBack()
    {
        transform.SetAsFirstSibling();
        OffsetContent(xOffset);
        bobAnimation.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        pinRecipe(recipe);
    }
}
