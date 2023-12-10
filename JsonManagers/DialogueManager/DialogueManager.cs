using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

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
                        dialogue.objectID = dialogue.questID + "-s" + dialogue.stage;
                        InitialiseDialogue(dialogue, Dialogues.all);
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

    public static void InitialiseDialogue(Dialogue dialogue, List<Dialogue> questList)
    {
        objectIDReader(ref dialogue);
        questList.Add(dialogue);
    }

    public static void objectIDReader(ref Dialogue dialogue)
    {

    }

    public static void ContentCreator(ref Dialogue dialogue)
    {

    }
}