using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
    public SynthesiserData synthData;
    public AlchemySelectedIngredients selectedIngredients;
    public AlchemyInventory inventory;
    public SynthesiserType synthesiserType;

    public List<GameObject> draggableIngredientPrefabs = new();

    public GameObject tableContainer;
    public GameObject infusionContainer;
    public GameObject dragParent;
    //public GameObject inventoryPage;

    public List<ItemIntPair> availableIngredients = new();

    bool containersEnabled;
    public bool isDebugging = true;

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
        this.synthesiserType = synthesiserType;

        string name = synthesiserType.ToString();

        if (dataManager == null)
        {
            dataManager = TransientDataCalls.gameManager.dataManager;
        }

        synthData = dataManager.alchemySynthesisers.FirstOrDefault(s => s.synthesiserID == name);

        if (synthData == null)
        {
            synthData = new() { synthesiserID = name };
            if (synthData.synthesiserID.ToLower().Contains("coach"))
            {
                synthData.consumesMana = true;
            }
            dataManager.alchemySynthesisers.Add(synthData);
        }

        availableIngredients = inventory.GetIngredientsInInventory(isDebugging);
        inventory.RenderInventory(ItemType.Catalyst, false, isDebugging);
        gameObject.SetActive(true);
        TransientDataCalls.SetGameState(GameState.Dialogue, name, gameObject);
    }
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
            //Debug.Log("Found entry. Attempting to update");

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
        var prefab = inventory.inventoryPrefabs.FirstOrDefault(prefab => prefab.GetComponent<AlchemyInventoryItem>().item == item);

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
}
