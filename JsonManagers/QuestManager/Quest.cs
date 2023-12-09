using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string objectID;
    public string name;
    public bool excludeFromJournal;
    public int maxValue = 1;
    public List<AdvancementCheck> unlockRequirements;
    public List<IdIntPair> rewards;

    //Repeatable quests increase in dataLevel to 100 on completion, and are reset to 10 (skipping discovery stages) by the world clock
    public int resetAfterDays;
    public int autoAdvanceAfterDays;

    public List<Dialogue> dialogues = new();
    // Add any other quest-related properties here

    // Check quest stage's dialogue list of characters and see if there is a match. Otherwise public SerializableDictionary<int, Character> perStageNPC;

    public void AddToPlayer(int amount = 1)
    {
        Player.Add(this, amount);
    }
    public int GetCountPlayer()
    {
        return Player.GetCount(objectID);
    }
}

public static class Quests
{
    public static List<Quest> all = new();

    public static void DebugList()
    {
        Debug.LogWarning("Items.DebugList() called");

        foreach (Quest item in all)
        {
            Debug.Log($"Item ID: {item.objectID}\tItem Name: {item.name}");
        }
    }

    public static void DebugAllItems()
    {
        foreach (Quest item in all)
        {
            item.AddToPlayer(1);
        }
    }

    public static Quest FindByID(string searchWord)
    {
        if (string.IsNullOrWhiteSpace(searchWord))
        {
            Debug.LogWarning($"Search term was null or white-space. Returned null. Ensure correct ID in calling script.");
            return null;
        }
        foreach (Quest item in all)
        {
            if (item.objectID.Contains(searchWord))
            {
                return item;
            }
        }
        Debug.LogWarning("No item found with ID containing this search term: " + searchWord);
        return null;
    }
}

[System.Serializable]
public class AdvancementCheck
{
    public int minimumCharacterLevel;
    public int minimumDaysPassed;

    //The player must have at least this much
    public List<IdIntPair> requirements;

    //The player must have less than this amount
    public List<IdIntPair> restrictions;
}

[System.Serializable]
public class IdIntPair
{
    public string objectID;
    public int amount;
}