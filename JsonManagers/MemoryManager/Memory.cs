using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class Memory : BaseObject
{
    public bool isUnique;
    public Sprite sprite;
    public Dialogue dialogue;

    public void Initialise()
    {
        if (objectID.Contains("-UNI"))
        {
            isUnique = true;
            objectID = objectID.Split('-')[0];
        }

        objectType = ObjectType.Memory;
        dialogue.objectID = objectID;
        dialogue.speakerID = "MEMORY";
        DialogueSetup.InitialiseDialogue(dialogue);
       
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