using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public TransientDataScript transientData;
    public Character Player;
    public Character Alchemist;
    public Character Botanist;
    public Character Machinist;
    public Character Archaeologist;
    //All NPCs
    public Quest quest;
    public Character topicMaster;
    public string topicName;
    public int questStage;
    public List<dynamic> dialogueContent;

    public void AddQuestStep(Quest q, int step, Character topicMaster, string topicName, List<dynamic> dialogueContent)
    {
        //Debug.LogWarning($"AddQuestStep activated. Quest: {q}. Step: {step}. Topic Master: {topicMaster}. Topic Name: {topicName}");
        if (q.dialogues.Count > step)
        {
            if (q.dialogues[step] != null)
            {
                var dObject = q.dialogues[step];

                dObject.topicMaster = topicMaster;
                dObject.topicName = topicName;

                dObject.speakers.Clear();
                dObject.content.Clear();

                for (int index = 0; index < dialogueContent.Count; index++)
                {
                    if (dialogueContent[index] is Character)
                    {
                        dObject.speakers.Add(dialogueContent[index]);
                    }
                    else if (dialogueContent[index] is string)
                    {
                        dObject.content.Add(dialogueContent[index]);
                    }
                    else
                        Debug.LogWarning(dObject + " is not recognised as Character nor string.");
                }
            }
            else
                Debug.LogWarning("No Dialogue object found for index " + step + "in " + q);
        }
        else
            Debug.LogWarning("Quest stage (" +step+") exeeds the range of dialogues in " + q + ". Make sure that the dialogues list has a unique Dialogue object for each quest stage.");
    }

    public void Test()
    {
        questStage = 0;
        dialogueContent = new List<dynamic>();

        dialogueContent.AddMany
        (
        Player,
        "Hello!",
        Alchemist,
        "Had a good day?"
        );
        UpdateDialogue();

        dialogueContent.AddMany
        (
            Alchemist,
            "Have you heard about secret alchemy?",
            Player,
            "No! Could you tell me about it?",
            Alchemist,
            "Well...",
            Alchemist,
            "If you prove yourself worthy, I might reveal the secrets to you some day."
        );
        UpdateDialogue();

        void UpdateDialogue()
        {
            AddQuestStep(quest, questStage, Alchemist, "Test-topic", dialogueContent);
            questStage++;
            dialogueContent = new List<dynamic>();
        }
    }
}