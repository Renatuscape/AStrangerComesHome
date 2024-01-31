using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class JournalQuestPage : MonoBehaviour
{
    public GameObject questPrefab; // Assign your prefab in the inspector
    public GameObject questContainer; // Assign your container in the inspector
    public float delayBetweenQuests = 0.1f; // Adjust the delay duration as needed
    public List<GameObject> questPrefabs;
    public TextMeshProUGUI displayTitle;
    public TextMeshProUGUI displayTopicName;
    public TextMeshProUGUI displayDescription;

    private void Awake()
    {
        displayTitle.text = "";
        displayTopicName.text = "";
        displayDescription.text = "";
    }
    private void OnEnable()
    {
        StartCoroutine(InstantiateQuests());
    }

    IEnumerator InstantiateQuests()
    {
        foreach (var quest in Quests.all)
        {
            if (!quest.excludeFromJournal && Player.GetEntry(quest.objectID, name, out var entry))
            {
                yield return new WaitForSeconds(delayBetweenQuests); // Add a delay

                GameObject newQuest = Instantiate(questPrefab, questContainer.transform);
                newQuest.GetComponent<QuestPrefab>().quest = quest;
                newQuest.GetComponent<QuestPrefab>().journalQuestPage = this;
                newQuest.transform.Find("QuestTitle").GetComponent<TextMeshProUGUI>().text = quest.name;
                questPrefabs.Add(newQuest);
            }
        }
    }

    private void OnDisable()
    {
        foreach (var quest in questPrefabs)
        {
            Destroy(quest);
        }
    }

    public void DisplayQuestDetails(Quest quest)
    {
        displayTitle.text = quest.name;
        string topicName = "";
        string description = "";
        
        int questStage = quest.GetQuestStage();

        if (questStage < quest.dialogues.Count)
        {
            topicName = quest.dialogues[questStage].topicName ?? "";
            description = quest.dialogues[questStage].hint ?? "";
        }

        if (topicName == "")
        {
            displayTopicName.gameObject.SetActive(false);
        }
        else
        {
            displayTopicName.gameObject.SetActive(true);
            displayTopicName.text = topicName;
        }

        if (description == "")
        {
            description = quest.description ?? "Missing topic and quest description";
        }

        displayDescription.text = description;
    }
}
