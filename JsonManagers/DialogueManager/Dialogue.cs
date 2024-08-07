using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StageType
{
    Dialogue,
    Memory,
    PopUp,
    Node,
    Letter
}

[System.Serializable]
public class Dialogue
{
    public StageType stageType;
    public string objectID;
    public string questID;
    public string speakerID;
    public int questStage;
    public string topicName;
    public string hint;
    public string backgroundID;

    public List<string> content;
    public List<DialogueEvent> dialogueEvents = new(); // new version of step with more data

    public RequirementPackage checks;
    //public List<IdIntPair> requirements = new();
    //public List<IdIntPair> restrictions = new();
    //public string locationID;
    //public float startTime = 0;
    //public float endTime = 0;
    //public int minimumDaysPassed;

    public bool disableAutoNode = false;
    public List<Choice> choices = new(); //LEAVE CHOICES BLANK TO LOOP DIALOGUE WITHOUT PROGRESSION
    public List<IdIntPair> taskTracking = new(); // For quests with multiple, non-chronological requirements that should be tracked in the journal. More responsive and situational than hints.
    public bool autoProgressStage = false; //automatically go to next stage when step is completed
    //public bool noLeaveButton = true; //if this is enabled, it is impossible to leave dialogue until you reach a choice

    public bool CheckRequirements()
    {
        return RequirementChecker.CheckDialogueRequirements(this);
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
            Debug.Log($"{dialogue}" + $"\n{dialogue.dialogueEvents.Count}");
        }
    }

    public static List<Dialogue> FindQuestDialogues(string questID)
    {
        return all
            .Where(d => d.questID == questID)
            .OrderBy(d => d.questStage)
            .ToList();
    }

    public static Dialogue FindByID(string dialogueID)
    {
        return all.FirstOrDefault(d => d.objectID == dialogueID);
    }
}