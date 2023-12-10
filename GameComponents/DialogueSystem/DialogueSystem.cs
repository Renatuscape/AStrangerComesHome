using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public GameObject button;
    public GameObject TopicMenu;
    public GameObject DialogueEvent;
    public GameState previousGameState = GameState.Overworld;

    public void OpenTopicMenu(string speakerID)
    {
        previousGameState = TransientDataScript.GetGameState();
        TopicMenu.SetActive(true);
        TopicMenu.GetComponent<TopicMenu>().OpenTopicsMenu(speakerID);
        DialogueEvent.SetActive(false);
    }

    public void StartDialogueEvent()
    {
        TopicMenu.SetActive(false);
        DialogueEvent.SetActive(true);
    }

    public void CloseTopicMenuAndLeave()
    {
        TopicMenu.SetActive(false);
        TransientDataScript.SetGameState(previousGameState, "DialogueSystem", gameObject);
    }
}