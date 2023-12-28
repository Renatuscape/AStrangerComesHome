using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public GameObject button;
    public GameObject topicMenu;
    public GameObject dialogueMenu;
    public GameState previousGameState = GameState.Overworld;

    private void Start()
    {
        topicMenu.SetActive(false);
        dialogueMenu.SetActive(false);
    }
    public void OpenTopicMenu(string speakerID)
    {
        previousGameState = TransientDataScript.GetGameState();
        topicMenu.SetActive(true);
        topicMenu.GetComponent<TopicMenu>().OpenTopicsMenu(speakerID);
        dialogueMenu.SetActive(false);
    }

    public void StartDialogueEvent()
    {
        topicMenu.SetActive(false);
        dialogueMenu.SetActive(true);
    }

    public void CloseTopicMenuAndLeave()
    {
        topicMenu.SetActive(false);
        TransientDataScript.SetGameState(previousGameState, "DialogueSystem", gameObject);
    }
}