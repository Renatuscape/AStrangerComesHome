using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueButtonFactory
{
    public DialogueMenu dialogueMenu;

    //BASIC BUTTON - CREATE ALL OBJECTS FROM THIS
    public GameObject InstantiateBasicButton()
    {
        GameObject button = UnityEngine.Object.Instantiate(dialogueMenu.dialogueSystem.button);

        //Choose width by parent container width
        float newSize = dialogueMenu.dialogueContainer.GetComponent<RectTransform>().sizeDelta.x - 20;
        RectTransform transform = button.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(newSize, transform.sizeDelta.y);
        button.transform.SetParent(dialogueMenu.dialogueContainer.transform, false);
        button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().font = dialogueMenu.dialogueSystem.fontManager.body.font;

        dialogueMenu.buttonList.Add(button);

        button.name = "BasicButton";
        return button;
    }

    //BASIC LEFT-ALIGNED TEXT
    public GameObject PrintText(string content)
    {
        GameObject button = InstantiateBasicButton();
        UnityEngine.Object.Destroy(button.GetComponent<Button>());

        var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMesh.alignment = TextAlignmentOptions.Left;
        textMesh.text = content;

        //Hide background
        button.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        button.name = "content";
        return button;
    }

    public GameObject PrintNamePlate(Character character)
    {
        character.NameSetup();
        GameObject button = PrintText(character.namePlate);
        var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMesh.fontSize += 2;
        textMesh.fontSizeMax += 2;
        textMesh.font = dialogueMenu.dialogueSystem.fontManager.subtitle.font;

        VerticalLayoutGroup layout = button.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(
        layout.padding.left,
        layout.padding.right,
        layout.padding.top,
        -5);

        button.name = "NamePlate";
        return button;
    }

    public GameObject PrintNarration(string content)
    {
        GameObject button = PrintText(content);
        var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0.7f);

        dialogueMenu.printedChoices.Add(button);
        return button;
    }


    //CONTINUE STEP BUTTON
    public GameObject PrintContinue(Dialogue dialogue)
    {
        GameObject button = InstantiateBasicButton();
        button.name = "ContinueButton";
        button.GetComponent<Button>().onClick.AddListener(() => dialogueMenu.ContinueToNextStep(dialogue, button));

        //SET BUTTON SIZE
        float newSize = 75;
        RectTransform transform = button.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(newSize, transform.sizeDelta.y);

        var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        //textMesh.alignment = TextAlignmentOptions.Left;
        textMesh.text = "Continue";

        return button;
    }
    //LEAVE BUTTON
    public GameObject PrintLeaveButton()
    {
        dialogueMenu.leavePrinted = true;
        GameObject button = InstantiateBasicButton();
        button.GetComponent<Button>().interactable = false;
        var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMesh.text = "Leave";

        //SET BUTTON SIZE
        float newSize = 65;
        RectTransform transform = button.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(newSize, transform.sizeDelta.y);

        button.GetComponent<Button>().onClick.AddListener(() => dialogueMenu.dialogueSystem.CloseDialogueMenu());
        dialogueMenu.StartCoroutine(DelayedEnable(button));

        return button;

        IEnumerator DelayedEnable(GameObject button, float timer = 0.5f)
        {
            yield return new WaitForSeconds(timer);

            button.GetComponent<Button>().interactable = true;
        }
    }
}
