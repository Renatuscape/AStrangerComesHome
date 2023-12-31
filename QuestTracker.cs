using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    public PopUpMenu popUpMenu;
    public bool hasCoolDown = true;
    public float timer = 0;
    public float questTick = 2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TransientDataScript.GameState == GameState.Overworld)
        {
            timer += 1f * Time.deltaTime;

            if (timer >= questTick)
            {
                hasCoolDown = false;
            }

            if (!hasCoolDown && popUpMenu.isActiveAndEnabled == false)
            {

                foreach (Quest quest in Quests.all)
                {
                    if (Player.GetEntry(quest.objectID, "TopicMenu", out IdIntPair entry))
                    {
                        if (entry.amount < quest.dialogues.Count) //make sure to exclude completed quests at stage 100
                        {
                            CheckQuestStage(quest, quest.dialogues[entry.amount]);
                        }
                    }
                    else //if quest is not already active and found in player journal
                    {
                        CheckQuestStage(quest, quest.dialogues[0]);
                    }
                }
            }
        }
    }

    void CheckQuestStage(Quest quest, Dialogue dialogue)
    {
        hasCoolDown = true;

        if (dialogue.stageType == StageType.PopUp)
        {
            if (dialogue.requirements.Count == 0)
            {
                popUpMenu.StartPopUp(dialogue);

                //Assuming pop-ups only have one choice for now:
                quest.SetQuestStage(dialogue.choices[0].advanceTo);

                foreach (string content in dialogue.content)
                {
                    Debug.Log(content);
                }
            }
        }
    }
}