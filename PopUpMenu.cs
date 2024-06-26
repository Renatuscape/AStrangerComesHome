using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpMenu : MonoBehaviour
{
    public StorySystem dialogueSystem;
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
        container.GetComponent<VerticalLayoutGroup>().enabled = false;
        container.GetComponent<ContentSizeFitter>().enabled = false;

        textMesh.font = dialogueSystem.fontManager.body.font;

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

            if (dialogue.choices.Count > 0)
            {
                dialogue.choices[0].GrantRewards();
            }
            else
            {
                Player.IncreaseQuest(dialogue.questID);
            }
            dialogueSystem.ColosePopUpMenu();
        }

        container.GetComponent<VerticalLayoutGroup>().enabled = true;
        container.GetComponent<ContentSizeFitter>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    GameObject CreateButton()
    {
        GameObject button = Instantiate(dialogueSystem.button);

        button.transform.SetParent(container.transform, false);

        buttons.Add(button);

        button.name = "BasicButton";
        return button;
    }

    GameObject PrintContinue(Dialogue dialogue)
    {
        container.GetComponent<VerticalLayoutGroup>().enabled = false;
        container.GetComponent<ContentSizeFitter>().enabled = false;
        GameObject button = CreateButton();
        button.name = "ContinueButton";
        button.GetComponent<Button>().onClick.AddListener(() => ContinueToNextStep(dialogue, button));

        //SET BUTTON SIZE
        float newSize = 165;
        RectTransform transform = button.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(newSize, transform.sizeDelta.y);

        var textMesh = button.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMesh.text = "Continue";
        textMesh.fontSizeMax = 24;

        container.GetComponent<VerticalLayoutGroup>().enabled = true;
        container.GetComponent<ContentSizeFitter>().enabled = true;
        Canvas.ForceUpdateCanvases();

        return button;
    }

    public void ContinueToNextStep(Dialogue dialogue, GameObject button)
    {

        stepIndex++;
        UpdateText(dialogue);
        Destroy(button);
    }
}
