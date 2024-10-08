﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceManager : MonoBehaviour
{
    public DialogueMenu dialogueMenu;
    public GameObject choiceContainer;

    public float minimumDisplaySize = 245;

    private void OnEnable()
    {
        foreach (Transform child in choiceContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void PrintChoices(Dialogue dialogue)
    {
        gameObject.SetActive(true);


        foreach (Choice choice in dialogue.choices)
        {
            bool allowPrint = true;

            if (choice.hiddenOnFail)
            {
                allowPrint = choice.AttemptAllChecks(false, out var missingItems);
                Debug.Log($"Option {choice.optionText} returned {allowPrint}.");
            }

            if (allowPrint)
            {
                choiceContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
                Canvas.ForceUpdateCanvases();

                var newChoice = BoxFactory.CreateButton(choice.optionText);
                newChoice.transform.SetParent(choiceContainer.transform, false);

                newChoice.GetComponent<Button>().onClick.AddListener(() => MakeChoice(choice));
                newChoice.GetComponentInChildren<TextMeshProUGUI>().fontSize = 30;

                choiceContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
                Canvas.ForceUpdateCanvases();
            }
        }

        if (choiceContainer.transform.childCount == 0 && dialogue.choices.Count > 0)
        {
            Debug.LogWarning("No choices were available even though choices exist. Check " + dialogueMenu.activeQuest.objectID + " options and ensure that there is always a non-hidden option.");
            PrintDummyButton();
        }

        choiceContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
        choiceContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    public void MakeChoice(Choice choice)
    {
        bool choiceResult = choice.AttemptAllChecks(true, out List<IdIntPair> missingItems);

        dialogueMenu.HandleChoiceResult(choice, choiceResult, missingItems);

        foreach (Transform child in choiceContainer.transform)
        {
            Destroy(child.gameObject);
        }

        if (choice.gate != null && !string.IsNullOrEmpty(choice.gate.destinationRegion))
        {
            if (choice.gateOnFailAndSuccess ||
                (choiceResult && !choice.gateOnFailOnly)
                || (!choiceResult && choice.gateOnFailOnly))
            {
                TransientDataScript.TravelByGate(choice.gate);
            }
        }

        gameObject.SetActive(false);
    }

    void PrintDummyButton()
    {
        Choice leaveChoice = new() { optionText = "No options available. Leave conversation.", endConversation = true };
        choiceContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
        Canvas.ForceUpdateCanvases();

        var newChoice = BoxFactory.CreateButton(leaveChoice.optionText);
        newChoice.transform.SetParent(choiceContainer.transform, false);

        newChoice.GetComponent<Button>().onClick.AddListener(() => MakeChoice(leaveChoice));

        choiceContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }
}