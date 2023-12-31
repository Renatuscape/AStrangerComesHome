using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceHandler
{
    public DialogueMenu dialogueMenu;
    public Dialogue activeDialogue;
    public Quest activeQuest;

    //CHOICE HANDLING
    public GameObject[] PrintChoices(Dialogue dialogue)
    {
        activeDialogue = dialogue;

        foreach (Choice choice in dialogue.choices)
        {
            GameObject button = dialogueMenu.buttonFactory.InstantiateBasicButton();
            dialogueMenu.printedChoices.Add(button);
            button.GetComponent<Button>().onClick.AddListener(() => ChoiceOnClick(choice, this));

            var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            textMesh.alignment = TextAlignmentOptions.Left;
            textMesh.text = choice.optionText;
        }
        return null;
    }

    public void HandleClick(Choice choice)
    {
        dialogueMenu.stepIndex = 0;

        bool passedChecks = choice.AttemptAllChecks(true, out bool passedRequirements, out bool passedRestrictions, out var missingItems);

        dialogueMenu.dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = false;

        dialogueMenu.buttonFactory.PrintNamePlate(Characters.FindByTag("Traveller", "DialogueMenu HanleClick.PrintNamePlate choice"));
        dialogueMenu.buttonFactory.PrintText("<i>" + choice.optionText + "</i>");

        if (passedChecks)
        {
            DestroyChoices();

            if (!string.IsNullOrEmpty(choice.successSpeaker))
            {
                dialogueMenu.buttonFactory.PrintNamePlate(Characters.FindByTag(choice.successSpeaker, "DialogueMenu HanleClick.PrintNamePlate success"));
                dialogueMenu.buttonFactory.PrintText(choice.successText);
            }


            if (!choice.doesNotAdvance)
            {
                activeQuest = Quests.FindByID(activeDialogue.questID);
                activeQuest.SetQuestStage(choice.advanceTo);
            }

            if (!choice.endConversation && activeQuest.dialogues[choice.advanceTo].stageType == StageType.Dialogue)
            {
                dialogueMenu.PrintStep(activeQuest.dialogues[choice.advanceTo]);
            }
            else
            {
                dialogueMenu.buttonFactory.PrintLeaveButton();
            }
        }
        else
        {
            DestroyChoices();
            if (!string.IsNullOrEmpty(choice.failureSpeaker))
            {
                dialogueMenu.buttonFactory.PrintNamePlate(Characters.FindByTag(choice.failureSpeaker, "DialogueMenu HanleClick.PrintNamePlate failure"));
                dialogueMenu.buttonFactory.PrintText(choice.failureText);
            }

            PrintMissingItems(missingItems);
            dialogueMenu.buttonFactory.PrintLeaveButton();
        }

        dialogueMenu.dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
        dialogueMenu.dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
        dialogueMenu.dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    void PrintMissingItems(List<IdIntPair> missingItems)
    {
        Debug.Log("Attempting to print missing items");
        if (missingItems.Count > 0)
        {
            dialogueMenu.buttonFactory.PrintNarration("I should return when I find another...");
            foreach (IdIntPair entry in missingItems)
            {
                dialogueMenu.buttonFactory.PrintNarration($" - {GameCodex.GetNameFromID(entry.objectID)} ({entry.amount})");
            }
        }

    }

    //CleanUp
    public void DestroyChoices()
    {
        foreach(GameObject choice in dialogueMenu.printedChoices)
        {
            Object.Destroy(choice);
        }
        dialogueMenu.printedChoices = new();
    }


    //BUTTON METHOD
    public void ChoiceOnClick(Choice choice, DialogueChoiceHandler choiceHandler)
    {
        choiceHandler.HandleClick(choice);
    }
}