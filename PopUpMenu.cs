using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpMenu : MonoBehaviour
{
    public DialogueSystem dialogueSystem;
    public TextMeshProUGUI textMesh;
    public GameObject container;
    public List<GameObject> buttons;
    public int stepIndex;

    public void StartPopUp(Dialogue dialogue)
    {
        DialogueTagParser.UpdateTags(GameObject.Find("DataManager").GetComponent<DataManagerScript>());
        gameObject.SetActive(true);
        stepIndex = 0;
        UpdateText(dialogue);
    }

    public void UpdateText(Dialogue dialogue)
    {
        for (int i = 0; i < dialogue.content.Count;i++)
        {
            dialogue.content[i] = DialogueTagParser.ParseText(dialogue.content[i]);
        }

        if (stepIndex < dialogue.content.Count)
        {
            textMesh.text = dialogue.content[stepIndex];
            GameObject button = PrintContinue(dialogue);

            //Handle choice/leave options
            if (stepIndex + 1 == dialogue.content.Count)
            {
                var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                textMesh.text = "Leave";
            }
        }
        else if (stepIndex >= dialogue.content.Count)
        {
            stepIndex = 0;
            dialogueSystem.ColosePopUpMenu();
        }
    }

    GameObject CreateButton()
    {
        GameObject button = Instantiate(dialogueSystem.button);

        //Choose width by parent container width
        float newSize = container.GetComponent<RectTransform>().sizeDelta.x - 20;
        RectTransform transform = button.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(newSize, transform.sizeDelta.y);
        button.transform.SetParent(container.transform, false);

        buttons.Add(button);

        button.name = "BasicButton";
        return button;
    }

    GameObject PrintContinue(Dialogue dialogue)
    {
        GameObject button = CreateButton();
        button.name = "ContinueButton";
        button.GetComponent<Button>().onClick.AddListener(() => ContinueToNextStep(dialogue, button));

        //SET BUTTON SIZE
        float newSize = 75;
        RectTransform transform = button.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(newSize, transform.sizeDelta.y);

        var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMesh.text = "Continue";

        return button;
    }

    public void ContinueToNextStep(Dialogue dialogue, GameObject button)
    {
        stepIndex++;
        UpdateText(dialogue);
        Destroy(button);
    }
}
