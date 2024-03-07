using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.EventSystems.EventTrigger;

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
    public ItemIntPair selectedCatalyst;
    public ItemIntPair selectedPlant;
    public List<ItemIntPair> selectedMaterials;

    public List<GameObject> ingredientPrefabs = new();
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
            tableLayout.fDistance = 264;

            infusionContainer.AddComponent<AlchemyContainer>();
            infusionContainer.GetComponent<AlchemyContainer>().itemLimit = 20;
            infusionContainer.AddComponent<RadialLayout>();
            var infusionLayout = infusionContainer.GetComponent<RadialLayout>();
            infusionLayout.MinAngle = 360;
            infusionLayout.fDistance = 110;

            containersEnabled = true;
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

    public void UpdateInventoryItem(Item item)
    {
        var entry = availableIngredients.FirstOrDefault(entry => entry.item.objectID == item.objectID);
        var prefab = inventoryPrefabs.FirstOrDefault(prefab => prefab.GetComponent<AlchemyInventoryItem>().item == item);

        if (entry.amount > 0)
        {
            if (prefab == null)
            {
                prefab = BoxFactory.CreateItemIcon(entry.item, true, 64);
                prefab.name = entry.item.name;
                prefab.transform.SetParent(inventoryPage.transform, false);
                prefab.AddComponent<AlchemyInventoryItem>();

                var script = prefab.GetComponent<AlchemyInventoryItem>();
                script.alchemyMenu = this;
                script.item = entry.item;

                inventoryPrefabs.Add(prefab);
            }

            var tag = prefab.transform.Find("Tag").gameObject;

            TextMeshProUGUI text = tag.transform.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"{entry.amount}";
        }
        else
        {
            inventoryPrefabs.Remove(prefab);
            Destroy(prefab);
        }
    }

    public GameObject SpawnDraggableItem(Item item)
    {
        if (item == null)
        {
            Debug.Log("Could not spawn item. It was null.");
            return null;
        }
        else
        {
            var prefab = BoxFactory.CreateItemIcon(item, false, 64);
            prefab.transform.SetParent(dragParent.transform, false);
            prefab.name = item.name;
            prefab.AddComponent<AlchemyDraggableItem>();

            var script = prefab.GetComponent<AlchemyDraggableItem>();
            script.item = item;
            script.dragParent = dragParent;
            script.alchemyMenu = this;

            ingredientPrefabs.Add(prefab);

            var matchingEntry = availableIngredients.FirstOrDefault(entry => entry.item == item);

            if (matchingEntry != null)
            {
                matchingEntry.amount--;
            }
            else
            {
                Debug.Log("Something went wrong when removing the item.");
            }

            UpdateInventoryItem(script.item);
            return prefab;
        }
    }

    public void ReturnIngredientToInventory(GameObject ingredientObject)
    {
        GameObject prefab = ingredientPrefabs.FirstOrDefault(obj => obj == ingredientObject);

        if (prefab != null)
        {
            ingredientPrefabs.Remove(prefab);

            Item item = prefab.GetComponent<AlchemyDraggableItem>().item;
            ItemIntPair matchingEntry = availableIngredients.FirstOrDefault(entry => entry.item == item);

            if (item == null)
            {
                Debug.LogWarning("Item in prefab's AlchemyDraggableItem script was null. Something went wrong.");
            }
            else
            {
                if (matchingEntry != null)
                {
                    matchingEntry.amount++;
                }
                else
                {
                    Debug.Log("Something went wrong when returning the item.");
                }

                UpdateInventoryItem(item);
            }
            Destroy(prefab);
        }

        if (ingredientObject != null)
        {
            Destroy(ingredientObject);
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
}