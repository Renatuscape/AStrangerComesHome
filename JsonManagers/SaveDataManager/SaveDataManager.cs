using System;
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
    public List<SaveDataPrefab> saveSlotPrefabs = new();
    public FileInfo[] loadedFiles;

    string fileExtension = ".strange";
    string dir;
    int filesLoaded;
    int numberOfFilesToLoad;

    public void LoadGameAndPlay(SaveDataInfo saveData)
    {
        saveLoadManager.LoadGame(saveData.fileName);

        gameObject.SetActive(false);
    }

    public void QuickSave()
    {
        var dataManager = TransientDataScript.gameManager.dataManager;

        if (dataManager.saveSlot < 0)
        {
            TransientDataScript.PushAlert("Choose a save slot to enable Quick Save.");
            gameObject.SetActive(true);
        }
        else
        {
            string savePrefix = "Save" + dataManager.saveSlot + "_";
            string saveName = savePrefix + dataManager.playerName + "_" + dataManager.saveID + fileExtension;

            ClearSlotAndFinalise(savePrefix, saveName);

            TransientDataScript.ReturnToOverWorld(name, gameObject);
        }
    }

    public void AttemptSave(SaveDataInfo saveData, SaveDataPrefab slotPrefab, bool overwriteConfirmed)
    {
        var dataManager = TransientDataScript.gameManager.dataManager;
        string savePrefix = "Save" + dataManager.saveSlot + "_";

        string saveNameToCheck = savePrefix + dataManager.playerName + "_" + dataManager.saveID + fileExtension;

        if (saveData == null)
        {
            FinaliseSave(saveNameToCheck);
        }
        else
        {
            if (saveData.fileName != saveNameToCheck)
            {
                Debug.LogWarning("File does not match. Savedata to overwrite was " + saveData.fileName + " but new file should be named " + saveNameToCheck);

                if (overwriteConfirmed)
                {
                    DeleteFileAndRefresh(saveData);
                    FinaliseSave(saveNameToCheck);
                }
                else
                {
                    slotPrefab.ConfirmOverwrite();
                }
            }
            else
            {
                if (savesInfo.FirstOrDefault(s => s.fileName.Contains(savePrefix)) != null)
                {
                    ClearSlotAndFinalise(savePrefix, saveData.fileName);
                }
                else
                {
                    FinaliseSave(saveData.fileName);
                }

            }
        }
    }

    async void ClearSlotAndFinalise(string saveSlotName, string fileName)
    {
        if (gameObject.activeInHierarchy && savesInfo.Count > 0)
        {
            ClearSlot();
        }
        else
        {
            await StartLoading();

            if (savesInfo.Count > 0)
            {
                ClearSlot();
            }
        }

        FinaliseSave(fileName);

        void ClearSlot()
        {
            List<SaveDataInfo> filesToDelete = new();

            foreach (var file in savesInfo)
            {
                Debug.Log("Checking file to clear: " + file.fileName);

                if (file.fileName.Contains(saveSlotName))
                {
                    filesToDelete.Add(file);
                }
            }

            foreach (var file in filesToDelete)
            {
                Debug.Log("Deleting " + file.fileName);
                DeleteFileAndRefresh(file);
            }
        }
    }

    public void FinaliseSave(string fileName)
    {
        Debug.Log("Save Game and Play was called.");
        TransientDataScript.gameManager.dataManager.lastSaveTime = DateTime.Now.ToShortTimeString() + ", " + DateTime.Now.ToShortDateString();
        saveLoadManager.SaveGame(fileName);
        gameObject.SetActive(false);
    }

    public void DeleteFileAndRefresh(SaveDataInfo saveData)
    {

        string filePath = dir + saveData.fileName;

        // check if file exists
        if (!File.Exists(filePath))
        {
            Debug.Log("no " + saveData.fileName + " file exists");
        }
        else
        {
            Debug.Log(saveData.fileName + " file exists, deleting...");

            File.Delete(filePath);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }

    void ClearOldData()
    {
        foreach (Transform child in saveSlotContainer.transform)
        {
            Destroy(child.gameObject);
        }

        saveSlotPrefabs.Clear();
        savesInfo.Clear();
    }
    private void OnDisable()
    {
        foreach (Transform child in saveSlotContainer.transform)
        {
            Destroy(child.gameObject);
        }

        saveSlotPrefabs.Clear();
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

        while (printedSlots < 4)
        {
            var newSlot = Instantiate(saveSlotPrefab);

            newSlot.transform.SetParent(saveSlotContainer.transform, false);

            var script = newSlot.GetComponent<SaveDataPrefab>();
            script.saveSlot = printedSlots;
            saveSlotPrefabs.Add(script);
            printedSlots++;
        }

        var savesFound = new List<SaveDataInfo>(savesInfo);

        foreach (var slot in saveSlotPrefabs)
        {
            bool foundMatchingData = false;

            foreach (var saveInfo in savesFound)
            {
                if (saveInfo.saveSlot == slot.saveSlot)
                {
                    // Debug.Log("Found matching slot!");
                    savesFound.Remove(saveInfo);
                    slot.InitialiseWithData(slot.saveSlot, saveInfo, this);
                    foundMatchingData = true;
                    break;
                }
            }

            if (!foundMatchingData)
            {
                slot.InitialiseEmpty(slot.saveSlot, this);
            }
        }
    }

    public Task StartLoading()
    {
        ClearOldData();
        List<Task> loadingTasks = new List<Task>();

        gameObject.SetActive(true);
        filesLoaded = 0;
        numberOfFilesToLoad = 0;

        string saveDirectory = "/SaveData/";
        //string fileName = "SaveData.ran";
        dir = Application.persistentDataPath + saveDirectory;
        Debug.Log("Looking for save files in " + dir);
        var info = new DirectoryInfo(dir);
        loadedFiles = info.GetFiles();

        foreach (var file in loadedFiles)
        {
            // Debug.Log("Found file named " + file.Name);
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

            if (saveInfo.lastVersionSaved != Application.version)
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
    public int saveSlot;
    public string lastSaveTime;
    public string version;
    public string lastVersionSaved;
    public string playerName;
    public string playerNameColour;
    public int totalGameDays;

    public string currentRegion;
    public float mapPositionX;
    public float mapPositionY;
    public float timeOfDay;
    public bool versionMismatch;
    public bool versionIncompatible;
}
