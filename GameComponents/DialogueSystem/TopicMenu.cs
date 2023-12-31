using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopicMenu : MonoBehaviour
{
    public List<Quest> questList;
    public DialogueSystem dialogueSystem;
    public PortraitRenderer portraitRenderer;
    public GameObject topicContainer;
    public List<GameObject> buttonList;
    public void OpenTopicsMenu(string speakerID)
    {
        TransientDataScript.SetGameState(GameState.Dialogue, "TopicMenu", gameObject);

        questList = new();
        portraitRenderer.gameObject.SetActive(true);
        portraitRenderer.EnableForTopicMenu(speakerID);

        if (speakerID != "DEBUG")
        {
            foreach (Quest quest in Quests.all)
            {
                if (quest.questGiver.objectID == speakerID)
                {
                    questList.Add(quest);
                }
            }
        }
        else
        {
            questList = new(Quests.all);
        }
        CreateTopicButtons();
    }
    public void CreateTopicButtons()
    {
        if (questList.Count > 0)
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
    }

    void PrintTopicButton(Quest quest)
    {
        Dialogue dialogue = GetRelevantDialogue(quest, out var topicName);
        {
            if (dialogue is not null && dialogue.stageType == StageType.Dialogue)
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

                button.GetComponent<Button>().onClick.AddListener(() => StartDialogue(quest));
            }
        }
    }

    public Dialogue GetRelevantDialogue(Quest quest, out string topicName)
    {
        if (Player.GetEntry(quest.objectID, "TopicMenu", out IdIntPair entry))
        {
            if (entry.amount < quest.dialogues.Count) //CHECK IF QUEST IS COMPLETED
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

    public void StartDialogue(Quest quest)
    {
        dialogueSystem.StartDialogue(quest);
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
