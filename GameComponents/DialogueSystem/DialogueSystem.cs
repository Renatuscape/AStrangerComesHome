using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public GameObject button;
    public GameObject TopicMenu;
    public GameObject DialogueEvent;

    public void OpenTopicMenu()
    {
        TopicMenu.SetActive(true);
        TopicMenu.GetComponent<TopicMenu>().OpenTopicsMenu("DEBUG");
        DialogueEvent.SetActive(false);
    }

    public void StartDialogueEvent()
    {
        TopicMenu.SetActive(false);
        DialogueEvent.SetActive(true);
    }
}
[System.Serializable]
public class Topic
{
    public DialogueSystem dialogueSystem;
    public Quest quest;
}