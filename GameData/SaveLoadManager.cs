using SaveLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
/*
public class SaveLoadManager : MonoBehaviour
{
    public DataManagerScript dataManager;
    private PlayerData MyData = new PlayerData(); //creates savedata when the save function is called
    public List<MotherObject> motherList = new List<MotherObject>();

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
    }

    public void SaveGame()
    {
        GetScriptableObjectValues(dataManager.playerInventory, motherList);

        MyData.playerName = dataManager.playerName;
        MyData.playerNameColour = dataManager.playerNameColour;
        MyData.playerGold = dataManager.playerGold;

        MyData.mapPositionX = dataManager.mapPositionX;
        MyData.mapPositionY = dataManager.mapPositionY;
        MyData.timeOfDay = dataManager.timeOfDay;
        MyData.totalGameDays = dataManager.totalGameDays;

        MyData.playerInventory = dataManager.playerInventory;
        MyData.dataCollection = dataManager.dataCollection;

        //PASSENGER DATA - A
        MyData.passengerIsActiveA = dataManager.passengerIsActiveA;
        MyData.passengerNameA = dataManager.passengerNameA;
        MyData.passengerOriginA = dataManager.passengerOriginA;
        MyData.passengerDestinationA = dataManager.passengerDestinationA;
        MyData.passengerChatListA = dataManager.passengerChatListA;

        //PASSENGER DATA - B
        MyData.passengerIsActiveB = dataManager.passengerIsActiveB;
        MyData.passengerNameB = dataManager.passengerNameB;
        MyData.passengerOriginB = dataManager.passengerOriginB;
        MyData.passengerDestinationB = dataManager.passengerDestinationB;
        MyData.passengerChatListB = dataManager.passengerChatListB;

        //PLANTER - A
        MyData.planterSpriteA = dataManager.planterSpriteA;
        MyData.planterIsActiveA = dataManager.planterIsActiveA;
        MyData.seedA = dataManager.seedA;
        MyData.progressSeedA = dataManager.progressSeedA;
        MyData.seedHealthA = dataManager.seedHealthA;

        //PLANTER - B
        MyData.planterSpriteB = dataManager.planterSpriteB;
        MyData.planterIsActiveB = dataManager.planterIsActiveB;
        MyData.seedB = dataManager.seedB;
        MyData.progressSeedB = dataManager.progressSeedB;
        MyData.seedHealthB = dataManager.seedHealthB;

        //PLANTER - C
        MyData.planterSpriteC = dataManager.planterSpriteC;
        MyData.planterIsActiveC = dataManager.planterIsActiveC;
        MyData.seedC = dataManager.seedC;
        MyData.progressSeedC = dataManager.progressSeedC;
        MyData.seedHealthC = dataManager.seedHealthC;

        //ALCHEMY SYNTHESISER - A
        MyData.isSynthActiveA = dataManager.isSynthActiveA;
        MyData.synthItemA = dataManager.synthItemA;
        MyData.progressSynthA = dataManager.progressSynthA;
        MyData.isSynthPausedA = dataManager.isSynthPausedA;

        //ALCHEMY SYNTHESISER - B
        MyData.isSynthActiveB = dataManager.isSynthActiveB;
        MyData.synthItemB = dataManager.synthItemB;
        MyData.progressSynthB = dataManager.progressSynthB;
        MyData.isSynthPausedB = dataManager.isSynthPausedB;

        //ALCHEMY SYNTHESISER - C
        MyData.isSynthActiveC = dataManager.isSynthActiveC;
        MyData.synthItemC = dataManager.synthItemC;
        MyData.progressSynthC = dataManager.progressSynthC;
        MyData.isSynthPausedC = dataManager.isSynthPausedC;

        SaveGameManager.CurrentSaveData.PlayerData = MyData;
        SaveGameManager.SaveGameData();
    }

    public void LoadGame()
    {
        bool fileSuccessfullyLoaded = SaveGameManager.LoadGameData();

        if (fileSuccessfullyLoaded == true)
        {
            MyData = SaveGameManager.CurrentSaveData.PlayerData;

            dataManager.playerName = MyData.playerName;
            dataManager.playerNameColour = MyData.playerNameColour;
            dataManager.playerGold = MyData.playerGold;

            dataManager.mapPositionX = MyData.mapPositionX;
            dataManager.mapPositionY = MyData.mapPositionY;
            dataManager.timeOfDay = MyData.timeOfDay;
            dataManager.totalGameDays = MyData.totalGameDays;

            dataManager.playerInventory = MyData.playerInventory;
            dataManager.dataCollection = MyData.dataCollection;

            //PASSENGER DATA - A
            dataManager.passengerIsActiveA = MyData.passengerIsActiveA;
            dataManager.passengerNameA = MyData.passengerNameA;
            dataManager.passengerOriginA = MyData.passengerOriginA;
            dataManager.passengerDestinationA = MyData.passengerDestinationA;
            dataManager.passengerChatListA = MyData.passengerChatListA;

            //PASSENGER DATA - B
            dataManager.passengerIsActiveB = MyData.passengerIsActiveB;
            dataManager.passengerNameB = MyData.passengerNameB;
            dataManager.passengerOriginB = MyData.passengerOriginB;
            dataManager.passengerDestinationB = MyData.passengerDestinationB;
            dataManager.passengerChatListB = MyData.passengerChatListB;

            //PLANTER - A
            dataManager.planterSpriteA = MyData.planterSpriteA;
            dataManager.planterIsActiveA = MyData.planterIsActiveA;
            dataManager.seedA = MyData.seedA;
            dataManager.progressSeedA = MyData.progressSeedA;
            dataManager.seedHealthA = MyData.seedHealthA;

            //PLANTER - B
            dataManager.planterSpriteB = MyData.planterSpriteB;
            dataManager.planterIsActiveB = MyData.planterIsActiveB;
            dataManager.seedB = MyData.seedB;
            dataManager.progressSeedB = MyData.progressSeedB;
            dataManager.seedHealthB = MyData.seedHealthB;

            //PLANTER - C
            dataManager.planterSpriteC = MyData.planterSpriteC;
            dataManager.planterIsActiveC = MyData.planterIsActiveC;
            dataManager.seedC = MyData.seedC;
            dataManager.progressSeedC = MyData.progressSeedC;
            dataManager.seedHealthC = MyData.seedHealthC;

            //ALCHEMY SYNTHESISER - A
            dataManager.isSynthActiveA = MyData.isSynthActiveA;
            dataManager.synthItemA = MyData.synthItemA;
            dataManager.progressSynthA = MyData.progressSynthA;
            dataManager.isSynthPausedA = MyData.isSynthPausedA;

            //ALCHEMY SYNTHESISER - B
            dataManager.isSynthActiveB = MyData.isSynthActiveB;
            dataManager.synthItemB = MyData.synthItemB;
            dataManager.progressSynthB = MyData.progressSynthB;
            dataManager.isSynthPausedB = MyData.isSynthPausedB;

            //ALCHEMY SYNTHESISER - C
            dataManager.isSynthActiveC = MyData.isSynthActiveC;
            dataManager.synthItemC = MyData.synthItemC;
            dataManager.progressSynthC = MyData.progressSynthC;
            dataManager.isSynthPausedC = MyData.isSynthPausedC;

            SetScriptableObjectValues(dataManager.playerInventory, motherList);
        }
    }
    public void SetScriptableObjectValues(Dictionary<MotherObject, int> loadedInventory, List<MotherObject> allScriptableObjects)
    {
        for (int index = 0; index < allScriptableObjects.Count; index++)
        {
            var scriptableObject = allScriptableObjects[index];

            if (scriptableObject != null)
            {
                if (loadedInventory.ContainsKey(scriptableObject) == true)
                {
                    scriptableObject.dataValue = loadedInventory[scriptableObject];
                }
                else if (loadedInventory.ContainsKey(scriptableObject) == false)
                {
                    loadedInventory.Add(scriptableObject, scriptableObject.dataValue);
                }
            }
        }
    }

    public void GetScriptableObjectValues(Dictionary<MotherObject, int> dic, List<MotherObject> list)
    {
        for (int index = 0; index < list.Count; index++)
        {
            var x = list[index];

            if (dic.ContainsKey(x) == false)
                dic.Add(x, x.dataValue);

            else
                dic[x] = x.dataValue;
        }
    }
}

[System.Serializable]
public struct PlayerData
{
    public string playerName;
    public string playerNameColour;
    public int playerGold;

    public float mapPositionX;
    public float mapPositionY;
    public float timeOfDay;
    public int totalGameDays;

    public SerializableDictionary<MotherObject, int> playerInventory;
    public SerializableDictionary<string, int> dataCollection;

    //PASSENGER DATA - A
    public bool passengerIsActiveA;
    public string passengerNameA;
    public Sprite passengerSpriteA;
    public Location passengerOriginA;
    public Location passengerDestinationA;
    public List<string> passengerChatListA;

    //PASSENGER DATA - B
    public bool passengerIsActiveB;
    public string passengerNameB;
    public Sprite passengerSpriteB;
    public Location passengerOriginB;
    public Location passengerDestinationB;
    public List<string> passengerChatListB;

    //PLANTER - A
    public int planterSpriteA;
    public bool planterIsActiveA;
    public Seed seedA;
    public float progressSeedA;
    public int seedHealthA;

    //PLANTER - B
    public int planterSpriteB;
    public bool planterIsActiveB;
    public Seed seedB;
    public float progressSeedB;
    public int seedHealthB;

    //PLANTER - C
    public int planterSpriteC;
    public bool planterIsActiveC;
    public Seed seedC;
    public float progressSeedC;
    public int seedHealthC;

    //ALCHEMY SYNTHESISER - A
    public bool isSynthActiveA;
    public Item synthItemA;
    public float progressSynthA;
    public bool isSynthPausedA;

    //ALCHEMY SYNTHESISER - B
    public bool isSynthActiveB;
    public Item synthItemB;
    public float progressSynthB;
    public bool isSynthPausedB;

    //ALCHEMY SYNTHESISER - C
    public bool isSynthActiveC;
    public Item synthItemC;
    public float progressSynthC;
    public bool isSynthPausedC;
}
public static class SaveGameManager
{
    public static UnityAction OnSaveGame;
    public static UnityAction<DataManagerScript> OnLoadGame;

    public static SaveData CurrentSaveData = new SaveData();

    public const string SaveDirectory = "/SaveData/";
    public const string FileName = "SaveData.ran";

    public static void SaveGameData()
    {
        OnSaveGame?.Invoke();

        var dir = Application.persistentDataPath + SaveDirectory;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string json = JsonUtility.ToJson(CurrentSaveData, prettyPrint: true);
        File.WriteAllText(dir + FileName, contents: json);

        //GUIUtility.systemCopyBuffer = dir; //copies path to clipboard. Remove later!
        Debug.Log("Game saved at " + dir); //write dir in console
    }

    public static bool LoadGameData()
    {
        string fullPath = Application.persistentDataPath + SaveDirectory + FileName;
        SaveData tempData = new SaveData();

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            tempData = JsonUtility.FromJson<SaveData>(json);
            CurrentSaveData = tempData;
            return true;
        }

        else
        {
            Debug.LogError(message: "Save file does not exist!");
            return false;
        }
    }
}*/

//NEW ATTEMPT STARTS HERE
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