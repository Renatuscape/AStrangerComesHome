using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopicMenu : MonoBehaviour
{
    public List<Quest> questList;
    public DialogueSystem dialogueSystem;
    public GameObject topicContainer;
    public List<GameObject> buttonList;
    public void OpenTopicsMenu(string speakerID)
    {
        TransientDataScript.SetGameState(GameState.Dialogue, "TopicMenu", gameObject);

        questList = new();

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
                GameObject button = Instantiate(dialogueSystem.button);

                //Choose width by parent container width
                float newSize = topicContainer.GetComponent<RectTransform>().sizeDelta.x - 20;
                RectTransform transform = button.GetComponent<RectTransform>();
                transform.sizeDelta = new Vector2(newSize, transform.sizeDelta.y);

                button.transform.SetParent(topicContainer.transform, false);
                var text = button.gameObject.transform.GetChild(0);
                text.GetComponent<TextMeshProUGUI>().text = GetTopicName(quest);
                buttonList.Add(button);
            }
            topicContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();
            topicContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
            topicContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();
        }
    }

    public string GetTopicName(Quest quest)
    {
        if (Player.GetEntry(quest.objectID, "TopicMenu", out var entry))
        {
            return quest.dialogues[entry.amount].topicName;
        }
        else
        {
            return quest.name;
        }
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
