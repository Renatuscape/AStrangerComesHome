using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    public PopUpMenu popUpMenu;
    public float timer;
    public float questTick;

    bool isEnabled = false;

    private void Awake()
    {
        if (!isEnabled)
        {
            gameObject.SetActive(false);
        }
    }
    public void StartTracking()
    {
        Debug.Log("Enabling Quest Tracker.");

        questTick = 3;
        timer = 0;
        isEnabled = true;
        gameObject.SetActive(true);
    }
    void Update()
    {
        if (isEnabled)
        {
            if (TransientDataScript.GameState == GameState.Overworld)
            {
                timer += 1f * Time.deltaTime;

                if (timer >= questTick)
                {
                    CheckActiveQuests();
                }
            }
        }
    }
    void CheckActiveQuests()
    {
        foreach (Quest quest in Quests.all)
        {
            if (quest.dialogues != null && quest.dialogues.Count > 0) //ensure that at least one dialogue entry exists
            {
                if (Player.GetEntry(quest.objectID, "TopicMenu", out IdIntPair entry))
                {
                    if (entry.amount < quest.dialogues.Count) //make sure to exclude quests with no remaining steps
                    {
                        if (CheckQuestStage(quest, quest.dialogues[entry.amount]))
                        {
                            break;
                        }
                    }
                }
                else //if quest is not already active and found in player journal
                {
                    if (CheckQuestStage(quest, quest.dialogues[0]))
                    {
                        break;
                    }
                }
            }
        }
        timer = 0;
    }

    bool CheckQuestStage(Quest quest, Dialogue dialogue)
    {
        if (dialogue.stageType == StageType.PopUp)
        {
            if (CheckRequirements(dialogue))
            {
                TransientDataScript.SetGameState(GameState.Dialogue, "QuestTracker", gameObject);

                popUpMenu.StartPopUp(dialogue);

                //Assuming pop-ups only have one choice for now:
                if (dialogue.choices != null && dialogue.choices.Count > 0)
                {
                    quest.SetQuestStage(dialogue.choices[0].advanceTo);
                }
                else
                {
                    Player.AddDynamicObject(quest, 1, false, "Quest Tracker: quest auto-progressed one step after pop-up because no choice was found");
                }

                return true;
            }
        }

        return false;
    }

    bool CheckRequirements(Dialogue dialogue)
    {
        return dialogue.CheckRequirements();
    }
}

public static class QuestResetter
{
    public static List<Quest> questsAdvancingDaily;
    public static List<Quest> questsAdvancingWeekly;
    public static List<Quest> questsAdvancingMonthly;
    public static List<Quest> questsAdvancingYearly;
    public static List<Quest> questsResettingOnComplete;
    static bool questsFound = false;
    public static void Tick()
    {
        if (!questsFound)
        {
            FindQuests();
            questsFound = true;
        }

        int daysPassed = TransientDataCalls.GetDaysPassed();

        foreach (Quest quest in questsAdvancingDaily)
        {
            Player.Add(quest.objectID);
        }
        if (daysPassed % 7 == 0)
        {
            Debug.Log("QuestTracker registered weekly advance but is not implemented.");
        }
        if (daysPassed % 28 == 0)
        {
            Debug.Log("QuestTracker registered monthly advance but is not implemented.");
        }
        if (daysPassed % 336 == 0)
        {
            Debug.Log("QuestTracker registered yearly advance but is not implemented.");
        }

        foreach (Quest quest in questsResettingOnComplete)
        {
            if (Random.Range(0, 100) < quest.resetChance)
            {
                int currentStage = Player.GetCount(quest.objectID, "QuestResetter");

                if (currentStage >= 100 || currentStage >= quest.dialogues.Count)
                {
                    if (quest.resetToRandomStage)
                    {
                        Player.Set(quest.objectID, Random.Range(0, quest.dialogues.Count));
                    }
                    else
                    {
                        Player.Set(quest.objectID, 0);
                    }
                }
            }
        }
    }

    public static void FindQuests()
    {
        questsAdvancingDaily = Quests.all.Where(q => q.advanceEveryDay).ToList();
        questsAdvancingWeekly = Quests.all.Where(q => q.advanceEveryWeek).ToList();
        questsAdvancingMonthly = Quests.all.Where(q => q.advanceEveryMonth).ToList();
        questsAdvancingYearly = Quests.all.Where(q => q.advanceEveryYear).ToList();

        questsResettingOnComplete = Quests.all.Where(q => q.resetOnComplete || q.resetToRandomStage).ToList();
    }
}