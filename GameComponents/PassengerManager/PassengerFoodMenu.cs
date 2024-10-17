using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassengerFoodMenu : MonoBehaviour
{
    public static bool isActive;
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

    public void Initialise()
    {
        if (PassengerSatisfaction.isEnabled)
        {
            if (PassengerFoodManager.viableFoodItems == null || PassengerFoodManager.viableFoodItems.Count == 0)
            {
                PassengerSatisfaction.ForceUpdateViableInventory();
            }

            isActive = true;
            canvasObject.SetActive(true);
            Clear();
            SetUpStockList();
            SetUpInventoryList();
        }
    }

    public void Close()
    {
        isActive = false;
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
                SetUpListDisplays(stockPrefab.listPrefab, stockPrefab.item);

                stockPrefab.UpdateValues();

                if (stockPrefab.item != null && !string.IsNullOrEmpty(stockPrefab.item.objectID))
                {
                    stockPrefab.listPrefab.button.onClick.AddListener(() => RemoveItemFromStock(stockPrefab));
                }
                else
                {
                    stockPrefab.listPrefab.button.transition = UnityEngine.UI.Selectable.Transition.None;
                    stockPrefab.listPrefab.button.interactable = false;
                }

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
            SetUpListDisplays(invPrefab.listPrefab, invPrefab.item);

            invPrefab.UpdateValues();

            if (invPrefab.item != null && !string.IsNullOrEmpty(invPrefab.item.objectID))
            {
                invPrefab.listPrefab.button.onClick.AddListener(() => AddItemToStock(invPrefab));
            }
            else
            {
                invPrefab.listPrefab.button.transition = UnityEngine.UI.Selectable.Transition.None;
                invPrefab.listPrefab.button.interactable = false;
            }

            inventoryPrefabs.Add(invPrefab);
        }
    }

    void SetUpListDisplays(ListItemPrefab listItem, Item item)
    {
        if (listItem != null && item != null)
        {
            listItem.CreateItemTags(item);

            if (!string.IsNullOrEmpty(item.objectID))
            {
                listItem.DisplayItemSprite(item.objectID);
            }
        }
    }

    List<IdIntPair> GetViableInventoryItems()
    {
        var inventoryItems = new List<IdIntPair>();

        Debug.Log($"PassSat: Viable food item list contained {PassengerFoodManager.viableFoodItems.Count}.");

        foreach (var item in PassengerFoodManager.viableFoodItems)
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
        var count = Player.GetCount(invPrefab.item.objectID, name);

        if (count > 0)
        {
            var stockEntry = dataManager.passengerFood.FirstOrDefault(e => e.objectID == invPrefab.item.objectID);

            if (stockEntry == null)
            {
                stockEntry = new() { objectID = invPrefab.item.objectID };
                dataManager.passengerFood.Add(stockEntry);
            }

            if (stockEntry.amount < 10)
            {
                stockEntry.amount++;
                count--;
                Player.Remove(invPrefab.item.objectID);

                bool foundPrefab = false;

                foreach (var script in stockPrefabs)
                {
                    if (script.item != null && script.item.objectID == invPrefab.item.objectID)
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
                LogAlert.QueueTextAlert($"Stock is already full on {invPrefab.item.name}.");
            }
        }
        else
        {
            LogAlert.QueueTextAlert($"Not enough {invPrefab.item.name} in inventory.");
        }


        if (count == 0)
        {
            SetUpInventoryList();
        }
        else
        {
            invPrefab.UpdateValues();
        }
    }

    void RemoveItemFromStock(SatisfactionStockPrefab stockPrefab)
    {
        var stockEntry = dataManager.passengerFood.FirstOrDefault(e => e.objectID == stockPrefab.item.objectID);

        if (stockEntry == null)
        {
            Debug.LogWarning("Could not find entry in stock.");
            stockPrefab.gameObject.SetActive(false);
        }
        else
        {
            var added = Player.Add(stockEntry.objectID);

            if (added == 1)
            {
                stockEntry.amount--;
                LogAlert.QueueTextAlert($"Retrieved {stockPrefab.item.name} from stock.");

                bool foundPrefab = false;

                foreach (var script in inventoryPrefabs)
                {
                    if (script.item != null && script.item.objectID == stockPrefab.item.objectID)
                    {
                        foundPrefab = true;
                        script.UpdateValues();
                        break;
                    }
                }

                if (!foundPrefab)
                {
                    SetUpInventoryList();
                }
            }
            else if (added == 0)
            {
                LogAlert.QueueTextAlert($"No more room in inventory for {stockPrefab.item.name}.");
            }

            if (stockEntry.amount == 0)
            {
                dataManager.passengerFood.Remove(stockEntry);
                SetUpStockList();
            }
            else
            {
                stockPrefab.UpdateValues();
            }
        }
    }
}
