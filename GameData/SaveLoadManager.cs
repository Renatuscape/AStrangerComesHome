using SaveLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;


public class SaveLoadManager : MonoBehaviour
{
    public DataManagerScript dataManager;
    public TransientDataScript transientData;

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(dataManager, prettyPrint: true);
        SaveJsonToFile(json);
    }

    public void LoadGame()
    {
        string json = LoadJsonFromFile();
        if (!string.IsNullOrEmpty(json))
        {
            JsonUtility.FromJsonOverwrite(json, dataManager);
        }
    }

    private void SaveJsonToFile(string json)
    {
        string fullPath = GetSaveFilePath();
        File.WriteAllText(fullPath, json);
        Debug.Log("Game saved at " + fullPath);
    }

    private string LoadJsonFromFile()
    {
        string fullPath = GetSaveFilePath();
        if (File.Exists(fullPath))
        {
            return File.ReadAllText(fullPath);
        }

        Debug.LogError("Save file does not exist!");
        return null;
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

}