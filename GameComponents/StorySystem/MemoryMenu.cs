using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryMenu : MonoBehaviour
{
    public StorySystem dialogueSystem;
    public GameObject container;
    public GameObject endButton;
    public Dialogue activeMemory;

    public TextMeshProUGUI memoryTitle;
    public Image shadowBox;
    public Image illustration;
    public Image illustrationFade;
    public List<Sprite> illustrations;

    public List<GameObject> dialogueList;
    public GameObject continueButton;

    public int index;
    public float columnWidth;

    Color shadowColour;

    void Awake()
    {
        shadowColour = new Color(0.1527f, 0.1073781f, 0.1792453f, 0.5f);
    }

    private void OnDisable()
    {
        shadowBox.color = new Color(shadowColour.r, shadowColour.g, shadowColour.b, 0);
        illustrationFade.color = new Color(illustrationFade.color.r, illustrationFade.color.g, illustrationFade.color.b, 1);

        foreach (var prefab in dialogueList)
        {
            Destroy(prefab);
        }
    }

    public bool InitialiseReader(string questID, int questStage, bool isReadingFromArchive = false)
    {
        illustration.gameObject.SetActive(false);
        endButton.gameObject.SetActive(false);

        shadowBox.color = new Color(shadowColour.r, shadowColour.g, shadowColour.b, 0);
        var quest = Quests.FindByID(questID);
        activeMemory = quest.GetStage(questStage);
        var playerProgress = Player.GetCount(questID, name);

        if (activeMemory == null || activeMemory.stageType != StageType.Memory)
        {
            Debug.Log($"{questID} stage {questStage} returned [{activeMemory.objectID}] of type {activeMemory.stageType}. Was null or not of type memory.");
            return false;
        }
        else if (!isReadingFromArchive && playerProgress != questStage)
        {
            Debug.Log($"Memory found, but quest stage was wrong. Player is not at the current quest stage.");
            return false;
        }
        else if (isReadingFromArchive && playerProgress < questStage)
        {
            Debug.Log($"Memory found, but quest stage was too low. Memory not yet unlocked.");
            return false;
        }
        else
        {
            Debug.Log($"Initialising memory reader successfully.");

            illustration.sprite = illustrations.Where(illustrations => activeMemory.objectID.Contains(illustrations.name)).FirstOrDefault();
            if (illustration.sprite == null)
            {
                Debug.Log($"No illustration found in illustrations list with the name {activeMemory.objectID}.");
            }

            memoryTitle.text = activeMemory.topicName ?? "A Stray Memory";
            TransientDataScript.SetGameState(GameState.Dialogue, name, gameObject);
            gameObject.SetActive(true);
            StartCoroutine(FadeInShadowBox());
            StartDialogue();
            return true;
        }
    }

    void StartDialogue()
    {
        index = 0;
        columnWidth = container.GetComponent<RectTransform>().sizeDelta.x - 20;
        Step();
    }

    void Step()
    {
        if (continueButton != null)
        {
            Destroy(continueButton);
            continueButton = null;
        }

        container.GetComponent<VerticalLayoutGroup>().enabled = false;
        var textBox = BoxFactory.CreateTextBox(activeMemory.content[index], columnWidth);
        textBox.transform.SetParent(container.transform, false);
        dialogueList.Add(textBox);
        container.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();

        index++;

        if (index == activeMemory.content.Count)
        {
            End();
        }
        else
        {
            container.GetComponent<VerticalLayoutGroup>().enabled = false;
            continueButton = BoxFactory.CreateButton("Continue");
            continueButton.GetComponent<Button>().onClick.AddListener(() => ClickContinueButton(continueButton));
            continueButton.transform.SetParent(container.transform, false);
            container.GetComponent<VerticalLayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();
        }
    }

    void End()
    {
        foreach (var prefab in dialogueList)
        {
            Destroy(prefab);
        }

        dialogueList.Clear();

        illustration.gameObject.SetActive(true);
        StartCoroutine(FadeRevealIllustration());
        endButton.gameObject.SetActive(true);
    }

    IEnumerator FadeInShadowBox()
    {
        float shadowTranslucence = 0;
        while (shadowTranslucence < 0.5f)
        {
            shadowTranslucence += 0.01f;
            shadowBox.color = new Color(shadowColour.r, shadowColour.g, shadowColour.b, shadowTranslucence);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator FadeRevealIllustration()
    {
        float translucense = 1;

        while (translucense > 0)
        {
            translucense -= 0.01f;
            illustrationFade.color = new Color(illustrationFade.color.r, illustrationFade.color.g, illustrationFade.color.b, translucense);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void ClickContinueButton(GameObject button)
    {
        Destroy(button);
        continueButton = null;
        Step();
    }

    public void ClickEndButton()
    {
        gameObject.SetActive(false);
        Player.Set(activeMemory.questID, activeMemory.choices[0].advanceTo);
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }
}
