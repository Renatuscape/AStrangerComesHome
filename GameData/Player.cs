using System.Collections.Generic;
using System.Linq;
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

    public static void Remove(IdIntPair entry, bool doNotLog = false)
    {
        Remove(entry.objectID, entry.amount, doNotLog);
    }
    public static void Remove(string objectID, int amount = 1, bool doNotLog = false)
    {
        int removeAmount = amount;

        if (removeAmount > 0)
        {
            removeAmount = removeAmount * -1;
        }

        Add(objectID, removeAmount, doNotLog);
    }

    public static void Set(string objectID, int amount, bool doNotLog = false)
    {
        var foundObject = inventoryList.FirstOrDefault(o => o.objectID == objectID);

        if (foundObject != null)
        {
            inventoryList.FirstOrDefault(o => o.objectID == objectID).amount = amount;
        }
        else
        {
            Add(objectID, amount, doNotLog);
        }
    }

    public static int Add(string objectID, int amount = 1, bool doNotLog = false)
    {
        if (objectID.Contains("-NAME"))
        {
            Debug.Log($"Detected -NAME tag. Adding {objectID} to list.");
            TransientDataCalls.gameManager.dataManager.unlockedNames.Add(objectID);
        }
        else if (GameCodex.ParseID(objectID) != null)
        {
            return AddDynamicObject(GameCodex.ParseID(objectID), amount, doNotLog);
        }
        return 0;
    }

    public static int Add(IdIntPair entry, bool doNotLog = false)
    {
        return Add(entry.objectID, entry.amount, doNotLog);
    }

    public static int AddDynamicObject(dynamic dynamicObject, int amount, bool doNotLog = false, string caller = "")
    {
        Debug.Log($"Attempting to add dynamic object {dynamicObject.name} ({amount})");
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
        else if (dynamicObject is Recipe)
        {
            var recipe = (Recipe)dynamicObject;
            max = recipe.maxStack;
            id = recipe.objectID;
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

                        if (!doNotLog)
                        {
                            AttemptToLog(dynamicObject, amount);
                        }
                        return amount;
                    }
                    else
                    {
                        int reduced = max - entry.amount;
                        entry.amount += reduced;

                        if (!doNotLog)
                        {
                            AttemptToLog(dynamicObject, reduced);
                        }
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

                    if (!doNotLog)
                    {
                        AttemptToLog(dynamicObject, amount);
                    }
                    return amount;
                }
                else
                {
                    int reduced = max - newEntry.amount;
                    newEntry.amount += reduced;

                    if (!doNotLog)
                    {
                        AttemptToLog(dynamicObject, reduced);
                    }
                    return reduced;
                }
            }
        }
        return 0;
    }

    static void AttemptToLog(dynamic dynamicObject, int value)
    {
        if (dynamicObject is Item)
        {
            LogAlert.QueueItemAlert((Item)dynamicObject, value);
        }
        else if (dynamicObject is Character)
        {
            LogAlert.QueueAffectionAlert((Character)dynamicObject, value);
        }
        else if (dynamicObject is Skill)
        {
            var skill = (Skill)dynamicObject;
            LogAlert.QueueTextAlert($"{skill.name} increased by {value}");
        }
        else if (dynamicObject is Recipe)
        {
            var recipe = (Recipe)dynamicObject;
            LogAlert.QueueTextAlert($"Obtained {recipe.name}!");
        }
        else if (dynamicObject is Upgrade)
        {
            var upgrade = (Upgrade)dynamicObject;
            LogAlert.QueueTextAlert($"{upgrade.name} upgraded!");
        }
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