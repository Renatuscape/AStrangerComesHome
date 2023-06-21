using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class DialogueManager : MonoBehaviour
{
    //narrator object? Or disable name text if Character field is blank or something
    //log: store dialogue into a basic string along with all system messages to easily check back
    public DataManagerScript dataManager;
    public TransientDataScript transientData;

    public Character playerObject;
    public GameObject dialogueContainer;
    public GameObject skipButton;
    public GameObject autoPlayButton;
    public GameObject contentPrefab;
    public GameObject namePrefab;
    public GameObject choicePrefab; //Choice, leave and continue are identical in appearance. See if they can't be replaced by one prefab
    public GameObject leaveButton;
    public GameObject continueButton;

    public Quest quest;
    public Character previousSpeaker;
    public int dialogueIndex;
    public float autoPlaySpeed;
    public bool isAutoPlaying;

    public List<GameObject> printedDialogue;

    private void OnEnable()
    {
        playerObject.printName = dataManager.playerName;
        playerObject.nameColour = ColorUtility.TryParseHtmlString("#" + dataManager.playerNameColour, out Color color) ? color : new Color(0.6549f, 0.2196f, 0.498f);
        playerObject.NameSetup();
    }
    private void OnDisable()
    {
        previousSpeaker = null;
        isAutoPlaying = false;
        StopAllCoroutines();
        DestroyChoices();

        foreach (Transform child in dialogueContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SkipToChoice()
    {
        if (quest.dataValue < quest.dialogues.Count) //only skip when there is dialogue
        {
            for (int i = dialogueIndex; i < quest.dialogues[quest.dataValue].content.Count; i++)
            {
                DestroyChoices();
                PrintDialogue();
            }
        }
        else
            Debug.LogWarning("There is no dialogue to be skipped.");
    }

    public void AutoPlay()
    {
        StartCoroutine(AutoPlayDelayer());
    }
    IEnumerator AutoPlayDelayer()
    {
        if (quest.dataValue < quest.dialogues.Count) //only skip when there is dialogue
        {
            if (isAutoPlaying == true)
            {
                if (autoPlaySpeed > 0.2f)
                    autoPlaySpeed -= 0.3f; //if autoplay is already running, increase autoplay speed
            }
            else
            {
                isAutoPlaying = true;
                autoPlaySpeed = dataManager.autoPlaySpeed; //replace with a setting variable from dataManager

                if (dialogueIndex < quest.dialogues[quest.dataValue].content.Count-1)
                {
                    for (int i = dialogueIndex; i < quest.dialogues[quest.dataValue].content.Count; i++)
                    {
                        DestroyChoices();
                        PrintDialogue();
                        yield return new WaitForSeconds(autoPlaySpeed);

                        // Check if the dialogueIndex is still within the valid range
                        if (dialogueIndex >= quest.dialogues[quest.dataValue].content.Count - 1)
                        {
                            break; // Exit the loop if the index is out of range
                        }
                    }
                }

                isAutoPlaying = false;
            }
        }
        else
            Debug.LogWarning("There is no dialogue to autoplay.");
    }
    public void DestroyChoices() //Button function, must be public
    {
        dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = false;

        foreach (GameObject choice in printedDialogue)
        {
            Destroy(choice.gameObject);
        }
        printedDialogue.Clear();

        dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    public void PrintChoiceText(string choiceText) //Button function, must be public
    {
        var output = StringFormatter(choiceText);
        var choicePrint = Instantiate(contentPrefab);
        choicePrint.transform.SetParent(dialogueContainer.transform, false);
        var text = choicePrint.gameObject.transform.GetChild(0);
        text.GetComponent<TextMeshProUGUI>().text = "<color=#80654F>" + output + "</color>";
        previousSpeaker = playerObject;
    }

    void PrintSpeakerName(Character speaker) //Character nameplate with colour
    {
        var namePlate = Instantiate(namePrefab);
        namePlate.transform.SetParent(dialogueContainer.transform, false);
        var nameText = namePlate.gameObject.transform.GetChild(0);
        nameText.GetComponent<TextMeshProUGUI>().text = speaker.namePlate;
    }

    void PrintContent(string content) //Average text block for longer strings
    {
        var output = StringFormatter(content);
        var contPrefab = Instantiate(contentPrefab);
        contPrefab.transform.SetParent(dialogueContainer.transform, false);
        var dialogueText = contPrefab.gameObject.transform.GetChild(0);
        dialogueText.GetComponent<TextMeshProUGUI>().text = output;
    }
    void PrintLeaveButton() //Return to topics
    {
        var leavePrefab = Instantiate(leaveButton);
        var btn = leavePrefab.GetComponent<Button>();
        btn.interactable = false;
        leavePrefab.transform.SetParent(dialogueContainer.transform, false);
        leavePrefab.GetComponent<LeaveButton>().dialogueManager = gameObject;
        printedDialogue.Add(leavePrefab);
        StartCoroutine(ButtonActivator(btn));
        //create a button function that returns to topics instead of quitting dialogue?
    }

    void PrintContinueButton()
    {
        if (!isAutoPlaying)
        {
            var contBtn = Instantiate(continueButton);
            contBtn.transform.SetParent(dialogueContainer.transform, false);
            var btnComp = contBtn.GetComponent<Button>();
            btnComp.onClick.AddListener(DestroyChoices);
            btnComp.onClick.AddListener(PrintDialogue);
            printedDialogue.Add(contBtn);
        }
    }
    public void PrintDialogue()
    {
        dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = false;

        var dialogue = quest.dialogues[quest.dataValue]; //if out of range errors point here, make sure choice isn't set to a type of 'continue' when the next stage has no dialogue

        if (dialogueIndex >= 0 && dialogueIndex < dialogue.content.Count)
        {
            var speaker = dialogue.topicMaster;
            var content = dialogue.content[dialogueIndex];

            if (dialogue.speakers.Count > 0 && dialogueIndex < dialogue.speakers.Count) //if the list of speakers is not populated, default to topic master
            {
                speaker = dialogue.speakers[dialogueIndex];
            }

            //Debug.Log(content);

            //PRINT NAME
            if (speaker != previousSpeaker)
            {
                PrintSpeakerName(speaker);
            }
            previousSpeaker = speaker;

            PrintContent(content);

            dialogueIndex++;
        }

        if (dialogueIndex < dialogue.content.Count)
        {
            PrintContinueButton();
        }

        if (dialogueIndex == dialogue.content.Count) //index is higher than the amount of strings. Print choices
        {
            //PRINT CHOICES
            if (dialogue.choices.Count > 0)
            {
                //Debug.Log("Choices detected.");
                foreach (Choice c in dialogue.choices)
                {
                    if (c != null)
                    {
                        var choice = Instantiate(choicePrefab);
                        choice.transform.SetParent(dialogueContainer.transform, false);
                        choice.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = c.choiceText;
                        printedDialogue.Add(choice);

                        var btnChoice = choice.GetComponent<Button>();
                        btnChoice.interactable = false; //add a slight delay to the button availability
                        btnChoice.onClick.AddListener(DestroyChoices);
                        btnChoice.onClick.AddListener(() => PrintChoiceText(c.choiceText));
                        btnChoice.onClick.AddListener(() => ChoiceChecker(c));
                        StartCoroutine(ButtonActivator(btnChoice));
                    }
                    else
                        Debug.Log("Choice " + c + " in " + dialogue + " is null. Check list of choices.");
                }
            }
            if (!dialogue.noLeaveButton)
                PrintLeaveButton();
        }
        dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    public IEnumerator ButtonActivator (Button btn)
    {
        yield return new WaitForSeconds(0.4f);
        btn.interactable = true;
    }
    public void ChoiceChecker(Choice choice)
    {
        dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = false;

        var hasDelivery = false;
        var checkPassed = true;

        if (choice.moreThanObject.Count > 0)
        {
            for (int index = 0; index < choice.moreThanObject.Count; index++)
            {
                if (choice.moreThanObject[index].dataValue < choice.moreThanValue[index])
                {
                    checkPassed = false;
                    break;
                }
            }
        }

        if (choice.lessThanObject.Count > 0)
        {
            for (int index = 0; index < choice.lessThanObject.Count; index++)
            {
                if (choice.lessThanObject[index].dataValue > choice.lessThanValue[index])
                {
                    checkPassed = false;
                    break;
                }
            }
        }

        if (choice.deliveries.Count > 0)
        {
            hasDelivery = true;

            for (int index = 0; index < choice.deliveries.Count; index++)
            {
                if (choice.deliveries[index].dataValue < choice.deliveriesAmount[index])
                {
                    checkPassed = false;
                    break;
                } 
            }
        }

        if (hasDelivery && checkPassed)
        {
            for (int index = 0; index < choice.deliveries.Count; index++)
            {
                choice.deliveries[index].dataValue -= choice.deliveriesAmount[index];
                PrintChoiceText($"{choice.deliveries[index].printName} ({choice.deliveriesAmount[index]}) removed from my inventory.");
            }
        }

        if (choice.rewards.Count > 0 && checkPassed)
        {
            for (int index = 0; index < choice.rewards.Count; index++)
            {
                if (choice.rewards[index].dataValue + choice.rewardsAmount[index] < choice.rewards[index].maxValue)
                {
                    choice.rewards[index].dataValue += choice.rewardsAmount[index];

                    if (choice.rewards[index] is Character)
                    {
                        PrintChoiceText($"Increased {choice.rewards[index].printName}'s disposition +{choice.rewardsAmount[index]}.");
                    }
                    else if (choice.rewards[index] is Skill)
                    {
                        PrintChoiceText($"{choice.rewards[index].printName} skill +{choice.rewardsAmount[index]}!");
                    }
                    else if (choice.rewards[index] is Recipe)
                    {
                        PrintChoiceText($"Learned recipe for {choice.rewards[index].printName}.");
                    }
                    else
                        PrintChoiceText($"Gained {choice.rewards[index].printName} +{choice.rewardsAmount[index]}.");
                }
                else
                {
                    var rewardDecrease = choice.rewards[index].dataValue + choice.rewardsAmount[index] - choice.rewards[index].maxValue;
                    var newReward = choice.rewards[index].dataValue - rewardDecrease;

                    if (newReward > 0)
                    {
                        if (choice.rewards[index] is Character)
                        {
                            PrintChoiceText($"{choice.rewards[index].printName}'s disposition +{newReward}.\nMy bond with {choice.rewards[index].printName} feels deep.");
                        }
                        else if (choice.rewards[index] is Skill)
                        {
                            PrintChoiceText($"{choice.rewards[index].printName} skill increase +{newReward}.\nI have mastered {choice.rewards[index].printName}!");
                        }
                        else if (choice.rewards[index] is Recipe)
                        {
                            PrintChoiceText($"My understanding of the {choice.rewards[index].printName} recipe is complete.");
                        }
                        else
                            PrintChoiceText($"Gained {choice.rewards[index].printName} +{choice.rewardsAmount[index]}.\nMy inventory is full up, and I was unable to collect all {choice.rewardsAmount[index]}.");
                    }
                    else
                    {
                        if (choice.rewards[index] is Character)
                        {
                            PrintChoiceText($"My bond with {choice.rewards[index].printName} feels deep.");
                        }
                        else if (choice.rewards[index] is Skill)
                        {
                            PrintChoiceText($"I was unable to learn anything new about {choice.rewards[index].printName}. I have mastered this skill!");
                        }
                        else if (choice.rewards[index] is Recipe)
                        {
                            PrintChoiceText($"My understanding of the {choice.rewards[index].printName} recipe is complete.");
                        }
                        else
                            PrintChoiceText($"My inventory is full up, and I was unable to collect any {choice.rewards[index].printName}.");
                    }
                }
            }
        }

        if (checkPassed)
        {
            if (choice.succeededRequirementText == null || choice.succeededRequirementText == "")
            {
                Debug.Log("Check passed! SucceededRequirementText for " + choice.name + " equals null.");
            }
            else
            {
                PrintSpeakerName(quest.dialogues[quest.dataValue].topicMaster);
                PrintContent(choice.succeededRequirementText);
            }

            //FILTER CHOICE TYPE
            if (choice.choiceType == ChoiceType.LeaveOnly)
            {
                dialogueIndex = 0;
                PrintLeaveButton();
            }
            else if (choice.choiceType == ChoiceType.LoopDialogue)
            {
                dialogueIndex = 0;
                PrintContinueButton();
                PrintLeaveButton();
            }
            else if (choice.choiceType == ChoiceType.SetDialogueAndContinue)
            {
                dialogueIndex = choice.setDialogueStage;
                PrintContinueButton();
            }
            else if (choice.choiceType == ChoiceType.SetQuestAndContinue)
            {
                dialogueIndex = 0;
                quest.dataValue = choice.setQuestStage;
                PrintContinueButton();
            }
            else if (choice.choiceType == ChoiceType.SetQuestAndLeave)
            {
                dialogueIndex = 0;
                quest.dataValue = choice.setQuestStage;
                PrintLeaveButton();
            }
        }
        else
        {
            if (choice.failedRequirementText != null || choice.failedRequirementText != "")
            {
                PrintSpeakerName(quest.dialogues[quest.dataValue].topicMaster);
                PrintContent(choice.failedRequirementText);
            }
            else
            {
                Debug.Log("Check failed! FailedRequirementText for " + choice.name + " equals null.");
            }
            PrintLeaveButton();
        }

        dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    public string StringFormatter(string input)
    {

        // Define the tags and their corresponding replacements
        SerializableDictionary<string, string> tagReplacements = gameObject.GetComponent<DialogueTags>().tagDictionary;

        string pattern = "\\|([^|]+)\\|"; // Regex pattern to match the tags

        string result = Regex.Replace(input, pattern, match =>
        {
            string tag = match.Groups[1].Value; // Extract the tag from the match
            if (tagReplacements.TryGetValue(tag, out string replacement))
                return replacement;
            else
                return match.Value; // No replacement found, keep the original tag
        });

        return result;
    }
}