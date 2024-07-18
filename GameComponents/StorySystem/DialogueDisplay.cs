using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueDisplay : MonoBehaviour
{
    public DialogueMenu dialogueMenu;
    public DialoguePrinter printer;
    public DialoguePortraitManager portraitManager;

    public AudioSource textSoundEffect;

    public GameObject dialogueContainer;

    public GameObject leftNameDisplay;
    public GameObject rightNameDisplay;

    public TextMeshProUGUI chatHistory;

    public TextMeshProUGUI contentText;
    public TextMeshProUGUI leftNameText;
    public TextMeshProUGUI rightNameText;

    public Button btnAutoPlay;
    public bool waitingForChoice;

    public Dialogue activeDialogue;
    public DialogueEvent activeEvent;
    public bool continueEnabled;
    public bool isPrinting;
    public bool autoEnabled;
    public bool endConversation;
    public bool readyToPrintChoices;
    public bool continueAfterChoice;
    public int eventIndex;

    public float autoDelay = 2;
    public float autoTimer;

    bool waitingForAutoPlayCoroutine = false;
    private void Start()
    {
        btnAutoPlay.onClick.AddListener(() => ToggleAuto());
        chatHistory.text = "<b>Conversation History</b>\n";
        autoDelay = 4.5f - GlobalSettings.TextSpeed;
        printer.Initialise(this);
    }

    private void Update()
    {
        if (autoEnabled && !isPrinting && continueEnabled && !waitingForChoice)
        {
            autoTimer += Time.deltaTime;
            autoDelay = 4.5f - GlobalSettings.TextSpeed;

            if (autoTimer > autoDelay)
            {
                if (eventIndex < activeDialogue.dialogueEvents.Count)
                {
                    autoTimer = 0;
                    PrintEvent();
                }
            }
        }

        if (readyToPrintChoices && !isPrinting)
        {
            dialogueMenu.PrintChoices(activeDialogue);

            readyToPrintChoices = false;
            continueEnabled = false;
            continueAfterChoice = false;
            waitingForChoice = true;
        }
    }

    IEnumerator StartNextStageForAutoPlay()
    {
        waitingForAutoPlayCoroutine = true;
        yield return new WaitForSeconds(autoDelay);
        autoTimer = 0;
        dialogueMenu.StartNextStageForAutoPlay();
        waitingForAutoPlayCoroutine = false;
    }

    public void StartDialogue(Dialogue dialogue, bool isInitialStep)
    {
        if (textSoundEffect.clip == null)
        {
            textSoundEffect.clip = AudioManager.GetSoundEffect("knockSmall");
        }

        textSoundEffect.volume = GlobalSettings.EffectVolume - 0.1f;

        if (dialogue.stageType != StageType.Dialogue)
        {
            Debug.LogWarning("Attempted to start a non-dialogue event. Was the choice leading to " + dialogue.objectID + " missing endConversation: true?");
            Debug.LogWarning("Ending dialogue early.");
            dialogueMenu.EndDialogue(null);
        }
        else
        {
            continueEnabled = false;
            endConversation = false;
            readyToPrintChoices = false;
            continueAfterChoice = false;
            isPrinting = false;
            waitingForChoice = false;

            gameObject.SetActive(true);

            activeDialogue = dialogue;
            eventIndex = 0;

            if (isInitialStep)
            {
                dialogueMenu.SetUpBackground(dialogue.backgroundID);

                var titleText = dialogue.topicName;
                if (string.IsNullOrEmpty(titleText))
                {
                    titleText = Quests.FindByID(dialogue.questID).name;
                }

                PrintToChatLog(DialogueTagParser.ParseText(titleText).ToUpper(), false, false);
            }
            else
            {
                if (!string.IsNullOrEmpty(dialogue.backgroundID))
                {
                    dialogueMenu.SetUpBackground(dialogue.backgroundID);
                }
            }

            PrintEvent();
        }
    }

    public void PrintEvent()
    {
        activeEvent = activeDialogue.dialogueEvents[eventIndex];
        SetDisplayNames(activeEvent);

        if (!string.IsNullOrEmpty(activeEvent.backgroundID))
        {
            dialogueMenu.SetUpBackground(activeEvent.backgroundID);
        }

        // Parse tags here instead of at start to get latest tags
        var parsedText = DialogueTagParser.ParseText(activeEvent.content);
        PrintContent(parsedText, activeEvent.speaker.objectID == "ARC999");
        chatHistory.text += parsedText + "\n";

        eventIndex++;

        if (TransientDataScript.isDemoEnabled && eventIndex >= 3)
        {
            eventIndex = activeDialogue.dialogueEvents.Count;
        }

        if (eventIndex >= activeDialogue.dialogueEvents.Count)
        {
            continueEnabled = false;


            if (activeDialogue.choices == null || activeDialogue.choices.Count == 0)
            {
                endConversation = true;
            }
            else
            {
                readyToPrintChoices = true;
            }
        }
        else
        {
            continueEnabled = true;
        }

        Canvas.ForceUpdateCanvases();
        portraitManager.StartDialogueEvent(activeEvent);
    }

    public void PrintChoiceResult(Choice choice, bool isSuccess, List<IdIntPair> missingItems)
    {
        autoTimer = 0;
        string speakerTag = isSuccess ? choice.successSpeaker : choice.failureSpeaker;
        bool hasResultText = !string.IsNullOrEmpty(speakerTag);
        bool hasMissingItemsToPrint = missingItems != null && missingItems.Count > 0;

        PrintToChatLog("Choice: " + choice.optionText, true, true);

        // HANDLE RESULT PRINT
        if (hasResultText) // if there is no speaker, skip the print
        {
            DialogueEvent resultEvent = new();

            // Parse speaker event data here if it exists

            resultEvent.speaker = Characters.FindByTag(speakerTag, name);

            if (resultEvent.speaker == null)
            {
                Debug.LogWarning("Something was wrong with speaker ID for choice in " + activeDialogue.objectID + ". Did you use objectID instead of tag?");
            }
            else
            {
                string content = isSuccess ? choice.successText : choice.failureText;

                SetDisplayNames(resultEvent);

                if (resultEvent.speaker.objectID != "ARC000" && resultEvent.speaker.objectID != "ARC999")
                {
                    portraitManager.SetRightPortrait(resultEvent.speaker.objectID);
                }

                var parsedText = DialogueTagParser.ParseText(content);
                PrintContent(parsedText, speakerTag == "Narration");
                PrintToChatLog(resultEvent.speaker.NamePlate(), true, false);
                PrintToChatLog(parsedText, false, false);
            }
        }

        // HANDLE MISSING ITEM PRINT
        if (hasMissingItemsToPrint)
        {
            foreach (var entry in missingItems)
            {
                Debug.Log($"Missing {entry.amount} {entry.objectID}. Print this for the player somehow.");

                BaseObject missingType = GameCodex.GetBaseObject(entry.objectID);

                if (missingType.objectType == ObjectType.Item)
                {
                    Item item = (Item)missingType;
                    PrintToChatLog($"\nMissing {entry.amount}x {item.name}.", false, false);
                }

            }
        }

        // HANDLE CLOSING OR CONTINUING

        if (isSuccess)
        {
            if (choice.endConversation)
            {
                endConversation = true;

                if (!hasResultText && !hasMissingItemsToPrint)
                {
                    dialogueMenu.EndDialogue(choice);
                }
            }
            else
            {
                endConversation = false;

                if (!hasResultText && !hasMissingItemsToPrint)
                {
                    dialogueMenu.ContinueAfterChoice();
                }
                else
                {
                    if (autoEnabled)
                    {
                        StartCoroutine(StartNextStageForAutoPlay());
                    }

                    continueAfterChoice = true;
                }
            }
        }
        else
        {
            if (choice.advanceToOnFailure >= 0 && !choice.endConversationOnFailure)
            {
                endConversation = false;

                if (!hasResultText && !hasMissingItemsToPrint)
                {
                    dialogueMenu.ContinueAfterChoice();
                }
                else
                {
                    continueAfterChoice = true;
                }
            }
            else
            {
                endConversation = true;

                if (!hasResultText && !hasMissingItemsToPrint)
                {
                    dialogueMenu.EndDialogue(choice);
                }
            }
        }

        waitingForChoice = false;
        Debug.Log($"Speaker was {speakerTag} and whether it has text returned {hasResultText}.");
    }

    void PrintContent(string textToPrint, bool isNarration)
    {
        if (!printer.readyToPrint)
        {
            printer.Initialise(this);
        }

        printer.StartPrint(textToPrint, isNarration);
    }

    void SetDisplayNames(DialogueEvent dEvent)
    {
        if (dEvent.speaker.objectID == "ARC999")
        {
            rightNameDisplay.gameObject.SetActive(false);
            leftNameDisplay.gameObject.SetActive(false);
        }
        else
        {
            if (dEvent.isLeft || dEvent.speaker.objectID == "ARC000")
            {
                if (dEvent.speaker.objectID == "ARC000" && GlobalSettings.AlwaysTrueNamePlate)
                {
                    leftNameText.text = dEvent.speaker.ForceTrueNamePlate();
                }
                else
                {
                    leftNameText.text = dEvent.speaker.NamePlate();
                }

                leftNameDisplay.gameObject.SetActive(true);
                rightNameDisplay.gameObject.SetActive(false);
            }
            else
            {
                rightNameText.text = dEvent.speaker.NamePlate();
                rightNameDisplay.SetActive(true);
                leftNameDisplay.gameObject.SetActive(false);
            }

            PrintToChatLog(dEvent.speaker.NamePlate(), true, false);
        }
    }

    public void Continue()
    {
        if (!autoEnabled && continueEnabled && !isPrinting)
        {
            PrintEvent();
        }
        else if (!autoEnabled && isPrinting)
        {
            //AudioManager.PlayAmbientSound("smallSnap");
            printer.printSpeed = 0;
        }
        else if (endConversation)
        {
            dialogueMenu.EndDialogue(null);
        }
        else if (continueAfterChoice && !isPrinting)
        {
            dialogueMenu.ContinueAfterChoice();
        }
    }

    public void ToggleAuto()
    {
        if (!waitingForAutoPlayCoroutine)
        {
            autoEnabled = !autoEnabled;

            if (autoEnabled)
            {
                autoTimer = autoDelay / 2;
                btnAutoPlay.image.color = Color.gray;
            }
            else
            {
                autoTimer = 0;
                btnAutoPlay.image.color = Color.white;
            }
        }
    }

    public void PrintToChatLog(string text, bool spaceBefore, bool italics = false)
    {
        if (spaceBefore)
        {
            chatHistory.text += "\n";
        }

        Canvas.ForceUpdateCanvases();

        if (italics)
        {
            chatHistory.text += "<i>";
        }

        Canvas.ForceUpdateCanvases();

        chatHistory.text += text;


        if (italics)
        {
            chatHistory.text += "</i>";
        }

        chatHistory.text += "\n";

        chatHistory.gameObject.GetComponent<ContentSizeFitter>().enabled = false;
        chatHistory.gameObject.GetComponent<ContentSizeFitter>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }
}
