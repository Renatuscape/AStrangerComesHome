using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class Quest : BaseObject
{
    public List<RestrictedInfo> dynamicDescriptions;
    public bool excludeFromJournal;
    public Character questGiver; //Set from objectID by QuestManager
    public AdvancementCheck unlockRequirements;
    public List<IdIntPair> rewards;

    //Repeatable quests increase in dataLevel to 100 on completion, and are reset to 10 (skipping discovery stages) by the world clock
    public int resetChance = 100;
    public int resetWeekDay = 0; //not all quests need to reset on the same day of the week
    public bool advanceEveryDay;
    public bool advanceEveryWeek;
    public bool advanceEveryMonth;
    public bool advanceEveryYear;
    public bool resetOnComplete;
    public bool resetToRandomStage;

    public List<Dialogue> dialogues = new();

    public int GetQuestStage()
    {
        return Player.GetCount(objectID, "Quest");
    }
    public void SetQuestStage(int stage)
    {
        Player.Set(objectID, stage);
    }

    public Dialogue GetStage(int stage)
    {
        return dialogues.FirstOrDefault(d => d.questStage == stage);
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

    public static Dialogue GetDialogueByQuestStage(string questID, int stage)
    {
        Quest quest = all.FirstOrDefault(q => q.objectID == questID);
        Dialogue dialogue = quest.dialogues.FirstOrDefault(d => d.questStage == stage);

        return dialogue;
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

    public static List<Quest> GetQuestsByGiver(string characterID)
    {
        return all.Where(q => q.questGiver.objectID == characterID).ToList();
    }
}