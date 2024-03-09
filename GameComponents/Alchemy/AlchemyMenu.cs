using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public enum SynthesiserType
{
    Stella,
    Capital,
    Magus,
    Home,
    Coach,
    Coach2,
    Coach3
}
public class AlchemyMenu : MonoBehaviour
{
    public DataManagerScript dataManager;
    public AlchemySelectedIngredients selectedIngredients;

    public List<GameObject> draggableIngredientPrefabs = new();
    public List<GameObject> inventoryPrefabs = new();

    public GameObject tableContainer;
    public GameObject infusionContainer;
    public GameObject dragParent;
    public GameObject inventoryPage;

    public List<ItemIntPair> availableIngredients = new();

    bool containersEnabled;
    bool isDebugging = true;

    private void Start()
    {
        dataManager = TransientDataCalls.gameManager.dataManager;
        SetUpContainers();
    }

    private void OnEnable()
    {
        if (isDebugging)
        {
            Initialise(SynthesiserType.Stella);
        }
    }

    public void Initialise(SynthesiserType synthesiserType)
    {
        string name = synthesiserType.ToString();

        if (dataManager == null)
        {
            dataManager = TransientDataCalls.gameManager.dataManager;
        }

        var synthesiserData = dataManager.alchemySynthesisers.FirstOrDefault(s => s.synthesiserID == name);

        if (synthesiserData == null)
        {
            synthesiserData = new() { synthesiserID = name };
            dataManager.alchemySynthesisers.Add(synthesiserData);
        }

        availableIngredients = GetIngredientsInInventory();
        RenderInventory(ItemType.Catalyst);
        gameObject.SetActive(true);
    }

    #region Initial Setup
    void SetUpContainers()
    {
        if (!containersEnabled)
        {
            tableContainer.AddComponent<AlchemyContainer>();
            tableContainer.GetComponent<AlchemyContainer>().itemLimit = 40;
            tableContainer.GetComponent<AlchemyContainer>().reverseAnimation = true;
            tableContainer.AddComponent<RadialLayout>();
            var tableLayout = tableContainer.GetComponent<RadialLayout>();
            tableLayout.MinAngle = 360;
            tableLayout.fDistance = 280;

            infusionContainer.AddComponent<AlchemyContainer>();
            infusionContainer.GetComponent<AlchemyContainer>().itemLimit = 20;
            infusionContainer.AddComponent<RadialLayout>();
            var infusionLayout = infusionContainer.GetComponent<RadialLayout>();
            infusionLayout.MinAngle = 360;
            infusionLayout.fDistance = 110;

            containersEnabled = true;
        }
    }

    public List<ItemIntPair> GetIngredientsInInventory()  //call only once at Initialisation or on Clear, and edit availableIngredients when manipulating materials.
    {
        List<ItemIntPair> availableIngredients = new();

        foreach (var item in Items.all) // exclude seeds, misc, scripts and books, and any unique item
        {
            if (item.type == ItemType.Treasure
            || item.type == ItemType.Plant
            || item.type == ItemType.Trade
            || item.type == ItemType.Catalyst
            || item.type == ItemType.Material)
            {
                if (item.rarity != ItemRarity.Unique)
                {

                    if (isDebugging)
                    {
                        availableIngredients.Add(new() { item = item, amount = 5 });
                    }
                    else
                    {
                        int amount = item.GetCountPlayer();

                        if (amount > 0)
                        {
                            availableIngredients.Add(new() { item = item, amount = amount });
                        }
                    }
                }
            }
        }

        return availableIngredients;
    }

    public void RenderInventory(ItemType pageType, bool printAll = false)
    {
        foreach (var prefab in inventoryPrefabs)
        {
            Destroy(prefab);
        }

        inventoryPrefabs = new();

        foreach (var entry in availableIngredients)
        {
            if (entry.amount > 0)
            {
                if (entry.item.type == pageType || printAll || isDebugging)
                {
                    var prefab = BoxFactory.CreateItemIcon(entry.item, true, 64);
                    prefab.name = entry.item.name;
                    prefab.transform.SetParent(inventoryPage.transform, false);
                    prefab.AddComponent<AlchemyInventoryItem>();
                    var script = prefab.GetComponent<AlchemyInventoryItem>();
                    script.alchemyMenu = this;
                    script.item = entry.item;

                    var tag = prefab.transform.Find("Tag").gameObject;

                    TextMeshProUGUI text = tag.transform.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = $"{entry.amount}";

                    inventoryPrefabs.Add(prefab);
                }
            }
        }
    }
    #endregion

