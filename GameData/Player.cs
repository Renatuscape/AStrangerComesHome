using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;

public static class Player
{
    public static SerializableDictionary<string, int> items = new();
    public static SerializableDictionary<string, int> skills = new();
    public static SerializableDictionary<string, int> upgrades = new();

    public static void Add(string searchID, int amount)
    {

        if (items.ContainsKey(searchID))
        {
            items[searchID] += amount;
        }
        else
        {
            var item = Items.FindByID(searchID);
            if (item != null)
            {
                items.Add(item.objectID, amount);
            }
        }
    }

    public static void Add(Item item, int amount, int maxStack)
    {
        if (items.ContainsKey(item.objectID))
        {
            if (items[item.objectID] + amount <= maxStack)
            {
                items[item.objectID] += amount;
            }
        }
    }

    public static int GetCount(string searchID)
    {
        if (items.ContainsKey(searchID))
        {
            return items[searchID];
        }
        else if (skills.ContainsKey(searchID))
        {
            return skills[searchID];
        }
        else if (upgrades.ContainsKey(searchID))
        {
            return upgrades[searchID];
        }
        else
        {
            return 0;
        }
    }
}