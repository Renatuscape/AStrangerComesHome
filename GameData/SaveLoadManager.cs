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
        PlayerInventory.UpdateDataManager();
        GetScriptableObjectValues(dataManager.playerInventory, transientData.objectIndex);

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

        SetScriptableObjectValues(dataManager.playerInventory, transientData.objectIndex);
        PlayerInventory.UpdateDataManager();
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

    public void SetScriptableObjectValues(Dictionary<string, int> loadedInventory, List<MotherObject> allScriptableObjects)
    {
        for (int index = 0; index < allScriptableObjects.Count; index++)
        {
            var scriptableObject = allScriptableObjects[index];

            if (scriptableObject != null)
            {
                if (loadedInventory.ContainsKey(scriptableObject.name) == true)
                {
                    scriptableObject.dataValue = loadedInventory[scriptableObject.name];
                }
                else if (loadedInventory.ContainsKey(scriptableObject.name) == false)
                {
                    loadedInventory.Add(scriptableObject.name, 0);
                    scriptableObject.dataValue = 0;
                }
            }
        }
    }

    public void GetScriptableObjectValues(Dictionary<string, int> dic, List<MotherObject> list)
    {
        for (int index = 0; index < list.Count; index++)
        {
            var x = list[index];

            if (dic.ContainsKey(x.name) == false)
                dic.Add(x.name, x.dataValue);

            else
                dic[x.name] = x.dataValue;
        }
    }

}