
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

    public void SaveGame(string fileName)
    {
        dataManager.lastVersionSaved = Application.version;

        string json = JsonUtility.ToJson(dataManager, prettyPrint: true);
        SaveJsonToFile(json, fileName);
    }

    private void SaveJsonToFile(string json, string fileName)
    {
        string fullPath = GetSaveFilePath(fileName);
        File.WriteAllText(fullPath, json);
        Debug.Log("Game saved at " + fullPath);
    }

    public async void LoadGame(string fileName)
    {
        //TransientDataScript.SetGameState(GameState.Loading, name, gameObject);

        string json = await LoadJsonFromFileAsync(fileName);

        // Log the JSON value before loading
        Debug.Log("Loaded JSON data: " + json);

        if (!string.IsNullOrEmpty(json))
        {
            JsonUtility.FromJsonOverwrite(json, dataManager);
            gameManager.LoadRoutine();
        }
    }

    private string GetSaveFilePath(string fileName)
    {
        string saveDirectory = "/SaveData/";
        string dir = Application.persistentDataPath + saveDirectory;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return Path.Combine(dir, fileName);
    }

    private async Task<string> LoadJsonFromFileAsync(string fileName)
    {
        string fullPath = GetSaveFilePath(fileName);
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