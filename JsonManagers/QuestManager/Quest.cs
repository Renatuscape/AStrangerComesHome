using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string objectID;
    public string name;
    public bool excludeFromJournal;
    public int maxValue = 100;
    public Character questGiver; //Set from objectID by QuestManager
    public List<AdvancementCheck> unlockRequirements;
    public List<IdIntPair> rewards;

    //Repeatable quests increase in dataLevel to 100 on completion, and are reset to 10 (skipping discovery stages) by the world clock
    public int resetAfterDays;
    public int autoAdvanceAfterDays;

    public List<Dialogue> dialogues = new();

    public int GetQuestStage()
    {
        return Player.GetCount(objectID, "Quest");
    }
    public void SetQuestStage(int stage)
    {
        Player.GetEntry(objectID, "Quest", out var entry);
        if (entry != null)
        {
            entry.amount = stage;
        }
        else
        {
            entry = new() { objectID = objectID, amount = stage };
            Player.inventoryList.Add(entry);
        }
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
        foreach (Quest quest in all)
        {
            quest.SetQuestStage(0);
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
            if (item.objectID == searchWord)
            {
                return item;
            }
        }
        //Debug.LogWarning("No item found with ID containing this search term: " + searchWord);
        return null;
    }
}