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

    private void Start()
    {
        topicMenu.SetActive(false);
        dialogueMenu.SetActive(false);
        button.GetComponentInChildren<TextMeshProUGUI>().font = fontManager.body.font;
    }
    public void OpenTopicMenu(string speakerID)
    {
        activeSpeaker = speakerID;
        button.GetComponentInChildren<TextMeshProUGUI>().font = fontManager.body.font;
        previousGameState = TransientDataScript.GetGameState();
        topicMenu.SetActive(true);
        topicMenu.GetComponent<TopicMenu>().OpenTopicsMenu(speakerID);
        dialogueMenu.SetActive(false);
    }

    public void StartDialogue(Quest quest) //called primarily from topic manager
    {
        Debug.Log($"Attempting to start dialogue {quest.objectID}");

        button.GetComponentInChildren<TextMeshProUGUI>().font = fontManager.body.font;

        topicMenu.SetActive(false);
        dialogueMenu.SetActive(true);
        dialogueMenu.GetComponent<DialogueMenu>().StartDialogue(quest);
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