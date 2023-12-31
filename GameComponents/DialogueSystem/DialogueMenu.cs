using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMenu : MonoBehaviour
{
    public RectMask2D containerMask;
    public GameObject bgReplacer;
    public GameObject dialogueContainer;
    public PortraitRenderer portraitRenderer; //remember to use .gameObject for the object
    public Image dialogueContainerBG;
    public DialogueSystem dialogueSystem;
    public List<GameObject> buttonList;
    public List<GameObject> printedChoices;
    public DialogueChoiceHandler choiceHandler = new();
    public DialogueButtonFactory buttonFactory = new();

    public int stepIndex = 0;

    private void Awake()
    {
        dialogueContainerBG = dialogueContainer.GetComponent<Image>();
        choiceHandler.dialogueMenu = this;
        buttonFactory.dialogueMenu = this;
    }

    private void OnEnable()
    {
        stepIndex = 0;
        foreach (GameObject step in buttonList)
        {
            Destroy(step.gameObject);
        }
        buttonList = new();
        SetValuesToDefault();
    }

    private void OnDisable()
    {
        foreach (GameObject step in buttonList)
        {
            Destroy(step.gameObject);
        }
        buttonList = new();
        stepIndex = 0;
        SetValuesToDefault();
    }
    void Update()
    {
        if (dialogueContainer.GetComponent<RectTransform>().rect.height > 389 && bgReplacer.activeInHierarchy == false)
        {
            EnableReplacerBG();
        }
        else if (dialogueContainer.GetComponent<RectTransform>().rect.height < 389 && bgReplacer.activeInHierarchy == true)
        {
            SetValuesToDefault();
        }
    }


    public void StartDialogueStage(Quest quest)
    {
        stepIndex = 0;
        portraitRenderer.EnableForDialogue();
        Dialogue dialogue = GetDialogueStage(quest);
        PrintStep(dialogue);
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

    //PRINT NEXT STEP
    public void PrintStep(Dialogue dialogue)
    {
        dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = false;

        if (stepIndex < dialogue.dialogueSteps.Count)
        {
            DialogueStep step = dialogue.dialogueSteps[stepIndex];

            //MANAGE TEXT
            if (step.speaker.dialogueTag != "Narration")
            {
                portraitRenderer.SetRightSprite(step.speaker, true);
                buttonFactory.PrintNamePlate(step.speaker);
                buttonFactory.PrintText(step.text);
            }
            else
            {
                buttonFactory.PrintNarration(step.text);
            }


            //MANAGE CONTINUE BUTTON
            if (stepIndex + 1 < dialogue.dialogueSteps.Count)
            {
                buttonFactory.PrintContinue(dialogue);
            }

            //MANAGE CHOICES AND LEAVE BUTTON
            if (stepIndex + 1 == dialogue.dialogueSteps.Count)
            {
                if (dialogue.choices is not null && dialogue.choices.Count > 0)
                {
                    choiceHandler.PrintChoices(dialogue);
                }
                else if (dialogue.choices is null || dialogue.choices.Count == 0 || !dialogue.noLeaveButton)
                {
                    buttonFactory.PrintLeaveButton();
                }
            }
        }
        else if (stepIndex == dialogue.dialogueSteps.Count)
        {
            //Forcibly end dialogue if step-index exceeds dialogueSteps
            dialogueSystem.CloseDialogueMenu();
        }
        else
        {
            Debug.Log("Error with PrintStep index or choices");
        }

        dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
        dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
        dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    public void ContinueToNextStep(Dialogue dialogue, GameObject button)
    {
        stepIndex++;
        PrintStep(dialogue);
        Destroy(button);
    }

    //HANDLE LAYOUT
    void SetValuesToDefault()
    {
        bgReplacer.SetActive(false);
        dialogueContainerBG.enabled = true;
        containerMask.enabled = false;
    }

    void EnableReplacerBG()
    {
        bgReplacer.SetActive(true);
        dialogueContainerBG.enabled = false;
        containerMask.enabled = true;
    }
}
