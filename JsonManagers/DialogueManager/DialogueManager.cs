using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using UnityEngineInternal;
using System;

public class DialogueManager : MonoBehaviour
{
    public List<Dialogue> debugCharacterList = Dialogues.all;
    public bool allObjecctsLoaded = false;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad = 1;

    void Start()
    {
        var info = new DirectoryInfo(Application.streamingAssetsPath + "/JsonData/Quests/Dialogues/");
        var fileInfo = info.GetFiles();
        numberOfFilesToLoad = fileInfo.Count();
        foreach (var file in fileInfo)
        {
            if (file.Extension != ".json")
            {
                numberOfFilesToLoad -= 1;
            }
            else
            {
                LoadFromJson(Path.GetFileName(file.FullName)); // Pass only the file name
            }
        }
    }

    [System.Serializable]
    public class DialogueWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Dialogue[] dialogues;
    }
    public void LoadFromJson(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Quests/Dialogues/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = File.ReadAllText(jsonPath);
            DialogueWrapper dataWrapper = JsonUtility.FromJson<DialogueWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.dialogues != null)
                {
                    foreach (Dialogue dialogue in dataWrapper.dialogues)
                    {
                        InitialiseDialogue(dialogue);
                        filesLoaded++;
                    }

                    if (filesLoaded == numberOfFilesToLoad)
                    {
                        allObjecctsLoaded = true;
                        Debug.Log("All DIALOGUE successfully loaded from Json.");
                    }
                }
                else
                {
                    Debug.LogError("Dialogue objet is null in JSON data. Check that the list has a wrapper with the \'dialogue\' tag and that the object class is tagged serializable.");
                }
            }
            else
            {
                Debug.LogError("JSON data is malformed. No wrapper found?");
                Debug.Log(jsonData); // Log the JSON data for inspection
            }
        }
        else
        {
            Debug.LogError("JSON file not found: " + jsonPath);
        }
    }

    public static void InitialiseDialogue(Dialogue dialogue)
    {
        if (string.IsNullOrEmpty(dialogue.objectID))
        {
            Debug.Log($"Missing objectID for {dialogue}");
        }
        else
        {
            ParseDialogueID(dialogue);

            if (dialogue.stageType == StageType.Dialogue)
            {
                ParseDialogueSteps(dialogue); //set up dialogue steps with proper speaker objects
            }

            Dialogues.all.Add(dialogue);
        }
    }
    static void ParseDialogueID(Dialogue dialogue)
    {
        //SET STAGE TYPE
        if (dialogue.objectID.Substring(14, 1) == "M")
        {
            dialogue.stageType = StageType.Memory;
        }
        else if (dialogue.objectID.Substring(14, 1) == "P")
        {
            dialogue.stageType = StageType.PopUp;
        }

        //SET QUEST STAGE
        dialogue.questStage = int.Parse(dialogue.objectID.Substring(11, 2));

        //SET QUEST ID
        dialogue.questID = dialogue.objectID.Substring(0, 10);
    }
    static void ParseDialogueSteps(Dialogue dialogue)
    {
        //Debug.Log($"parsing ${dialogue.objectID}" +
        //    $"\n Content count: {dialogue.content.Count}");

        for (int i = 0; i < dialogue.content.Count; i = i + 2)
        {
            string speakerTag = dialogue.content[i];
            Character foundSpeaker = Characters.all.Find((s) => s.dialogueTag.ToLower() == speakerTag.ToLower());
            if (foundSpeaker is not null)
            {
                DialogueStep step = new() { name = $"{foundSpeaker.name} - step{Mathf.FloorToInt(i / 2 + 1)}" };
                step.speaker = foundSpeaker;
                step.text = dialogue.content[i + 1];
                //Debug.Log($"New step {i} - {i + 1}| {step.speaker}: {step.text}");
                dialogue.dialogueSteps.Add(step);
            }
            else
            {
                Debug.LogWarning($"Could not parse dialogue content for {dialogue.questID} stage {dialogue.questStage} because speaker tag \"{speakerTag}\" return null.");
            }

        }
    }
}