using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class Memory : BaseObject
{
    public bool isUnique; // Unique memories cannot be lost, nor can you randomly find extra copies anywhere
    public bool contentWarning; // If this is flagged, bring up a pop-up warning and print the DESCRIPTION before proceeding to dialogue reader
    public Dialogue dialogue;

    public void Initialise()
    {
        if (isUnique)
        {
            maxValue = 1;
        }
        else
        {
            maxValue = 3;
        }

        objectType = ObjectType.Memory;
        dialogue.stageType = StageType.Memory;
        dialogue.objectID = objectID + "-D";
        dialogue.questID = objectID + "-Q";
        dialogue.speakerID = "MEMORY";

        if (string.IsNullOrEmpty(dialogue.topicName))
        {
            dialogue.topicName = name;
        }

        dialogue.dialogueEvents = DialogueSetup.CreateEventsFromStringList(dialogue.content);
       
        if (dialogue.choices == null || dialogue.choices.Count == 0)
        {
            dialogue.choices = new()
            {
                new Choice()
                {
                    endConversation = true,
                    optionText = "End"
                }
            };
        }
        else
        {
            foreach (var choice in dialogue.choices)
            {
                choice.endConversation = true;
            }
        }

        // Set sprite
    }
}

public static class Memories
{
    public static List<Memory> all = new();

    public static void DebugList()
    {
        Debug.LogWarning("Memories.DebugList() called");

        foreach (Memory memory in all)
        {
            Debug.Log($"Memory ID: {memory.objectID}\tMemory Name: {memory.name}");
        }
    }

    public static void DebugAllMemories()
    {
        foreach (Memory memory in all)
        {
            Player.Add(memory.objectID, 25, true);
        }
    }

    public static Memory FindByID(string searchWord)
    {
        return all.FirstOrDefault(s => s.objectID == searchWord);
    }

    public static int GetMax(string searchWord)
    {
        foreach (Memory memory in all)
        {
            if (memory.objectID == searchWord)
            {
                return memory.maxValue;
            }
        }
        return 3;
    }
}