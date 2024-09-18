using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueMenu : MonoBehaviour
{  
    public StorySystem storySystem;
    public DeathManager deathManager;
    public DialogueDisplay dialogueDisplay;
    public DialogueChoiceManager choiceManager;
    public DialogueBackgroundManager backgroundManager;

    public Quest activeQuest;
    public string initiatingNPC;
    public bool doNotReopenTopic = false;
    public bool memoryMode = true;
    private void OnEnable()
    {
        choiceManager.gameObject.SetActive(false);
    }

    public void StartDialogue(Quest quest, string speakerID, bool doNotReopenTopic, bool clearBackground)
    {
        memoryMode = false;

        if (clearBackground)
        {
            backgroundManager.ClearBackground();
        }
        this.doNotReopenTopic= doNotReopenTopic;
        choiceManager.gameObject.SetActive(false);
        initiatingNPC = speakerID;
        activeQuest = quest;

        Dialogue dialogue = GetDialogue(activeQuest);

        dialogueDisplay.StartDialogue(dialogue, true);
    }

    public void StartMemory(Memory memory)
    {
        memoryMode = true;
        activeQuest = null;
        doNotReopenTopic = true;
        backgroundManager.ClearBackground();
        choiceManager.gameObject.SetActive(false);

        dialogueDisplay.StartDialogue(memory.dialogue, true);
    }

    public void SetUpBackground(string backgroundID)
    {
        backgroundManager.SetUpBackground(backgroundID);
    }

    public void PrintChoices(Dialogue dialogue)
    {
        //choiceManager.PrintChoices(dialogue);
        StartCoroutine(PrintChoicesWithDelay(dialogue));
    }

    IEnumerator PrintChoicesWithDelay(Dialogue dialogue)
    {
        yield return new WaitForSeconds(0.5f);
        choiceManager.PrintChoices(dialogue);
    }

    public void HandleChoiceResult(Choice choice, bool isSuccess, List<IdIntPair> missingItems)
    {
        if (!memoryMode && isSuccess)
        {
            Player.SetQuest(activeQuest.objectID, choice.advanceTo);
        }

        if (!memoryMode && !isSuccess && choice.advanceToOnFailure > -1)
        {
            Debug.Log("Choice failed, and choice had valid advanceToOnFailure value.");
            Player.SetQuest(activeQuest.objectID, choice.advanceToOnFailure);
        }

        choiceManager.gameObject.SetActive(false);

        if ((choice.dieOnFailure && !isSuccess) || (choice.dieOnSuccess && isSuccess))
        {
            deathManager.InitiateDeath(choice, isSuccess);
            dialogueDisplay.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            dialogueDisplay.PrintChoiceResult(choice, isSuccess, missingItems);
        }
    }

    public void ContinueAfterChoice()
    {
        Dialogue dialogue = GetDialogue(activeQuest);

        if (dialogue != null)
        {
            dialogueDisplay.StartDialogue(dialogue, false);
        }
        else
        {
            Debug.Log("Attempted to continue after choice, but target stage does not exist. Ensure that dialogue is set to end conversation, or that next stage exists.");
            EndDialogue(null);
        }
    }

    public void StartNextStageForAutoPlay()
    {
        Dialogue dialogue = GetDialogue(activeQuest);

        if (dialogue != null)
        {
            dialogueDisplay.StartDialogue(dialogue, false);
        }
        else
        {
            Debug.Log("Auto-play tried to advance to next dialogue step, but no step was found.");
        }
    }

    public void EndDialogue(Choice choice)
    {
        Debug.Log("DIALOGUE: ENDING DIALOGUE.");

        if (choice != null && choice.nodeData != null && choice.nodeData.removeSpeakerNode)
        {
            var speaker = choice.nodeData.nodeID;
            if (string.IsNullOrEmpty(speaker))
            {
                Debug.LogWarning("Set speaker ID for " + choice.optionText + " to use removeSpeakerNode feature.");
            }

            Debug.Log("DIALOGUE: Attempting to disable node.");
            WorldNodeTracker.DisableNodeWithFade(speaker);
        }

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
        Debug.Log("DiaMenu called ReturnToOverworld().");
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }

}
