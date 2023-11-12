using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;

public static class Player
{
    public static SerializableDictionary<string, int> items = new();
    public static SerializableDictionary<string, int> skills = new();
    public static SerializableDictionary<string, int> upgrades = new();

    public static void Add(dynamic addObject, int amount, int maxStack)
    {
        if (addObject is Item)
        {
            var item = (Item)addObject;
            if (items.ContainsKey(item.objectID))
            {
                if (items[item.objectID] + amount <= maxStack && items[item.objectID] + amount >= 0)
                {
                    items[item.objectID] += amount;
                }
            }
            else
            {
                if (amount >= 0 && amount <= item.maxStack)
                {
                    items.Add(item.objectID, amount);
                }
            }
        }
        else if (addObject is Skill)
        {
            var skill = (Skill)addObject;
            if (skills.ContainsKey(skill.objectID))
            {
                if (skills[skill.objectID] + amount <= maxStack && skills[skill.objectID] + amount >= 0)
                {
                    skills[skill.objectID] += amount;
                }
            }
            else
            {
                if (amount >= 0 && amount <= skill.maxLevel)
                {
                    skills.Add(skill.objectID, amount);
                }
            }
        }
        else if (addObject is Upgrade)
        {
            var upgrade = (Upgrade)addObject;
            if (upgrades.ContainsKey(upgrade.objectID))
            {
                if (upgrades[upgrade.objectID] + amount <= maxStack && upgrades[upgrade.objectID] + amount >= 0)
                {
                    upgrades[upgrade.objectID] += amount;
                }
            }
            else
            {
                if (amount >= 0 && amount <= upgrade.maxLevel)
                {
                    upgrades.Add(upgrade.objectID, amount);
                }
            }
        }
        else
        {
            Debug.LogWarning($"{addObject.name} is not a valid type for Player.Add.");
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