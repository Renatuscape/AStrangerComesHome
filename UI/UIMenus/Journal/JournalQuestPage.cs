using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalQuestPage : MonoBehaviour
{
    public FontManager fontManager;
    public PageinatedList pageinatedList;
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
        SetDefaultText();
        btnTaskToggle.onClick.AddListener(() =>
        {
            taskTrackerPanel.SetActive(!taskTrackerPanel.activeInHierarchy);
        });
    }

    void SetDefaultText()
    {
        displayTitle.text = "";
        displayTopicName.text = "Details";
        displayDescription.text = "Choose a quest to see the details.\nA '?' button will appear for quests with a to-do list.";
    }
    private void OnEnable()
    {
        //displayTitle.font = fontManager.header.font;
        //displayTopicName.font = fontManager.subtitle.font;
        //displayDescription.font = fontManager.script.font;
        //pageTitle.font = fontManager.header.font;

        btnTaskToggle.gameObject.SetActive(false);
        taskTrackerPanel.gameObject.SetActive(false);

        var questList = pageinatedList.InitialiseWithoutCategories(GetActiveQuestsForPageination());

        if (questList != null && questList.Count > 0)
        {
            AddButtonFunctionality(questList);
        }
    }

    void AddButtonFunctionality(List<GameObject> prefabs)
    {
        foreach (var prefab in prefabs)
        {
            var btn = prefab.GetComponent<Button>();

            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    DisplayQuestDetails(Quests.FindByID(prefab.GetComponent<ListItemPrefab>().entry.objectID));
                });
            }
        }
    }

    List<Quest> GetActiveQuests()
    {
        List<Quest> activeQuests = new();

        foreach (var quest in Quests.all)
        {
            if (!quest.excludeFromJournal && Player.GetEntry(quest.objectID, name, out var entry))
            {
                if (entry.amount < 100) // Exclude completed quests here for now. Make separate complete and active category later
                {
                    activeQuests.Add(quest);
                }
            }
        }

        return activeQuests;
    }

    List<IdIntPair> GetActiveQuestsForPageination()
    {
        List<IdIntPair> activeQuests = new();

        foreach (var quest in GetActiveQuests())
        {
            var entry = new IdIntPair();

            entry.objectID = quest.objectID;
            entry.description = quest.name;
            activeQuests.Add(entry);
        }

        return activeQuests;
    }

    private void OnDisable()
    {
        foreach (var quest in questPrefabs)
        {
            Destroy(quest);
        }

        SetDefaultText();
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

        if (dialogue.taskTracking != null && dialogue.taskTracking.Count > 0)
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
                    taskTrackerText.text += DialogueTagParser.ParseText(taskDescription);
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
