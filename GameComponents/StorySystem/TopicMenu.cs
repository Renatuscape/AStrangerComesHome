using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopicMenu : MonoBehaviour
{
    public List<Quest> questList;
    public StorySystem dialogueSystem;
    public PortraitRenderer portraitRenderer;
    public GameObject topicContainer;
    public List<GameObject> buttonList;
    public bool allowAutoPlay;

    public void OpenTopicsMenu(string speakerID, bool noPortrait = false, bool allowAutoPlay = true)
    {
        TransientDataScript.SetGameState(GameState.Dialogue, "TopicMenu", gameObject);
        this.allowAutoPlay = allowAutoPlay;

        questList = new();

        if (!noPortrait)
        {
            portraitRenderer.gameObject.SetActive(true);
            portraitRenderer.EnableForTopicMenu(speakerID);
        }
        questList = FilterBySpeaker(speakerID);

        CreateTopicButtons();
    }

    public void ReopenTopicsAfterDialogue(string speakerID, bool noPortrait = false)
    {
        OpenTopicsMenu(speakerID, noPortrait, false);
    }

    public List<Quest> FilterBySpeaker(string speakerID)
    {
        // Debug.Log("Sorting topics by speaker.");

        List<Quest> foundQuests = new();

        foreach (Quest quest in Quests.all)
        {
            int stage = Player.GetCount(quest.objectID, "topicMenu");
            Dialogue activeDialogue = quest.dialogues.FirstOrDefault(d => d.questStage == stage);

            if (activeDialogue != null && activeDialogue.stageType == StageType.Dialogue)
            {

                if (activeDialogue.speakerID == speakerID)
                {
                    Debug.Log($"TOPIC MENU: Speaker matched. Checking requirements for dialogue {activeDialogue.objectID}");

                    if (activeDialogue.CheckRequirements())
                    {
                        Debug.Log($"TOPIC MENU: Check for quest and dialogue {activeDialogue.objectID} passed.");
                        foundQuests.Add(quest);
                    }
                }
            }
        }

        Debug.Log($"Found {foundQuests.Count} topics.");
        return foundQuests;
    }

    public void CreateTopicButtons()
    {
        Debug.Log("TOPIC MENU: Quest list contained " + questList.Count);

        if (questList.Count > 1 || (!allowAutoPlay && questList.Count > 0))
        {
            foreach (Quest quest in questList)
            {
                topicContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
                topicContainer.GetComponent<ContentSizeFitter>().enabled = false;
                Canvas.ForceUpdateCanvases();

                PrintTopicButton(quest);

                topicContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
                topicContainer.GetComponent<ContentSizeFitter>().enabled = true;
                Canvas.ForceUpdateCanvases();
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(topicContainer.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        else if (questList.Count == 1)
        {
            Debug.Log("Only one relevant quest was found.");
            var quest = questList[0];
            Dialogue dialogue = GetRelevantDialogue(quest, out var topicName);

            if (dialogue != null && dialogue.stageType == StageType.Dialogue)
            {
                Debug.Log("Attempting to start dialogue immediately.");
                StartDialogue(questList[0], true);
            }
            else
            {
                Debug.Log("Attempting to start dialogue immediately.");
                var message = BoxFactory.CreateTextBox("Nothing to talk about right now.", topicContainer.GetComponent<RectTransform>().sizeDelta.x - 20);
                message.transform.SetParent(topicContainer.transform, false);
                buttonList.Add(message);
            }
        }
        else
        {
            var message = BoxFactory.CreateTextBox("Nothing to talk about right now.", topicContainer.GetComponent<RectTransform>().sizeDelta.x - 20);
            message.transform.SetParent(topicContainer.transform, false);
            buttonList.Add(message);
        }
    }

    void PrintTopicButton(Quest quest)
    {
        Dialogue dialogue = GetRelevantDialogue(quest, out var topicName);
        {
            if (dialogue != null && dialogue.stageType == StageType.Dialogue)
            {
                GameObject button = Instantiate(dialogueSystem.button);

                //Choose width by parent container width
                float newSize = topicContainer.GetComponent<RectTransform>().sizeDelta.x - 20;
                RectTransform transform = button.GetComponent<RectTransform>();
                transform.sizeDelta = new Vector2(newSize, transform.sizeDelta.y);

                button.transform.SetParent(topicContainer.transform, false);
                var text = button.gameObject.transform.GetChild(0);

                // Debug.Log("TPMenu: Attempting to print topic with name " + topicName);
                var parsedText = DialogueTagParser.ParseText(topicName);
                // Debug.Log("TPMenu: Parsed text result: " + parsedText);
                text.GetComponent<TextMeshProUGUI>().text = parsedText;
                // Debug.Log("TPMenu: Button text should read: " + text.GetComponent<TextMeshProUGUI>().text);
                buttonList.Add(button);

                button.GetComponent<Button>().onClick.AddListener(() => StartDialogue(quest, false));
            }
        }
    }

    public Dialogue GetRelevantDialogue(Quest quest, out string topicName)
    {
        if (quest.dialogues != null && quest.dialogues.Count > 0)
        {
            int stage = Player.GetCount(quest.objectID, "topicMenu");
            Dialogue activeDialogue = quest.dialogues.FirstOrDefault(d => d.questStage == stage);

            if (activeDialogue != null && activeDialogue.stageType == StageType.Dialogue)
            {
                if (!string.IsNullOrEmpty(activeDialogue.topicName))
                {
                    topicName = activeDialogue.topicName;
                }
                else
                {
                    topicName = quest.name;
                }

                return activeDialogue;
            }
            else
            {
                topicName = "No Topic";
                return null;
            }
        }
        else
        {
            topicName = "No Topic";
            return null;
        }
    }

    public void StartDialogue(Quest quest, bool doNotReopenTopic)
    {
        portraitRenderer.gameObject.SetActive(false);
        dialogueSystem.StartDialogue(quest, doNotReopenTopic);
    }

    private void OnDisable()
    {
        CloseTopics();
    }

    public void CloseTopics()
    {
        foreach (GameObject topic in buttonList)
        {
            Destroy(topic.gameObject);
        }
        buttonList.Clear();
        questList = null;
    }
}
