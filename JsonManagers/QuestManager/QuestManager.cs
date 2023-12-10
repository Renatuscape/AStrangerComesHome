using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class QuestManager : MonoBehaviour
{
    public List<Quest> debugCharacterList = Quests.all;
    public bool allObjecctsLoaded = false;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad = 1;

    void Start()
    {
        LoadFromJson("Quests.json");
        //Remember to update numberOfFilesToLoad if more files are added
    }

    [System.Serializable]
    public class QuestWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Quest[] quests;
    }
    public void LoadFromJson(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Quests/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = File.ReadAllText(jsonPath);
            QuestWrapper dataWrapper = JsonUtility.FromJson<QuestWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.quests != null)
                {
                    foreach (Quest character in dataWrapper.quests)
                    {
                        InitialiseQuest(character, Quests.all);
                    }
                    filesLoaded++;
                    if (filesLoaded == numberOfFilesToLoad)
                    {
                        allObjecctsLoaded = true;
                        Debug.Log("All QUESTS successfully loaded from Json.");
                    }
                }
                else
                {
                    Debug.LogError("Character array is null in JSON data. Check that the list has a wrapper with the \'quests\' tag and that the object class is tagged serializable.");
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

    public static void InitialiseQuest(Quest quest, List<Quest> questList)
    {
        objectIDReader(ref quest);
        quest.dialogues = Dialogues.FindQuestDialogues(quest.objectID);
        questList.Add(quest);
    }

    public static void objectIDReader(ref Quest quest)
    {
        quest.questGiver = Characters.FindByID(quest.objectID.Substring(2, 6));
    }
}