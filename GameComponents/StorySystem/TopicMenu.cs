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
            string speaker = "";
            int stage = Player.GetCount(quest.objectID, "topicMenu");

            if (stage < quest.dialogues.Count)
            {
                Dialogue activeDialogue = quest.dialogues.FirstOrDefault(d => d.questStage == stage);

                if (activeDialogue.stageType == StageType.Dialogue) // Make sure this stage is of type dialogue
                {
                    speaker = activeDialogue.speakerID;
                    //Debug.Log($"Dialogue type was dialogue, and speakerID in dialogue was \"{speaker}\".");
                }

                if (string.IsNullOrEmpty(speaker)) // If there is no speaker assigned to this dialogue, default to quest giver ID.
                {
                    speaker = quest.questGiver.objectID;
                    //Debug.Log($"Speaker was null or empty in dialogue. Speaker is set to {quest.questGiver.objectID} ({speaker})");
                }

                if (speaker == speakerID)
                {
                    //Debug.Log($"SPEAKER MATCH FOUND. Checking requirements for dialogue {activeDialogue.objectID}");

                    bool passedChecks = true;

                    if (stage == 0)
                    {
                        passedChecks = quest.unlockRequirements.CheckRequirements(out var minDaysPassed);
                    }

                    if (passedChecks)
                    {
                        passedChecks = activeDialogue.CheckRequirements();

                        if (passedChecks)
                        {
                            //Debug.Log($"Check for quest and dialogue {activeDialogue.objectID} passed.");
                            foundQuests.Add(quest);
                        }
                    }
                }
            }
        }

        //Debug.Log($"Found {foundQuests.Count} topics.");
        return foundQuests;
    }

    public void CreateTopicButtons()
    {
        // Debug.Log("Quest list contained " + questList.Count);

        if (questList.Count > 1 || (!allowAutoPlay && questList.Count > 0))
        {
            topicContainer.GetComponent<VerticalLayoutGroup>().enabled = false;

            foreach (Quest quest in questList)
            {
                PrintTopicButton(quest);
            }

            topicContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();
            topicContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
            topicContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
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
                text.GetComponent<TextMeshProUGUI>().text = topicName;
                buttonList.Add(button);

                button.GetComponent<Button>().onClick.AddListener(() => StartDialogue(quest, false));
            }
        }
    }

    public Dialogue GetRelevantDialogue(Quest quest, out string topicName)
    {
        if (quest.dialogues != null && quest.dialogues.Count > 0)
        {
            if (Player.GetEntry(quest.objectID, "TopicMenu", out IdIntPair entry))
            {
                if (entry.amount < quest.dialogues.Count) //CHECK IF NEW DIALOGUES EXIST
                {
                    topicName = quest.dialogues[entry.amount].topicName;

                    if (string.IsNullOrEmpty(topicName))
                    {
                        topicName = quest.name; //EXCHANGE WITH LOGIC TO FIND PREVIOUS STAGE NAME
                    }

                    return quest.dialogues[entry.amount];
                }
                topicName = "";
                return null;
            }
            else //if quest is not already active and found in player journal
            {
                topicName = quest.name;
                return quest.dialogues[0];
            }
        }
        else
        {
            topicName = "";
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
