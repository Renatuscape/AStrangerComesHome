using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    public SaveLoadManager saveLoadManager;
    public GameObject saveSlotPrefab;
    public GameObject saveSlotContainer;
    public List<SaveDataInfo> savesInfo;
    public bool allObjectsLoaded = false;
    public List<string> backwardsCompatibleVersions = new();

    string fileExtension = ".ren";
    string dir;
    int filesLoaded;
    int numberOfFilesToLoad;

    public void LoadGameAndPlay(SaveDataInfo saveData)
    {
        saveLoadManager.LoadGame(saveData.fileName);

        gameObject.SetActive(false);
    }

    public void SaveGameAndPlay(SaveDataInfo saveData)
    {
        string fileName;

        if (saveData == null)
        {
            fileName = (filesLoaded + 1) + "_" + TransientDataScript.gameManager.dataManager.playerName + "_" + Random.Range(0, 1000000) + fileExtension;
        }
        else
        {
            fileName = saveData.fileName;
        }

        Debug.Log("Save Game and Play was called.");
        saveLoadManager.SaveGame(fileName);
        gameObject.SetActive(false);
    }

    public void DeleteFileAndRefresh(SaveDataInfo saveData)
    {

        string filePath = dir + saveData.fileName;

        // check if file exists
        if (!File.Exists(filePath))
        {
            Debug.Log( "no " + saveData.fileName + " file exists" );
        }
        else
        {
            Debug.Log( saveData.fileName + " file exists, deleting..." );

            File.Delete(filePath);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in saveSlotContainer.transform)
        {
            Destroy(child.gameObject);
        }

        savesInfo.Clear();
    }

    private async void OnEnable()
    {
        savesInfo.Clear();

        await StartLoading();
        Debug.Log("STARTUP: Loading save data completed");

        PrintSaveFiles();
    }


    public void PrintSaveFiles()
    {
        int printedSlots = 0;

        foreach (var saveInfo in savesInfo)
        {
            var newSlot = Instantiate(saveSlotPrefab);

            newSlot.transform.SetParent(saveSlotContainer.transform, false);

            var script = newSlot.GetComponent<SaveDataPrefab>();
            script.InitialiseWithData(saveInfo, this);
            printedSlots++;
        }

        if (printedSlots < 4)
        {
            while (printedSlots < 4)
            {
                var newSlot = Instantiate(saveSlotPrefab);

                newSlot.transform.SetParent(saveSlotContainer.transform, false);

                var script = newSlot.GetComponent<SaveDataPrefab>();
                script.InitialiseEmpty(this);
                printedSlots++;
            }
        }
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
            if (file.Extension == fileExtension)
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

            if (string.IsNullOrEmpty(saveInfo.fileName))
            {
                saveInfo.fileName = fileName;
            }

            savesInfo.Add(saveInfo);

            if (saveInfo.version != Application.version)
            {
                Debug.LogWarning($"Save data version mismatch. Save data was created in version {saveInfo.version} and was last saved duing {saveInfo.lastVersionSaved}. Current version is {Application.version}.");
                saveInfo.versionMismatch = true;

                if (backwardsCompatibleVersions.FirstOrDefault(s => s == saveInfo.lastVersionSaved) == null)
                {
                    Debug.LogWarning($"Current game version is not compatible with saved data. Cannot load game.");
                    saveInfo.versionIncompatible = true;
                }
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
    public string playerNameColour;
    public int totalGameDays;

    public float mapPositionX;
    public float mapPositionY;
    public float timeOfDay;
    public bool versionMismatch;
    public bool versionIncompatible;
}
