using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMenu : MonoBehaviour
{  
    public StorySystem storySystem;
    public DialogueDisplay dialogueDisplay;
    public DialogueChoiceManager choiceManager;

    public Quest activeQuest;
    public string initiatingNPC;

    private void OnEnable()
    {
        choiceManager.gameObject.SetActive(false);
    }

    public void StartDialogue(Quest quest, string speakerID)
    {
        choiceManager.gameObject.SetActive(false);
        initiatingNPC = speakerID;
        activeQuest = quest;
        Dialogue dialogue = GetDialogueStage(activeQuest);
        dialogueDisplay.StartDialogue(dialogue);
    }

    public void PrintChoices(Dialogue dialogue)
    {
        choiceManager.PrintChoices(dialogue);
    }


    public void HandleChoiceResult(Choice choice, bool isSuccess, List<IdIntPair> missingItems)
    {
        Player.Set(activeQuest.objectID, choice.advanceTo);

        choiceManager.gameObject.SetActive(false);
        dialogueDisplay.PrintChoiceResult(choice, isSuccess, missingItems);
    }

    public void ContinueAfterChoice()
    {
        Dialogue dialogue = GetDialogueStage(activeQuest);
        dialogueDisplay.StartDialogue(dialogue);
    }

    public void EndDialogue()
    {
        dialogueDisplay.gameObject.SetActive(false);
        storySystem.OpenTopicMenu(initiatingNPC);
    }

    Dialogue GetDialogueStage(Quest quest)
    {
        if (Player.GetEntry(quest.objectID, "TopicMenu", out var entry))
        {
            return quest.dialogues[entry.amount];
        }
        else
        {
            return quest.dialogues[0];
        }
    }
}
