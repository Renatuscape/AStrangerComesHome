using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    public List<Quest> questList = Quests.all;
    public bool allObjecctsLoaded = false;
    public int filesLoaded;
    public int numberOfFilesToLoad;

    public Task StartLoading()
    {
        List<Task> loadingTasks = new List<Task>();

        gameObject.SetActive(true);
        filesLoaded = 0;
        numberOfFilesToLoad = 0;

        var info = new DirectoryInfo(Application.streamingAssetsPath + "/JsonData/Quests/");
        var fileInfo = info.GetFiles();

        foreach (var file in fileInfo)
        {
            if (file.Extension == ".json")
            {
                numberOfFilesToLoad++;
                Task loadingTask = LoadFromJsonAsync(Path.GetFileName(file.FullName)); // Pass only the file name
                loadingTasks.Add(loadingTask);
            }
        }

        return Task.WhenAll(loadingTasks);
    }

    [System.Serializable]
    public class QuestWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Quest[] quests;
    }

    public async Task LoadFromJsonAsync(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Quests/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
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

        if (filesLoaded == numberOfFilesToLoad)
        {
            allObjecctsLoaded = true;
            Debug.Log("All QUESTS successfully loaded from Json.");
        }
    }

    public static void InitialiseQuest(Quest quest, List<Quest> questList)
    {
        quest.objectType = BaseObjectType.Quest;
        quest.maxValue = StaticGameValues.maxQuestValue;
        objectIDReader(ref quest);
        if (quest.dialogues == null || quest.dialogues.Count == 0)
        {
            quest.dialogues = Dialogues.FindQuestDialogues(quest.objectID);
        }
        else
        {
            foreach (Dialogue dialogue in quest.dialogues)
            {
                DialogueSetup.InitialiseDialogue(dialogue);
                Dialogues.all.Add(dialogue);
            }
        }

        questList.Add(quest);
    }

    public static void objectIDReader(ref Quest quest)
    {
        quest.questGiver = Characters.FindByID(quest.objectID.Substring(0, 6));
    }
}