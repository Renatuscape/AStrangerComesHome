using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlchemyInventoryItem : MonoBehaviour, IPointerClickHandler //, IInitializePotentialDragHandler, IDragHandler
{
    public ItemIconData itemIconData;
    public GameObject selectedEntryPrefab;
    public List<AlchemyIngredient> alchemyIngredients = new();
    public GameObject infusionContainer;
    public GameObject materialContainer;
    public GameObject inventoryContainer;
    public GameObject dragParent;
    public AlchemySelectedIngredients selectedIngredients;
    public bool isInfusion;
    public bool isMaterial;
    public int inventoryCount;
    public void Initialise(ItemIconData itemIconData, AlchemyMenu alchemyMenu)
    {
        this.itemIconData = itemIconData;
        selectedIngredients = alchemyMenu.selectedIngredients;
        infusionContainer = alchemyMenu.infusionContainer;
        materialContainer = alchemyMenu.materialContainer;
        inventoryContainer = alchemyMenu.pageinatedContainer.gameObject;
        dragParent = alchemyMenu.dragParent;

        inventoryCount = Player.GetCount(itemIconData.item.objectID, "Alchemy Inventory Item");
    }

    public void UseForCreation() //only called on create
    {
        Player.Remove(itemIconData.item.objectID, alchemyIngredients.Count);
        alchemyIngredients.Clear();
        UpdateNumbers();
    }

    public void UpdateNumbers()
    {
        if (selectedEntryPrefab != null)
        {
            Destroy(selectedEntryPrefab);
        }

        if (alchemyIngredients.Count > 0)
        {
            var newPrefab = BoxFactory.CreateItemRow(itemIconData.item, alchemyIngredients.Count);
            newPrefab.name = itemIconData.item.name;


            if (isInfusion)
            {
                newPrefab.transform.SetParent(selectedIngredients.infusionContainer.transform, false);
            }
            else
            {
                newPrefab.transform.SetParent(selectedIngredients.materialContainer.transform, false);
            }

            var scaleAnimator = newPrefab.GetComponent<Anim_ScaleOnEnable>();
            scaleAnimator.startScale = new Vector3(0.9f, 1, 1);

            selectedEntryPrefab = newPrefab;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventoryCount - alchemyIngredients.Count > 0)
        {
            if (AlchemyMenu.synthData.isSynthActive)
            {
                LogAlert.QueueTextAlert("A synthesis is already in progress.");
            }
            else
            {
                int ingredientTypes = selectedIngredients.infusionContainer.transform.childCount + selectedIngredients.materialContainer.transform.childCount;
                
                if (ingredientTypes >= 12)
                {
                    LogAlert.QueueTextAlert("Too many different types of ingredients on the table.");
                }
                else
                {
                    int ingredientNumber = infusionContainer.transform.childCount + materialContainer.transform.childCount;

                    if (ingredientNumber >= 60)
                    {
                        LogAlert.QueueTextAlert("There is no space for more ingredients.");
                    }
                    else
                    {
                        ItemIconData ingredient = BoxFactory.CreateItemIcon(itemIconData.item, false, 98);
                        AlchemyIngredient ingredientScript = ingredient.gameObject.AddComponent<AlchemyIngredient>();

                        ingredient.disableFloatText = true;
                        ingredient.gameObject.transform.SetParent(dragParent.transform);

                        alchemyIngredients.Add(ingredientScript);
                        ingredientScript.Initialise(this);
                        itemIconData.UpdateCount(inventoryCount - alchemyIngredients.Count);

                        UpdateNumbers();
                    }
                }
            }

        }
        else
        {
            LogAlert.QueueTextAlert("I don't have more of these.");
        }
    }

    public void ReturnItem(AlchemyIngredient alchemyIngredient)
    {
        alchemyIngredients.Remove(alchemyIngredient);
        itemIconData.UpdateCount(inventoryCount - alchemyIngredients.Count);

        alchemyIngredient.gameObject.GetComponent<ItemIconData>().Return("Ingredient collision fail");

        UpdateNumbers();
    }

    public void SetAllPrefabParents(GameObject parent)
    {
        if (parent == infusionContainer)
        {
            isInfusion = true;
            isMaterial = false;
        }
        else if (parent == materialContainer)
        {
            isMaterial = true;
            isInfusion = false;
        }

        foreach (var ingredient in alchemyIngredients)
        {
            ingredient.gameObject.transform.SetParent(parent.transform);
        }

        UpdateNumbers();
    }
}
