using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public GameObject button;
    public GameObject topicMenu;
    public GameObject dialogueMenu;
    public GameObject popUpMenu;
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

    public void StartDialogue(Quest quest) //called primarily from topic manager
    {
        topicMenu.SetActive(false);
        dialogueMenu.SetActive(true);
        dialogueMenu.GetComponent<DialogueMenu>().StartDialogueStage(quest);
    }

    public void CloseTopicMenuAndLeave()
    {
        topicMenu.SetActive(false);
        TransientDataScript.SetGameState(previousGameState, "DialogueSystem", gameObject);
    }

    public void CloseDialogueMenu()
    {
        dialogueMenu.SetActive(false);
        TransientDataScript.SetGameState(previousGameState, "DialogueSystem", gameObject);
    }

    public void ColosePopUpMenu()
    {
        popUpMenu.gameObject.SetActive(false);
        StartCoroutine(GameStateDelay());

        IEnumerator GameStateDelay()
        {
            yield return new WaitForSeconds(1);
            TransientDataScript.SetGameState(GameState.Overworld, "DialogueSystem", gameObject);
        }
    }
}