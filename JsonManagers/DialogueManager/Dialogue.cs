using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StageType
{
    Dialogue,
    Memory,
    PopUp
}

[System.Serializable]
public class Dialogue
{
    public StageType stageType;
    public string objectID;
    public string questID;
    public int questStage;
    public string topicName;
    public string hint;
    public int startTime = 0;
    public int endTime = 0;

    public List<string> content;
    public List<DialogueStep> dialogueSteps = new(); //a step is one line of dialogue from one speaker
    public List<IdIntPair> requirements = new();
    public List<IdIntPair> restrictions = new();
    public List<Choice> choices = new(); //LEAVE CHOICES BLANK TO LOOP DIALOGUE WITHOUT PROGRESSION
    public bool autoProgressStage = false; //automatically go to next stage when step is completed
    public bool noLeaveButton = true; //if this is enabled, it is impossible to leave dialogue until you reach a choice

    //MEMORY SPECIFIC
    public string location;

    public bool CheckRequirements(out bool hasTimer, out bool hasLocation)
    {
        bool hasPassed = true;
        hasTimer = false;
        hasLocation = false;

        if (startTime != 0 && endTime != 0)
        {
            hasTimer = true;
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            hasLocation = true;
        }

        if (requirements is not null && requirements.Count > 0)
        {
            foreach (IdIntPair entry in requirements)
            {
                int amount = Player.GetCount(entry.objectID, "Choice Requirement Check");
                if (amount < entry.amount)
                {
                    hasPassed = false;
                    break;
                }
            }
        }
        if (hasPassed && restrictions is not null && restrictions.Count > 0) //don't run if checks already failed
        {
            foreach (IdIntPair entry in restrictions)
            {
                int amount = Player.GetCount(entry.objectID, "Choice Restriction Check");
                if (amount >= entry.amount)
                {
                    hasPassed = false;
                    break;
                }
            }
        }
        return hasPassed;
    }
}

public static class Dialogues
{
    public static List<Dialogue> all = new();

    public static void DebugList()
    {
        Debug.LogWarning("Dialogue.DebugList() called");

        foreach (Dialogue dialogue in all)
        {
            Debug.Log($"Dialogue for quest {dialogue.questID}: stage {dialogue.questStage}");
        }
    }

    public static void DebugAllItems()
    {
        foreach (Dialogue dialogue in all)
        {
            Debug.Log($"{dialogue}" +
                $"\n{dialogue.dialogueSteps.Count}");
        }
    }

    public static List<Dialogue> FindQuestDialogues(string questID)
    {
        return all
            .Where(d => d.questID == questID)
            .OrderBy(d => d.questStage)
            .ToList();
    }
}