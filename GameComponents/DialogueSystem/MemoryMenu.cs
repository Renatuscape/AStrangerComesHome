using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryMenu : MonoBehaviour
{
    public DialogueSystem dialogueSystem;
    public GameObject dialogueContainer;
    public GameObject endButton;
    public Dialogue activeMemory;

    public TextMeshProUGUI memoryTitle;
    public Color shadowColour;
    public Image shadowBox;
    public Image illustration;
    public List<Sprite> illustrations;

    public List<GameObject> dialogueList;

    void Start()
    {
        shadowColour = shadowBox.color;
    }

    private void OnDisable()
    {
        shadowBox.color = new Color(shadowColour.r, shadowColour.g, shadowColour.b, 0);

        foreach (var prefab in  dialogueList)
        {
            Destroy(prefab);
        }
    }

    public bool InitialiseReader(string questID, int questStage, bool isReadingFromArchive = false)
    {
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
            memoryTitle.text = activeMemory.topicName ?? "A Stray Memory";
            TransientDataCalls.SetGameState(GameState.Dialogue, name, gameObject);
            gameObject.SetActive(true);
            StartCoroutine(FadeInShadowBox());
            return true;
        }
    }

    public void ClickEndButton()
    {
        gameObject.SetActive(false);
        TransientDataCalls.SetGameState(GameState.Overworld, name, gameObject);
    }

    IEnumerator FadeInShadowBox()
    {
        float shadowTranslucence = 0;
        while (shadowBox.color != shadowColour)
        {
            shadowTranslucence += 0.1f;
            shadowBox.color = new Color(shadowColour.r, shadowColour.g, shadowColour.b, shadowTranslucence);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
