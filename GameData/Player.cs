using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using static UnityEditor.Progress;

public static class Player
{
    //public static SerializableDictionary<string, int> items = new();
    //public static SerializableDictionary<string, int> skills = new();
    //public static SerializableDictionary<string, int> upgrades = new();
    //public static SerializableDictionary<string, int> quests = new();
    //public static SerializableDictionary<string, int> friendships = new();

    public static List<IdIntPair> inventoryList = new();

    public static bool GetEntry(string objectID, out IdIntPair entry)
    {
        entry = inventoryList.Find(item => item.objectID.Contains(objectID));

        if (entry != null)
        {
            return true;
        }
        return false;
    }

    public static void Add(string objectID, int amount = 1)
    {
        if (ParseID(objectID) != null)
        {
            Add(ParseID(objectID), amount);
        }
    }

    public static dynamic ParseID(string searchID)
    {
        Item foundItem = Items.FindByID(searchID);
        if (foundItem != null)
        {
            return foundItem;
        }

        Skill foundSkill = Skills.FindByID(searchID);
        if (foundItem != null)
        {
            return foundSkill;
        }

        Upgrade foundUpgrade = Upgrades.FindByID(searchID);
        if (foundUpgrade != null)
        {
            return foundUpgrade;
        }

        Quest foundQuest = Quests.FindByID(searchID);
        if (foundQuest != null)
        {
            return foundQuest;
        }

        Character foundCharacter = Characters.FindByID(searchID);
        if (foundCharacter != null)
        {
            return foundCharacter;
        }

        return null;
    }
    public static int Add(dynamic addObject, int amount)
    {
        int max = 0;
        string id = string.Empty;

        if (addObject is Item)
        {
            var item = (Item)addObject;
            max = item.maxStack;
            id = item.objectID;
        }
        else if (addObject is Skill)
        {
            var skill = (Skill)addObject;
            max = skill.maxLevel;
            id = skill.objectID;
        }
        else if (addObject is Upgrade)
        {
            var upgrade = (Upgrade)addObject;
            max = upgrade.maxLevel;
            id = upgrade.objectID;
        }
        else if (addObject is Quest)
        {
            var quest = (Quest)addObject;
            max = quest.maxValue;
            id = quest.objectID;
        }
        else if (addObject is Character)
        {
            var quest = (Quest)addObject;
            max = quest.maxValue;
            id = quest.objectID;
        }

        if (!string.IsNullOrWhiteSpace(id))
        {
            if (GetEntry(id, out var entry))
            {
                if (entry.amount + amount <= max && entry.amount + amount >= 0)
                {
                    entry.amount += amount;
                    return amount;
                }
                else
                {
                    int reduced = max - entry.amount;
                    entry.amount += reduced;
                    return reduced;
                }
            }
            else
            {
                entry = new() { objectID = id, amount = 0 };
                inventoryList.Add(entry);

                if (amount >= 0 && amount <= max)
                {
                    entry.amount += amount;
                    return amount;
                }
                else
                {
                    int reduced = max - entry.amount;
                    entry.amount += reduced;
                    return reduced;
                }
            }
        }
        else
        {
            Debug.LogWarning($"{addObject.name} is not a valid type for Player.Add.");
            return 0;
        }
    }

    public static int GetCount(string searchID)
    {
        if (GetEntry(searchID, out var entry))
        {
            return entry.amount;
        }
        else
        {
            return 0;
        }
    }
}