using System.Collections.Generic;
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
            choiceContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
            Canvas.ForceUpdateCanvases();

            var newChoice = BoxFactory.CreateButton(choice.optionText);
            newChoice.transform.SetParent(choiceContainer.transform, false);

            newChoice.GetComponent<Button>().onClick.AddListener(() => MakeChoice(choice));

            choiceContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();
        }

        choiceContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
        choiceContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    public void MakeChoice(Choice choice)
    {

        bool choiceResult = choice.AttemptAllChecks(true, out bool passedRequirements, out bool passedRestrictions, out List<IdIntPair> missingItems);
        Debug.Log($"Choice resulted in {choiceResult}." +
            $"\nRequirements passed: {passedRequirements}." +
            $"\nRestrictions passed: {passedRestrictions}." +
            $"\nMissing item count: {missingItems.Count}");

        dialogueMenu.HandleChoiceResult(choice, choiceResult, missingItems);

        foreach (Transform child in choiceContainer.transform)
        {
            Destroy(child.gameObject);
        }

        gameObject.SetActive(false);
    }
}