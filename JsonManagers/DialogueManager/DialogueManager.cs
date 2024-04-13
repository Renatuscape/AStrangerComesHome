using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using UnityEngineInternal;
using System;
using System.Threading.Tasks;

public class DialogueManager : MonoBehaviour
{
    public List<Dialogue> debugCharacterList = Dialogues.all;
    public bool allObjecctsLoaded = false;
    public int filesLoaded;
    public int numberOfFilesToLoad;

    public Task StartLoading()
    {
        List<Task> loadingTasks = new List<Task>();

        gameObject.SetActive(true);
        filesLoaded = 0;
        numberOfFilesToLoad = 0;

        var info = new DirectoryInfo(Application.streamingAssetsPath + "/JsonData/Quests/Dialogues/");
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
    public class DialogueWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Dialogue[] dialogues;
    }
    public async Task LoadFromJsonAsync(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Quests/Dialogues/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
            DialogueWrapper dataWrapper = JsonUtility.FromJson<DialogueWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.dialogues != null)
                {
                    foreach (Dialogue dialogue in dataWrapper.dialogues)
                    {
                        DialogueSetup.InitialiseDialogue(dialogue);
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

}
