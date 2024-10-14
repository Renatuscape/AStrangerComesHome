using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PassengerSatisfaction : MonoBehaviour
{
    public static PassengerSatisfaction instance;
    public DataManagerScript dataManager;

    public float feedChanceA;
    public float feedChanceB;
    public bool dinnerForB;
    public bool debugMode;

    public static List<Item> viableFoodItems;
    public List<Item> debugStockList;

    private void Start()
    {
        instance = this;
        NewPassengerA();
        NewPassengerB();
    }

    void FindViableFoodItems()
    {
        if (Items.all != null && Items.all.Count > 0)
        {
            viableFoodItems = new();

            foreach (Item item in Items.all)
            {
                if (RequirementChecker.CheckForTag(item, "food") || RequirementChecker.CheckForTag(item, "drink"))
                {
                    viableFoodItems.Add(item);
                }
            }

            viableFoodItems = viableFoodItems.OrderBy(i => i.rarity).ThenBy(i => i.basePrice).ToList();
        }
        else
        {
            viableFoodItems = null;
        }
    }

    void Daily()
    {
        if (dataManager.seatA != null && dataManager.seatA.isActive)
        {
            ReduceSatisfaction(dataManager.seatA);
        }

        if (dataManager.seatB != null && dataManager.seatB.isActive)
        {
            ReduceSatisfaction(dataManager.seatB);
        }
    }

    void Hourly()
    {
        if (dataManager.passengerFood == null)
        {
            dataManager.passengerFood = new();
        }

        if (!dinnerForB)
        {
            if (dataManager.seatA != null && dataManager.seatA.isActive && !dataManager.seatA.isFedToday)
            {
                if (RollForMealtime(ref feedChanceA))
                {
                    FeedPassenger(dataManager.seatA);
                }
            }


            dinnerForB = true;
        }
        else
        {
            if (dataManager.seatB != null && dataManager.seatB.isActive && !dataManager.seatB.isFedToday)
            {
                if (RollForMealtime(ref feedChanceB))
                {
                    FeedPassenger(dataManager.seatB);
                }
            }

            dinnerForB = false;
        }
    }

    bool RollForMealtime(ref float chance)
    {
        if (dataManager.timeOfDay > 0.9f || Random.Range(0f, 100f) < chance)
        {
            if (!PassengerSatisfactionMenu.isActive)
            {
                chance = 5;
                return true;
            }
            else
            {
                LogAlert.QueueTextAlert("Passengers are unable to eat while I am handling the food stock.");
                chance += 2;
                return false;
            }
        }
        else
        {
            chance += 2;
            return false;
        }
    }

    void ReduceSatisfaction(PassengerData passenger)
    {
        passenger.isFedToday = false;

        if (passenger.satisfaction > 0)
        {
            passenger.satisfaction--;
        }
    }

    void FeedPassenger(PassengerData passenger)
    {
        if (dataManager.passengerFood.Count == 0)
        {
            LogAlert.QueueTextAlert($"{passenger.passengerName} would have liked a meal.");
        }
        else
        {
            passenger.isFedToday = true;

            if (passenger.satisfaction < 10)
            {
                passenger.satisfaction += ConsumeFood(out var food);
                string verb = RequirementChecker.CheckForTag(food, "drink") ? "drank" : "ate";

                LogAlert.QueueTextAlert($"{passenger.passengerName} {verb} {food.name}.");
            }
            else
            {
                ConsumeFood(out var food, true);
                string verb = RequirementChecker.CheckForTag(food, "drink") ? "drank" : "ate";

                LogAlert.QueueTextAlert($"{passenger.passengerName} looked for a small snack. They {verb} {food.name}.");
            }
        }
    }

    float ConsumeFood(out Item food, bool satisfactionMaxed = false)
    {
        if (viableFoodItems == null || viableFoodItems.Count < 1)
        {
            FindViableFoodItems();
        }

        var stock = GetStock(); // Rebuilding the stock for each food attempt ensures that data is updated and matching the save data
        debugStockList = stock; // View stock list copy

        if (stock.Count > 0)
        {
            float foodValue = 0;

            Item item = null;

            if (satisfactionMaxed)
            {
                if (stock.Count >= 3)
                {
                    item = stock[Random.Range(0, 3)];
                }
                else
                {
                    item = stock[0];
                }
            }
            else
            {
                if (stock.Count >= 5)
                {
                    item = stock[Random.Range(Mathf.RoundToInt(stock.Count * 0.66f), stock.Count)];
                }
                else
                {
                    item = stock[stock.Count - 1];
                }


                if (item != null && !string.IsNullOrEmpty(item.objectID))
                {
                    foodValue += CalculateFoodValue(item);
                }
            }

            var entry = dataManager.passengerFood.FirstOrDefault(e => e.objectID == item.objectID);

            if (entry.amount > 1)
            {
                entry.amount--;
            }
            else
            {
                dataManager.passengerFood.Remove(entry);
            }

            Debug.Log(item.objectID + " food value was assessed to be " + foodValue);
            food = item;
            return foodValue;
        }

        food = null;
        return 0;
    }

    float CalculateFoodValue(Item item)
    {
        float foodValue = 0;

        if (RequirementChecker.CheckForTags(item, new() { "drink", "snack" }, 1))
        {
            foodValue += 1.25f;
        }
        else
        {
            foodValue += 1.5f;
        }

        if (RequirementChecker.CheckForTag(item, "condiment"))
        {
            foodValue -= 0.5f;
        }

        if (item.type == ItemType.Plant)
        {
            foodValue -= 0.25f;
        }
        else if (item.type == ItemType.Trade)
        {
            foodValue += 1f;
        }
        else if (item.type == ItemType.Treasure)
        {
            foodValue += 2.5f;
        }

        foodValue += (float)item.rarity;

        if (RequirementChecker.CheckForTag(item, "basic"))
        {
            foodValue = foodValue * 0.5f;
        }
        else if (RequirementChecker.CheckForTag(item, "moreish"))
        {
            foodValue = foodValue * 1.5f;
        }

        return foodValue;
    }

    List<Item> GetStock()
    {
        List<Item> availableStock = new();

        if (debugMode)
        {
            dataManager.passengerFood.Clear();

            foreach (var entry in viableFoodItems)
            {
                dataManager.passengerFood.Add(new() { objectID = entry.objectID, amount = 2 });
            }
        }

        foreach (var entry in dataManager.passengerFood)
        {
            var foundItem = viableFoodItems.FirstOrDefault(i => i.objectID == entry.objectID);

            if (foundItem != null)
            {
                availableStock.Add(foundItem);
            }
        }

        return availableStock = availableStock.OrderBy(i => CalculateFoodValue(i)).ToList(); //i.type).ThenBy(i => RequirementChecker.CheckForTags(i, new() { "snack", "drink", "basic" }, 1)).ThenBy(i => i.rarity).ThenBy(i => i.basePrice).ToList();
    }

    public void NewPassengerA()
    {
        feedChanceA = Random.Range(5f, 20f);
    }

    public void NewPassengerB()
    {
        feedChanceB = Random.Range(5f, 20f);
    }

    public static void DailyTick()
    {
        if (instance != null)
        {
            instance.Daily();
        }
    }

    public static void HourlyTick()
    {
        if (instance != null)
        {
            instance.Hourly();
        }
    }

    public static void ForceUpdateViableInventory()
    {
        if (instance != null)
        {
            instance.FindViableFoodItems();
        }
    }
}
