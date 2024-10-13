using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        stockPrefabs.Clear();

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
        inventoryPrefabs.Clear();

        var inventoryList = availableInventory.InitialiseWithoutCategories(GetViableInventoryItems());

        foreach (var invObject in inventoryList)
        {
            var invPrefab = invObject.AddComponent<SatisfactionInventoryPrefab>();

            invPrefab.listPrefab = invObject.GetComponent<ListItemPrefab>();

            invPrefab.item = Items.FindByID(invPrefab.listPrefab.entry.objectID);

            invPrefab.UpdateValues();

            if (invPrefab.item != null && !string.IsNullOrEmpty(invPrefab.item.objectID))
            {
                invPrefab.listPrefab.button.onClick.AddListener(() => AddItemToStock(invPrefab));
            }
            else
            {
                invPrefab.listPrefab.button.interactable = false;
            }

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

    void AddItemToStock(SatisfactionInventoryPrefab invPrefab)
    {
        if (invPrefab.item != null && !string.IsNullOrEmpty(invPrefab.item.objectID))
        {
            var count = Player.GetCount(invPrefab.item.objectID, name);

            if (count > 0)
            {
                var stockEntry = dataManager.passengerFood.FirstOrDefault(e => e.objectID == invPrefab.item.objectID);

                if (stockEntry == null)
                {
                    stockEntry = new() { objectID = invPrefab.item.objectID };
                    dataManager.passengerFood.Add(stockEntry);
                }

                if (stockEntry.amount < 30)
                {
                    stockEntry.amount++;
                    count--;
                    Player.Remove(invPrefab.item.objectID);

                    bool foundPrefab = false;

                    foreach (var script in stockPrefabs)
                    {
                        if (script.item.objectID == invPrefab.item.objectID)
                        {
                            foundPrefab = true;
                            script.UpdateValues();
                            break;
                        }
                    }

                    if (!foundPrefab)
                    {
                        SetUpStockList();
                    }

                    LogAlert.QueueTextAlert($"Added {invPrefab.item.name} to stock.");
                }
                else
                {
                    LogAlert.QueueTextAlert($"Added {invPrefab.item.name} to stock.");
                }
            }
            else
            {
                LogAlert.QueueTextAlert($"Not enough {invPrefab.item.name} in inventory.");
            }


            if (count == 0)
            {
                invPrefab.gameObject.SetActive(false);
            }
            else
            {
                invPrefab.UpdateValues();
            }
        }
        else
        {
            Debug.LogWarning("Inventory prefab had no item.");
        }
    }

    void RemoveItemFromStock(SatisfactionStockPrefab stockPrefab)
    {

    }
}
