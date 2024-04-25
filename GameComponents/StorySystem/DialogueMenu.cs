using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMenu : MonoBehaviour
{  
    public StorySystem storySystem;
    public DialogueDisplay dialogueDisplay;
    public DialogueChoiceManager choiceManager;

    public Quest activeQuest;
    public string initiatingNPC;
    public bool doNotReopenTopic = false;

    private void OnEnable()
    {
        choiceManager.gameObject.SetActive(false);
    }

    public void StartDialogue(Quest quest, string speakerID, bool doNotReopenTopic)
    {
        this.doNotReopenTopic= doNotReopenTopic;
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
        if (isSuccess)
        {
            Player.Set(activeQuest.objectID, choice.advanceTo);
        }
        if (!isSuccess && choice.advanceToOnFailure > -1)
        {
            Player.Set(activeQuest.objectID, choice.advanceToOnFailure);
        }

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

        if (!doNotReopenTopic)
        {
            storySystem.ReopenTopicsAfterDialogue(initiatingNPC);
        }
        else
        {
            gameObject.SetActive(false);
            Invoke(nameof(ReturnToOverWorld), 1);
        }
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

    void ReturnToOverWorld()
    {
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }
}
