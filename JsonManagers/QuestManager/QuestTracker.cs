using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    public PopUpMenu popUpMenu;

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
        isEnabled = true;
        gameObject.SetActive(true);
    }

    public void StopTracking()
    {
        isEnabled = false;
    }
    public void RunCheck()
    {
        Debug.Log("QuestTracker running checks.");

        if (isEnabled)
        {
            if (TransientDataScript.GameState == GameState.Overworld)
            {
                SearchForPopUps();
            }
        }
    }
    void SearchForPopUps()
    {
        List<Dialogue> relevantDialogues = new();

        foreach (Quest quest in Quests.all)
        {
            int stage = Player.GetCount(quest.objectID, name);

            if (quest.dialogues != null &&
                quest.dialogues.Count > 0
                && quest.dialogues.Count >= stage
                && stage < 100)
            {
                Dialogue dialogue = quest.dialogues.FirstOrDefault(d => d.questStage == stage);

                if (dialogue != null && dialogue.stageType == StageType.PopUp)
                {
                    Debug.Log($"Added {quest.objectID} at stage {stage} to relevant dialogue list for pop-ups.");
                    relevantDialogues.Add(dialogue);
                }
            }
        }

        foreach (Dialogue dialogue in relevantDialogues)
        {
            CheckQuestStage(dialogue);
        }
    }

    void CheckQuestStage(Dialogue dialogue)
    {
        if (CheckRequirements(dialogue))
        {
            StartCoroutine(InitiatePop(dialogue));
        }
    }

    IEnumerator InitiatePop(Dialogue dialogue)
    {
        yield return new WaitForSeconds(1);

        if (TransientDataScript.GameState == GameState.Overworld)
        {
            TransientDataScript.SetGameState(GameState.Dialogue, "QuestTracker", gameObject);

            popUpMenu.StartPopUp(dialogue);

            //Assuming pop-ups only have one choice for now:
            if (dialogue.choices != null && dialogue.choices.Count > 0)
            {
                Player.Set(dialogue.questID, dialogue.choices[0].advanceTo);
            }
            else
            {
                Player.Add(dialogue.questID, 1, false);
            }
        }
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

        int daysPassed = TransientDataScript.GetDaysPassed();

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