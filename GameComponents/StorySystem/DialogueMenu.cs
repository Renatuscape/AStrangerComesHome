using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        Dialogue dialogue = GetDialogue(activeQuest);

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
        Dialogue dialogue = GetDialogue(activeQuest);

        if (dialogue != null)
        {
            dialogueDisplay.StartDialogue(dialogue);
        }
        else
        {
            Debug.Log("Attempted to continue after choice, but target stage does not exist. Ensure that dialogue is set to end conversation, or that next stage exists.");
            EndDialogue();
        }
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

    Dialogue GetDialogue(Quest quest)
    {
        int stage = Player.GetCount(quest.objectID, "DialogueMenu");
        return quest.dialogues.FirstOrDefault(d => d.questStage == stage);
    }

    void ReturnToOverWorld()
    {
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }
}
