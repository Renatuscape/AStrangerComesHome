using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public List<Memory> debugMemoryList = Memories.all;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad;

    public Task StartLoading()
    {
        List<Task> loadingTasks = new List<Task>();

        gameObject.SetActive(true);
        filesLoaded = 0;
        numberOfFilesToLoad = 0;

        var info = new DirectoryInfo(Application.streamingAssetsPath + "/JsonData/Memories/");
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
    public class MemoryWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Memory[] memories;
    }
    public async Task LoadFromJsonAsync(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Memories/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
            MemoryWrapper dataWrapper = JsonUtility.FromJson<MemoryWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.memories != null)
                {
                    foreach (Memory memory in dataWrapper.memories)
                    {
                        memory.Initialise();
                        debugMemoryList.Add(memory);
                    }
                    filesLoaded++;

                    if (filesLoaded == numberOfFilesToLoad)
                    {
                        Debug.Log("All MEMORIES successfully loaded from Json.");
                    }
                }
                else
                {
                    Debug.LogError("Memories array is null in JSON data. Check that the list has a wrapper with the \'memories\' tag and that the object class is serializable.");
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
