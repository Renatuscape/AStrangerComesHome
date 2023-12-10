using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string objectID;
    public string questID;
    public int stage;
    public string topicName;
    public List<DialogueContent> dialogueContent;
}

public static class Dialogues
{
    public static List<Dialogue> all = new();

    public static void DebugList()
    {
        Debug.LogWarning("Dialogue.DebugList() called");

        foreach (Dialogue dialogue in all)
        {
            Debug.Log($"Dialogue for quest {dialogue.questID}: stage {dialogue.stage}");
        }
    }

    public static void DebugAllItems()
    {
        foreach (Dialogue dialogue in all)
        {
            Debug.Log($"{dialogue}" +
                $"\n{dialogue.dialogueContent.Count}");
        }
    }

    public static List<Dialogue> FindQuestDialogues(string questID)
    {
        return all
            .Where(d => d.questID == questID)
            .OrderBy(d => d.stage)
            .ToList();
    }
}