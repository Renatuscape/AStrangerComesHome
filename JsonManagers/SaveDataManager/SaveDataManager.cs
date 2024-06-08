using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    int filesLoaded;
    int numberOfFilesToLoad;
    public List<SaveDataInfo> savesInfo;
    public bool allObjectsLoaded = false;
    string dir;

    private async void OnEnable()
    {
        savesInfo.Clear();

        await StartLoading();
        Debug.Log("STARTUP: Loading save data completed");

        PrintSaveFiles();
    }


    public void PrintSaveFiles()
    {

    }

    public Task StartLoading()
    {
        List<Task> loadingTasks = new List<Task>();

        gameObject.SetActive(true);
        filesLoaded = 0;
        numberOfFilesToLoad = 0;

        string saveDirectory = "/SaveData/";
        //string fileName = "SaveData.ran";
        dir = Application.persistentDataPath + saveDirectory;
        Debug.Log("Looking for save files in " + dir);
        var info = new DirectoryInfo(dir);
        var fileInfo = info.GetFiles();

        foreach (var file in fileInfo)
        {
            Debug.Log("Found file named " + file.Name);
            if (file.Extension == ".ran")
            {
                numberOfFilesToLoad++;
                Task loadingTask = LoadFromJsonAsync(Path.GetFileName(file.FullName)); // Pass only the file name
                loadingTasks.Add(loadingTask);
            }
        }

        return Task.WhenAll(loadingTasks);
    }

    [System.Serializable]
    public class ItemsWrapper //Necessary for Unity to read the .json contents as an object
    {
        public SaveDataInfo[] saveData;
    }
    public async Task LoadFromJsonAsync(string fileName)
    {

        string jsonPath = dir + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
            SaveDataInfo saveInfo = JsonUtility.FromJson<SaveDataInfo>(jsonData);

            saveInfo.fileName = fileName;
            savesInfo.Add(saveInfo);

            if (saveInfo.version != Application.version)
            {
                Debug.LogWarning($"Save data version mismatch. Save data was created in version {saveInfo.version} and was last saved duing {saveInfo.lastVersionSaved}. Current version is {Application.version}.");
            }

            filesLoaded++;
            if (filesLoaded == numberOfFilesToLoad)
            {
                allObjectsLoaded = true;
                Debug.Log("All SAVES successfully loaded from Json.");
            }
        }
    }
}

[System.Serializable]
public class SaveDataInfo
{
    public string fileName;
    public string version;
    public string lastVersionSaved;
    public string playerName;
    public int totalGameDays;

    public float mapPositionX;
    public float mapPositionY;
    public float timeOfDay;
}
