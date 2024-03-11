using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class AlchemySelectedIngredients : MonoBehaviour
{
    public TextMeshProUGUI tipText;
    public GameObject materialContainer;
    public GameObject infusionContainer;
    public List<GameObject> prefabs = new();
    public List<ItemIntPair> selectedMaterials = new();
    public List<ItemIntPair> selectedInfusions = new();
    public List<AlchemyDraggableItem> draggableItems = new();

    private void OnEnable()
    {
        ClearList();
    }
    public void UpdateIngredient(AlchemyDraggableItem draggableItem, bool remove)
    {
        if (!remove)
        {
            draggableItems.Add(draggableItem);
        }
        else
        {
            var draggableEntry = draggableItems.FirstOrDefault(e => e == draggableItem);

            if (draggableEntry != null)
            {
                draggableItems.Remove(draggableEntry);
            }
        }

        BuildIngredientList();
    }

    public void UpdateItemContainer(AlchemyDraggableItem callerScript, GameObject draggableObject)
    {
        var alchemyMenu = callerScript.alchemyMenu;
        var matchingEntry = draggableItems.FirstOrDefault(e => e == callerScript);

        if (matchingEntry == null)
        {
            draggableItems.Add(callerScript);
        }

        // Sort similar objects
        foreach (GameObject ingredientPrefab in alchemyMenu.draggableIngredientPrefabs)
        {
            var prefabScript = ingredientPrefab.GetComponent<AlchemyDraggableItem>();

            if (prefabScript.item == callerScript.item)
            {
                ingredientPrefab.transform.SetParent(draggableObject.transform.parent.transform, false);
                prefabScript.isInfusion = callerScript.isInfusion;
            }
        }

        BuildIngredientList();
    }

    void BuildIngredientList()
    {
        //Debug.Log($"Building ingredient list with {draggableItems.Count} draggable items.");
        ClearList();

        foreach (var draggable in draggableItems)
        {
            if (draggable.isInfusion)
            {
                var existingEntry = selectedInfusions.FirstOrDefault(e =>  e.item == draggable.item);

                if (existingEntry != null)
                {
                    //Debug.Log("Item was infusion and not null.");
                    existingEntry.amount++;
                }
                else
                {
                    //Debug.Log("Item was infusion and returned null.");
                    selectedInfusions.Add(new ItemIntPair() { item = draggable.item, amount = 1 });
                }
            }
            else
            {
                var existingEntry = selectedMaterials.FirstOrDefault(e => e.item == draggable.item);

                if (existingEntry != null)
                {
                    //Debug.Log("Item was not infusion and not null.");
                    existingEntry.amount++;
                }
                else
                {
                    //Debug.Log("Item was not infusion and returned null.");
                    selectedMaterials.Add(new ItemIntPair() { item = draggable.item, amount = 1 });
                }
            }
        }

        ConfigureRendering();
    }

    void ConfigureRendering()
    {
        if (selectedInfusions.Count > 0)
        {
            tipText.gameObject.SetActive(false);
            infusionContainer.SetActive(true);
            RenderItemRows(selectedInfusions, infusionContainer);
        }
        else
        {
            infusionContainer.SetActive(false);
        }

        if (selectedMaterials.Count > 0)
        {
            tipText.gameObject.SetActive(false);
            materialContainer.SetActive(true);
            RenderItemRows(selectedMaterials, materialContainer);
        }
        else
        {
            materialContainer.SetActive(false);
        }

        if (selectedMaterials.Count == 0 && selectedInfusions.Count == 0)
        {
            tipText.gameObject.SetActive(true);
        }
    }
    public void ClearList()
    {
        foreach (GameObject prefab in prefabs)
        {
            Destroy(prefab);
        }

        prefabs = new();
        selectedInfusions.Clear();
        selectedMaterials.Clear();
        materialContainer.SetActive(false);
        infusionContainer.SetActive(false);
        tipText.gameObject.SetActive(true);
    }

    void RenderItemRows(List<ItemIntPair> ingredients, GameObject container)
    {
        foreach (ItemIntPair entry in ingredients)
        {
            SpawnItemRow(entry.item, entry.amount, container);
        }
    }

    void SpawnItemRow(Item item, int amount, GameObject container)
    {
        var newPrefab = BoxFactory.CreateItemRow(item, amount);
        newPrefab.name = item.name;

        newPrefab.transform.SetParent(container.transform, false);

        var scaleAnimator = newPrefab.GetComponent<Anim_ScaleOnEnable>();
        scaleAnimator.startScale = new Vector3(0.9f, 1, 1);

        prefabs.Add(newPrefab);

        materialContainer.SetActive(true);
        infusionContainer.SetActive(true);
        tipText.gameObject.SetActive(false);

        //Debug.Log($"Spawned prefab ({newPrefab.name}).");
    }
}
