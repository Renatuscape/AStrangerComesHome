using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    public PopUpMenu popUpMenu;
    public float timer;
    public float questTick;

    private void Awake()
    {
        timer = 0;
        questTick = 2;
    }
    void Update()
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
    void CheckActiveQuests()
    {
        foreach (Quest quest in Quests.all)
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
        timer = 0;
    }

    bool CheckQuestStage(Quest quest, Dialogue dialogue)
    {
        bool foundQuest = false;
        if (dialogue.stageType == StageType.PopUp)
        {
            if (CheckRequirements())
            {
                TransientDataScript.SetGameState(GameState.Dialogue, "QuestTracker", gameObject);

                popUpMenu.StartPopUp(dialogue);
                foundQuest = true;

                //Assuming pop-ups only have one choice for now:
                if (dialogue.choices is not null && dialogue.choices.Count > 0)
                {
                    quest.SetQuestStage(dialogue.choices[0].advanceTo);
                }
                else
                {
                    Player.AddDynamicObject(quest, 1, "Quest Tracker: quest auto-progressed one step after pop-up because no choice was found");
                }

                foreach (string content in dialogue.content)
                {
                    Debug.Log(content);
                }
            }
        }
        return foundQuest;
    }

    bool CheckRequirements()
    {
        return true;
    }
}
