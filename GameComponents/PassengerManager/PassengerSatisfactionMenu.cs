using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class PassengerSatisfactionMenu : MonoBehaviour
{
    public DataManagerScript dataManager;
    public GameObject canvasObject;
    public PageinatedList stockedFood;
    public PageinatedList availableInventory;
    public List<SatisfactionStockPrefab> stockPrefabs;
    public List<SatisfactionInventoryPrefab> inventoryPrefabs;

    private void Start()
    {
        canvasObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            Initialise();
        }
    }
    public void Initialise()
    {
        if (PassengerSatisfaction.viableFoodItems == null || PassengerSatisfaction.viableFoodItems.Count == 0)
        {
            PassengerSatisfaction.ForceUpdateViableInventory();
        }

        canvasObject.SetActive(true);
        Clear();
        SetUpStockList();
        SetUpInventoryList();
    }

    public void Close()
    {
        canvasObject.SetActive(false);
        Clear();
    }

    void Clear()
    {
        stockPrefabs.Clear();
        inventoryPrefabs.Clear();
    }

    void SetUpStockList()
    {
        if (dataManager.passengerFood.Count > 0)
        {
            var stockObjects = stockedFood.InitialiseWithoutCategories(dataManager.passengerFood);

            foreach (var stockObject in stockObjects)
            {
                var stockPrefab = stockObject.AddComponent<SatisfactionStockPrefab>();

                stockPrefab.listPrefab = stockObject.GetComponent<ListItemPrefab>();

                stockPrefab.item = Items.FindByID(stockPrefab.listPrefab.entry.objectID);

                stockPrefab.UpdateValues();

                stockPrefabs.Add(stockPrefab);
            }
        }
        else
        {
            stockedFood.InitialiseWithoutCategories(new() { new() { objectID = "ALERT", description = "No food has been stocked" } });
        }
    }

    void SetUpInventoryList()
    {
        var inventoryList = availableInventory.InitialiseWithoutCategories(GetViableInventoryItems());

        foreach (var invObject in inventoryList)
        {
            var invPrefab = invObject.AddComponent<SatisfactionInventoryPrefab>();

            invPrefab.listPrefab = invObject.GetComponent<ListItemPrefab>();

            invPrefab.item = Items.FindByID(invPrefab.listPrefab.entry.objectID);

            invPrefab.UpdateValues();

            inventoryPrefabs.Add(invPrefab);
        }
    }

    List<IdIntPair> GetViableInventoryItems()
    {
        var inventoryItems = new List<IdIntPair>();

        Debug.Log($"PassSat: Viable food item list contained {PassengerSatisfaction.viableFoodItems.Count}.");

        foreach (var item in PassengerSatisfaction.viableFoodItems)
        {
            int count = Player.GetCount(item.objectID, name);

            Debug.Log($"PassSat: Checking inventory for {item.objectID}. Found {count} in player inventory.");

            if (count > 0)
            {
                inventoryItems.Add(new() { objectID = item.objectID, amount = count });
            }
        }

        if (inventoryItems.Count == 0)
        {
            Debug.Log($"PassSat: Viable inventory items was zero.");
            inventoryItems.Add(new() { objectID = "ALERT", description = "No food or drink items in inventory" });
        }

        return inventoryItems;
    }
}

public class SatisfactionStockPrefab : MonoBehaviour
{
    public Item item;
    public ListItemPrefab listPrefab;

    public void UpdateValues()
    {
        if (item != null && listPrefab.entry != null && !string.IsNullOrEmpty(item.name))
        {
            if (listPrefab.entry.amount == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                listPrefab.textMesh.text = $"{item.name} ({listPrefab.entry.amount})";
            }
        }
        else if (listPrefab.entry == null || listPrefab.entry.objectID != "ALERT")
        {
            gameObject.SetActive(false);
        }
    }
}

public class SatisfactionInventoryPrefab : MonoBehaviour
{
    public Item item;
    public ListItemPrefab listPrefab;

    public void UpdateValues()
    {
        if (item != null && !string.IsNullOrEmpty(item.name))
        {
            var count = Player.GetCount(item.objectID, name);

            if (count == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                listPrefab.textMesh.text = $"{item.name} ({Player.GetCount(item.objectID, name)})";
            }
        }
        else if (listPrefab.entry == null || listPrefab.entry.objectID != "ALERT")
        {
            gameObject.SetActive(false);
        }
    }
}