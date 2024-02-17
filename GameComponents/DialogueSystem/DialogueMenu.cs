using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMenu : MonoBehaviour
{  
    public DialogueSystem dialogueSystem;
    public PortraitRenderer portraitRenderer; //remember to use .gameObject for the object

    public GameObject dialogueContainer;
    public Button autoPlay;
    public Button skip;
    public Image dialogueContainerBG;
    public List<GameObject> buttonList;
    public List<GameObject> continueButtons;
    public List<GameObject> printedChoices;
    public DialogueChoiceHandler choiceHandler = new();
    public DialogueButtonFactory buttonFactory = new();

    public int stepIndex = 0;

    public float autoPlaySpeed = 2;
    public bool isAutoPlaying;
    public bool leavePrinted;

    private void Awake()
    {
        dialogueContainerBG = dialogueContainer.GetComponent<Image>();
        choiceHandler.dialogueMenu = this;
        buttonFactory.dialogueMenu = this;
    }

    private void OnEnable()
    {
        leavePrinted = false;
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


    public void StartDialogueStage(Quest quest)
    {
        leavePrinted = false;
        stepIndex = 0;
        portraitRenderer.EnableForDialogue();
        Dialogue dialogue = GetDialogueStage(quest);

        PrintStep(dialogue);

        skip.onClick.AddListener(()=> Skip(dialogue));
        autoPlay.onClick.AddListener(() => AutoPlay(dialogue));
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
    public void PrintStep(Dialogue dialogue, bool hasContinueButton = true)
    {
        foreach (DialogueStep step in dialogue.dialogueSteps)
        {
            step.text = DialogueTagParser.ParseText(step.text);
        }

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
            if (stepIndex + 1 < dialogue.dialogueSteps.Count && hasContinueButton)
            {
                GameObject button = buttonFactory.PrintContinue(dialogue);
                continueButtons.Add(button);
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


        //Ensure buttons use latest dialogue stage
        skip.onClick.RemoveAllListeners();
        autoPlay.onClick.RemoveAllListeners();
        skip.onClick.AddListener(() => Skip(dialogue));
        autoPlay.onClick.AddListener(() => AutoPlay(dialogue));
    }

    public void ContinueToNextStep(Dialogue dialogue, GameObject button)
    {
        stepIndex++;
        PrintStep(dialogue);
        Destroy(button);
    }

    void SetValuesToDefault()
    {
        skip.onClick.RemoveAllListeners();
        autoPlay.onClick.RemoveAllListeners();
    }

    //Speed controls
    public void Skip(Dialogue dialogue)
    {
        isAutoPlaying = false;
        while (stepIndex < dialogue.dialogueSteps.Count && !leavePrinted)
        {
            PrintStep(dialogue, false);
            stepIndex++;
        }
    }

    public void AutoPlay(Dialogue dialogue)
    {
        if (stepIndex < dialogue.dialogueSteps.Count && !leavePrinted)
        {
            if (isAutoPlaying)
            {
                switch (autoPlaySpeed)
                {
                    case 2f:
                        autoPlaySpeed = 1f;
                        break;
                    case 1f:
                        autoPlaySpeed = 0.5f;
                        break;
                    case 0.5f:
                        autoPlaySpeed = 2f;
                        break;
                }
            }
            else
            {
                foreach(GameObject obj in continueButtons)
                {
                    Destroy(obj);
                }
                continueButtons = new();

                isAutoPlaying = true;
                StartCoroutine(AutoPlayTimer(dialogue));
            }
        }

    }

    IEnumerator AutoPlayTimer(Dialogue dialogue)
    {
        while (isAutoPlaying && stepIndex < dialogue.dialogueSteps.Count)
        {
            PrintStep(dialogue, false);
            stepIndex++;
            AutoPlayTimer(dialogue);

            yield return new WaitForSeconds(autoPlaySpeed);
        }

        if (stepIndex >= dialogue.dialogueSteps.Count)
        {
            isAutoPlaying = false;
            autoPlaySpeed = 2;
        }
    }
}
