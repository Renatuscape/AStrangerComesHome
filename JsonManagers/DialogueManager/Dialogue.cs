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
    public int startTime = 0;
    public int endTime = 0;

    public List<string> content;
    public List<DialogueStep> dialogueSteps = new(); //a step is one line of dialogue from one speaker
    public List<IdIntPair> requirements = new();
    public List<Choice> choices = new (); //LEAVE CHOICES BLANK TO LOOP DIALOGUE WITHOUT PROGRESSION
    public bool progressOnContinue = false; //automatically go to next stage when step is completed
    public bool noLeaveButton = true; //if this is enabled, it is impossible to leave dialogue until you reach a choice

    //MEMORY SPECIFIC
    public string location;
    public string hint;
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