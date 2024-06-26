using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalQuestPage : MonoBehaviour
{
    public FontManager fontManager;
    public GameObject questPrefab;
    public GameObject questContainer;
    public GameObject detailContainer;
    public float delayBetweenQuests = 0.1f;
    public List<GameObject> questPrefabs;
    public TextMeshProUGUI displayTitle;
    public TextMeshProUGUI displayTopicName;
    public TextMeshProUGUI displayDescription;
    public TextMeshProUGUI pageTitle;
    public Button btnTaskToggle;
    public GameObject taskTrackerPanel;
    public TextMeshProUGUI taskTrackerText;

    private void Awake()
    {
        displayTitle.text = "";
        displayTopicName.text = "";
        displayDescription.text = "";
        btnTaskToggle.onClick.AddListener(() =>
        {
            taskTrackerPanel.SetActive(!taskTrackerPanel.activeInHierarchy);
        });
    }
    private void OnEnable()
    {
        //displayTitle.font = fontManager.header.font;
        //displayTopicName.font = fontManager.subtitle.font;
        //displayDescription.font = fontManager.script.font;
        //pageTitle.font = fontManager.header.font;

        btnTaskToggle.gameObject.SetActive(false);
        taskTrackerPanel.gameObject.SetActive(false);
        StartCoroutine(InstantiateQuests());
    }

    IEnumerator InstantiateQuests()
    {
        foreach (var quest in Quests.all)
        {
            if (!quest.excludeFromJournal && Player.GetEntry(quest.objectID, name, out var entry))
            {
                if (entry.amount < 100) // Exclude completed quests here for now. Make separate complete and active category later
                {
                    questContainer.GetComponent<VerticalLayoutGroup>().enabled = false;

                    yield return new WaitForSeconds(delayBetweenQuests); // Add a delay

                    GameObject newQuest = Instantiate(questPrefab, questContainer.transform);
                    newQuest.GetComponent<QuestPrefab>().quest = quest;
                    newQuest.GetComponent<QuestPrefab>().journalQuestPage = this;
                    var textMesh = newQuest.transform.Find("QuestTitle").GetComponent<TextMeshProUGUI>();
                    textMesh.text = DialogueTagParser.ParseText(quest.name);
                    textMesh.font = fontManager.subtitle.font;
                    questPrefabs.Add(newQuest);

                    questContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
                    Canvas.ForceUpdateCanvases();
                }
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
        displayTitle.text = DialogueTagParser.ParseText(quest.name);

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
            displayTopicName.text = DialogueTagParser.ParseText(topicName);
        }

        if (dialogue.taskTracking.Count > 0)
        {
            taskTrackerText.text = "";
            btnTaskToggle.gameObject.SetActive(true);

            foreach (var task in dialogue.taskTracking)
            {
                int inventoryCount = Player.GetCount(task.objectID, "JournalQuestPage");
                string taskDescription;

                if (string.IsNullOrEmpty(task.description))
                {
                    BaseObject obj = GameCodex.GetBaseObject(task.objectID);

                    if (obj.objectType == ObjectType.Item)
                    {
                        taskDescription = $"{Items.GetEmbellishedItemText((Item)obj, false, false, false)} {inventoryCount}/{task.amount}";
                    }
                    else if (obj.objectType != ObjectType.Quest || obj.objectType != ObjectType.Character)
                    {
                        taskDescription = $"{obj.name} {inventoryCount}/{task.amount}";
                    }
                    else if (obj.objectType == ObjectType.Character)
                    {
                        taskDescription = $"Improve relations with {Characters.FindByID(obj.objectID).NamePlate()}.";
                    }
                    else
                    {
                        taskDescription = "Progress " + obj.name;
                    }
                }
                else
                {
                    taskDescription = DialogueTagParser.ParseText(task.description);
                }

                if (inventoryCount >= task.amount)
                {
                    taskTrackerText.text += "<color=#718c81><s>" + taskDescription + "</s></color>";
                }

                else
                {
                    taskTrackerText.text += DialogueTagParser.ParseText(task.description);
                }

                taskTrackerText.text += "\n\n";
            }
        }
        else
        {
            btnTaskToggle.gameObject.SetActive(false);
            taskTrackerPanel.gameObject.SetActive(false);
        }

        displayDescription.text = DialogueTagParser.ParseText(description);
        detailContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
        detailContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
        detailContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }
}