    public GameObject DragItemFromInventory(Item item)
    {
        int maxTypes = 10;
        int maxAmount = 60;
        int uniqueEntries = selectedIngredients.selectedInfusions.Count + selectedIngredients.selectedMaterials.Count;
        int totalEntries = selectedIngredients.draggableItems.Count;


        if (totalEntries >= maxAmount)
        {
            TransientDataCalls.PushAlert("Too many ingredients!");
            Debug.Log("Could not spawn item. Too many were already on the table.");
            return null;
        }
        else if (uniqueEntries >= maxTypes)
        {
            var match = selectedIngredients.draggableItems.FirstOrDefault(d => d.item == item);

            if (match != null && totalEntries < maxAmount)
            {
                return SpawnDraggableItem(item);
            }
            else
            {
                TransientDataCalls.PushAlert("Too many types of ingredients!");
                Debug.Log("Could not spawn item. Too many were already on the table.");
                return null;
            }
        }
        else if (uniqueEntries < maxTypes && totalEntries < maxAmount)
        {
            return SpawnDraggableItem(item);
        }

        return null;
    }

    GameObject SpawnDraggableItem(Item item)
    {
        if (item == null)
        {
            Debug.Log("Could not spawn item. It was null.");
            return null;
        }
        else
        {
            var prefab = BoxFactory.CreateItemIcon(item, false, 96);
            prefab.transform.SetParent(dragParent.transform, false);
            prefab.name = item.name;
            prefab.AddComponent<AlchemyDraggableItem>();

            var script = prefab.GetComponent<AlchemyDraggableItem>();
            script.item = item;
            script.dragParent = dragParent;
            script.alchemyMenu = this;
            draggableIngredientPrefabs.Add(prefab);

            var matchingEntryInInventory = availableIngredients.FirstOrDefault(entry => entry.item == item);

            if (matchingEntryInInventory != null)
            {
                matchingEntryInInventory.amount--;
            }
            else
            {
                Debug.Log("Something went wrong when removing the item.");
            }

            UpdateInventoryItemNumber(script.item);
            return prefab;
        }
    }

    public void ReturnIngredientToInventory(GameObject draggableCallerObject)
    {
        GameObject draggableIngredient = draggableIngredientPrefabs.FirstOrDefault(obj => obj == draggableCallerObject);
        var script = draggableIngredient.GetComponent<AlchemyDraggableItem>();
        ItemIntPair matchingInventoryEntry = availableIngredients.FirstOrDefault(entry => entry.item == script.item);

        if (draggableIngredient != null)
        {
            Debug.Log("Found entry. Attempting to update");

            draggableIngredientPrefabs.Remove(draggableIngredient);


            if (script.item == null)
            {
                Debug.LogWarning("Item in prefab's AlchemyDraggableItem script was null. Something went wrong.");
            }
            else
            {
                if (matchingInventoryEntry != null)
                {
                    matchingInventoryEntry.amount++;
                }
                else
                {
                    Debug.Log("Something went wrong when returning the item.");
                }

                UpdateInventoryItemNumber(script.item);
                selectedIngredients.UpdateIngredient(script, true);
            }

            Destroy(draggableIngredient);
        }
    }

    void UpdateInventoryItemNumber(Item item)
    {
        var entry = availableIngredients.FirstOrDefault(entry => entry.item.objectID == item.objectID);
        var prefab = inventoryPrefabs.FirstOrDefault(prefab => prefab.GetComponent<AlchemyInventoryItem>().item == item);

        if (prefab != null)
        {
            if (entry.amount > 0)
            {
                var tag = prefab.transform.Find("Tag").gameObject;

                TextMeshProUGUI text = tag.transform.GetComponentInChildren<TextMeshProUGUI>();
                text.text = $"{entry.amount}";
                prefab.SetActive(true);
            }
            else
            {
                prefab.SetActive(false);
            }
        }
        else
        {
            Debug.Log($"Could not find prefab for {item}.");
        }
    }


    public void CancelAlchemy()
    {

    }

    public void ClearTable()
    {
        for (int i = draggableIngredientPrefabs.Count - 1; i >= 0; i--)
        {
            float time = 0.2f * i;
            if (time > 1)
            {
                time = time / 2;
            }
            if (time > 2)
            {
                time = 0.9f;
            }
            StartCoroutine(ClearTableInStyle(draggableIngredientPrefabs[i], time));
        }
    }

    IEnumerator ClearTableInStyle(GameObject prefab, float duration)
    {
        prefab.transform.SetParent(dragParent.transform);
        Vector3 targetLocation = inventoryPage.transform.position;
        Vector3 startPosition = prefab.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            prefab.transform.position = Vector3.Lerp(startPosition, targetLocation, t);
            yield return null;
        }

        // Ensure the object is exactly at the target position
        prefab.transform.position = targetLocation;

        ReturnIngredientToInventory(prefab);
    }

    public void Leave()
    {
        gameObject.SetActive(false);
        TransientDataCalls.SetGameState(GameState.Overworld, name, gameObject);
    }
}