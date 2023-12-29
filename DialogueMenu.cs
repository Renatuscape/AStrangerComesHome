using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
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

    int stepIndex = 0;

    private void Awake()
    {
        dialogueContainerBG = dialogueContainer.GetComponent<Image>();
    }

    private void OnEnable()
    {
        stepIndex = 0;
        foreach (GameObject step in buttonList)
        {
            Destroy(step.gameObject);
        }
        SetValuesToDefault();
    }

    private void OnDisable()
    {
        foreach (GameObject step in buttonList)
        {
            Destroy(step.gameObject);
        }
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
    void PrintStep(Dialogue dialogue)
    {
        if (dialogue is not null)
        {
            DialogueStep step = dialogue.dialogueSteps[stepIndex];

            if (step.speaker.dialogueTag != "Narration")
            {
                PrintNamePlate(step.speaker);
            }

            PrintText(step.text);

            if (stepIndex <= dialogue.dialogueSteps.Count)
            {
                Debug.Log("Attempting to print continue button");
                PrintContinue(dialogue);
            }
        }
        else
        {
            Debug.Log("dialogue passed to PrintStep was null");
        }
    }

    //BASIC BUTTON - CREATE ALL OBJECTS FROM THIS
    GameObject InstantiateBasicButton()
    {
        GameObject button = Instantiate(dialogueSystem.button);

        //Choose width by parent container width
        float newSize = dialogueContainer.GetComponent<RectTransform>().sizeDelta.x - 20;
        RectTransform transform = button.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(newSize, transform.sizeDelta.y);

        buttonList.Add(button);

        button.name = "BasicButton";
        return button;
    }

    //BASIC LEFT-ALIGNED TEXT
    GameObject PrintText(string content)
    {
        GameObject button = InstantiateBasicButton();
        Destroy(button.GetComponent<Button>());

        var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMesh.alignment = TextAlignmentOptions.Left;
        textMesh.text = content;

        //Hide background
        button.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        button.transform.SetParent(dialogueContainer.transform, false);

        button.name = "Content";
        return button;
    }

    GameObject PrintNamePlate(Character character)
    {
        GameObject button = PrintText(character.namePlate);
        var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMesh.fontSize += 2;
        textMesh.fontSizeMax += 2;

        VerticalLayoutGroup layout = button.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(
        layout.padding.left,
        layout.padding.right,
        layout.padding.top,
        -5);

        button.name = "NamePlate";
        return button;
    }

    GameObject PrintNarration(string content)
    {
        return null;
    }


    //CONTINUE STEP BUTTON
    void PrintContinue(Dialogue dialogue)
    {
        Debug.Log("Print continue was called");
        GameObject button = InstantiateBasicButton();
        button.name = "ContinueButton";
        button.GetComponent<Button>().onClick.AddListener(() => ContinueToNextStep(dialogue));

        var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMesh.alignment = TextAlignmentOptions.Left;
        textMesh.text = "Continue";
    }

    public void ContinueToNextStep(Dialogue dialogue)
    {
        stepIndex++;
        PrintStep(dialogue);
        Destroy(gameObject);
    }

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
