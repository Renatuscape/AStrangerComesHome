
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;


public class SaveLoadManager : MonoBehaviour
{
    public DataManagerScript dataManager;
    public TransientDataScript transientData;
    public GameManagerScript gameManager;

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    public void SaveGame()
    {
        dataManager.lastVersionSaved = Application.version;

        string json = JsonUtility.ToJson(dataManager, prettyPrint: true);
        SaveJsonToFile(json);
    }

    public async void LoadGame()
    {
        //TransientDataScript.SetGameState(GameState.Loading, name, gameObject);

        string json = await LoadJsonFromFileAsync();

        // Log the JSON value before loading
        Debug.Log("Loaded JSON data: " + json);

        if (!string.IsNullOrEmpty(json))
        {
            JsonUtility.FromJsonOverwrite(json, dataManager);
            gameManager.LoadRoutine();
        }
    }
    private void SaveJsonToFile(string json)
    {
        string fullPath = GetSaveFilePath();
        File.WriteAllText(fullPath, json);
        Debug.Log("Game saved at " + fullPath);
    }

    private string GetSaveFilePath()
    {
        string saveDirectory = "/SaveData/";
        string fileName = "SaveData.ran";
        string dir = Application.persistentDataPath + saveDirectory;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return Path.Combine(dir, fileName);
    }

    private async Task<string> LoadJsonFromFileAsync()
    {
        string fullPath = GetSaveFilePath();
        if (File.Exists(fullPath))
        {
            using (StreamReader reader = new StreamReader(fullPath))
            {
                return await reader.ReadToEndAsync();
            }
        }
        else
        {
            Debug.LogError("Save file does not exist!");
            return null;
        }
    }

}