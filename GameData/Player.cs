using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
public static class Player
{
    public static List<IdIntPair> inventoryList = new();

    public static bool GetEntry(string objectID, string caller, out IdIntPair entry)
    {
        entry = inventoryList?.Find(o => o?.objectID != null && o.objectID.Contains(objectID));

        if (entry != null)
        {
            return true;
        }
        return false;
    }

    public static void Add(string objectID, int amount = 1)
    {
        if (GameCodex.ParseID(objectID) != null)
        {
            Add(GameCodex.ParseID(objectID), amount);
        }
    }

    public static int Add(dynamic dynamicObject, int amount, string caller = "")
    {
        int max = 0;
        string id = string.Empty;

        if (dynamicObject is Item)
        {
            var item = (Item)dynamicObject;
            max = item.maxStack;
            id = item.objectID;
        }
        else if (dynamicObject is Skill)
        {
            var skill = (Skill)dynamicObject;
            max = skill.maxLevel;
            id = skill.objectID;
        }
        else if (dynamicObject is Upgrade)
        {
            var upgrade = (Upgrade)dynamicObject;
            max = upgrade.maxLevel;
            id = upgrade.objectID;
        }
        else if (dynamicObject is Quest)
        {
            var quest = (Quest)dynamicObject;
            max = quest.maxValue;
            id = quest.objectID;
        }
        else if (dynamicObject is Character)
        {
            var character = (Character)dynamicObject;
            max = character.maxValue;
            id = character.objectID;
        }
        if (id != "")
        {
            bool itemExists = false;
            foreach (IdIntPair entry in inventoryList)
            {
                if (entry.objectID == id)
                {
                    itemExists = true;
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
            }
            if (!itemExists)
            {
                IdIntPair newEntry = new() { objectID = id, amount = 0 };
                inventoryList.Add(newEntry);
                Debug.Log($"Caller {caller} created new entry for {id} in player inventory.");

                if (amount >= 0 && amount <= max)
                {
                    newEntry.amount += amount;
                    return amount;
                }
                else
                {
                    int reduced = max - newEntry.amount;
                    newEntry.amount += reduced;
                    return reduced;
                }
            }
        }
        return 0;
    }

    public static int GetCount(string searchID, string caller)
    {
        //Debug.Log(caller + " called Player.GetCount()");
        if (GetEntry(searchID, "Player", out var entry))
        {
            return entry.amount;
        }
        else
        {
            return 0;
        }
    }
}