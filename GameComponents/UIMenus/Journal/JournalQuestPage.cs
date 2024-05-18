using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class JournalQuestPage : MonoBehaviour
{
    public FontManager fontManager;
    public GameObject questPrefab; // Assign your prefab in the inspector
    public GameObject questContainer; // Assign your container in the inspector
    public GameObject detailContainer;
    public float delayBetweenQuests = 0.1f; // Adjust the delay duration as needed
    public List<GameObject> questPrefabs;
    public TextMeshProUGUI displayTitle;
    public TextMeshProUGUI displayTopicName;
    public TextMeshProUGUI displayDescription;
    public TextMeshProUGUI pageTitle;

    private void Awake()
    {
        displayTitle.text = "";
        displayTopicName.text = "";
        displayDescription.text = "";
    }
    private void OnEnable()
    {
        //displayTitle.font = fontManager.header.font;
        //displayTopicName.font = fontManager.subtitle.font;
        //displayDescription.font = fontManager.script.font;
        //pageTitle.font = fontManager.header.font;
        StartCoroutine(InstantiateQuests());
    }

    IEnumerator InstantiateQuests()
    {
        foreach (var quest in Quests.all)
        {
            if (!quest.excludeFromJournal && Player.GetEntry(quest.objectID, name, out var entry))
            {
                questContainer.GetComponent<VerticalLayoutGroup>().enabled = false;

                yield return new WaitForSeconds(delayBetweenQuests); // Add a delay

                GameObject newQuest = Instantiate(questPrefab, questContainer.transform);
                newQuest.GetComponent<QuestPrefab>().quest = quest;
                newQuest.GetComponent<QuestPrefab>().journalQuestPage = this;
                var textMesh = newQuest.transform.Find("QuestTitle").GetComponent<TextMeshProUGUI>();
                textMesh.text = quest.name;
                textMesh.font = fontManager.subtitle.font;
                questPrefabs.Add(newQuest);

                questContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
                Canvas.ForceUpdateCanvases();
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
        detailContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
        displayTitle.text = quest.name;

        int questStage = quest.GetQuestStage();

        var dialogue = quest.dialogues.FirstOrDefault(d => d.questStage == questStage);
        string topicName = "";
        string description = "";

        if (dialogue != null)
        {
            topicName = dialogue.topicName ?? "";

            if (string.IsNullOrEmpty(dialogue.hint))
            {
                if (quest.dynamicDescriptions != null && quest.dynamicDescriptions.Count > 0)
                {
                    for (int i = quest.dynamicDescriptions.Count - 1; i >= 0; i--)
                    {
                        var dDescription = quest.dynamicDescriptions[i];

                        if (RequirementChecker.CheckPackage(dDescription.checks))
                        {
                            description = dDescription.content;
                            break;
                        }
                    }
                }
                
                if (description == "")
                {
                    description = DialogueTagParser.ParseText(quest.description);
                }
            }
            else
            {
                description = DialogueTagParser.ParseText(dialogue.hint);
            }
        }

        if (topicName == "" || topicName == quest.name)
        {
            displayTopicName.gameObject.SetActive(false);
        }
        else
        {
            displayTopicName.gameObject.SetActive(true);
            displayTopicName.text = topicName;
        }

        displayDescription.text = description;
        detailContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
        detailContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
        detailContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }
}
