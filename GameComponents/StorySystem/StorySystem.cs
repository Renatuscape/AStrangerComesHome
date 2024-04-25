using System.Collections;
using TMPro;
using UnityEngine;

public class StorySystem : MonoBehaviour
{
    public DataManagerScript dataManager;
    public FontManager fontManager;
    public GameObject button;
    public GameObject topicMenu;
    public GameObject dialogueMenu;
    public GameObject popUpMenu;
    public GameState previousGameState = GameState.Overworld;
    public MemoryMenu memoryMenu;
    public string activeSpeaker;

    public DialoguePortraitManager portraitManager;
    private void Start()
    {
        if (TransientDataScript.GameState != GameState.Dialogue)
        {
            topicMenu.SetActive(false);
            dialogueMenu.SetActive(false);
        }
        button.GetComponentInChildren<TextMeshProUGUI>().font = fontManager.body.font;
        DialoguePortraitHelper.portraitManager = portraitManager;
    }
    public void OpenTopicMenu(string speakerID)
    {
        dialogueMenu.SetActive(false);
        activeSpeaker = speakerID;
        button.GetComponentInChildren<TextMeshProUGUI>().font = fontManager.body.font;
        previousGameState = TransientDataScript.GetGameState();
        topicMenu.SetActive(true);

        topicMenu.GetComponent<TopicMenu>().OpenTopicsMenu(speakerID);
    }

    public void ReopenTopicsAfterDialogue(string speakerID)
    {
        dialogueMenu.SetActive(false);
        activeSpeaker = speakerID;
        button.GetComponentInChildren<TextMeshProUGUI>().font = fontManager.body.font;
        previousGameState = TransientDataScript.GetGameState();
        topicMenu.SetActive(true);

        topicMenu.GetComponent<TopicMenu>().ReopenTopicsAfterDialogue(speakerID);
    }

    public void StartDialogue(Quest quest, bool doNotReopenTopic) //called primarily from topic manager
    {
        Debug.Log($"Attempting to start dialogue {quest.objectID}");

        button.GetComponentInChildren<TextMeshProUGUI>().font = fontManager.body.font;

        topicMenu.SetActive(false);
        dialogueMenu.SetActive(true);
        dialogueMenu.GetComponent<DialogueMenu>().StartDialogue(quest, activeSpeaker, doNotReopenTopic);
        Debug.Log($"Starting dialogue for quest {quest.objectID} with speaker {activeSpeaker}.");
        // speaker may not correspond to quest giver. This will help track which NPC started the dialogue so that topic menu can be reopened properly.

        if (!dialogueMenu.activeInHierarchy) {
            Debug.Log("Something disabled the dialogue menu.");
        }
    }

    public void CloseTopicMenuAndLeave()
    {
        topicMenu.SetActive(false);
        activeSpeaker = string.Empty;
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }

    public void CloseDialogueMenu()
    {
        dialogueMenu.SetActive(false);

        if (!string.IsNullOrEmpty(activeSpeaker))
        {
            OpenTopicMenu(activeSpeaker);
        }
        else
        {
            TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
        }
    }

    public void ColosePopUpMenu()
    {
        popUpMenu.gameObject.SetActive(false);
        StartCoroutine(GameStateDelay());

        IEnumerator GameStateDelay()
        {
            yield return new WaitForSeconds(1);
            TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
        }
    }
}