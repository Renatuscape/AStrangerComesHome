using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDataPrefab : MonoBehaviour
{
    public SaveDataInfo saveData;
    public SaveDataManager saveDataManager;
    public TextMeshProUGUI displayInfo;
    public Button btnSave;
    public Button btnLoad;
    public Button btnDelete;

    public void InitialiseWithData(SaveDataInfo saveData, SaveDataManager saveDataManager)
    {
        this.saveData = saveData;
        this.saveDataManager = saveDataManager;

        Location location = Locations.FindByCoordinates((int)saveData.mapPositionX, (int)saveData.mapPositionY, false);
        string locationName;

        if (location != null)
        {
            locationName = location.name;
        }
        else
        {
            locationName = "Wilderness";
        }

        displayInfo.text = $"<color=#{saveData.playerNameColour}><b>{saveData.playerName}</b></color>\nDays passed: {saveData.totalGameDays}\nLocation: {locationName}";

        if (TransientDataScript.GameState == GameState.MainMenu)
        {
            btnSave.gameObject.SetActive(false);
        }
        else
        {
            btnSave.gameObject.SetActive(true);
            btnSave.onClick.AddListener(() => ButtonSave());
        }

        btnDelete.onClick.AddListener(() => ButtonDelete());
        btnLoad.onClick.AddListener(() => ButtonLoad());
    }

    public void InitialiseEmpty(SaveDataManager saveDataManager)
    {
        this.saveDataManager = saveDataManager;

        displayInfo.text = $"<b>Free Slot</b>";
        btnLoad.gameObject.SetActive(false);
        btnDelete.gameObject.SetActive(false);

        if (TransientDataScript.GameState == GameState.MainMenu)
        {
            btnSave.gameObject.SetActive(false);
        }
        else
        {
            btnSave.gameObject.SetActive(true);
            btnSave.onClick.AddListener(() => ButtonSave());
        }

        saveData = null;
    }

    public void ButtonLoad()
    {
        if (saveData.versionIncompatible)
        {
            displayInfo.text = $"This save data is incompatible with the application version. Please start a new game.";
            btnLoad.gameObject.SetActive(false);
        }
        else if (saveData.versionMismatch)
        {
            displayInfo.text = $"This save data does not match the application version. Strangeness may occur. Click again to load anyway.";
            btnLoad.onClick.RemoveAllListeners();
            btnLoad.onClick.AddListener(() => ExecuteLoad());
        }
        else
        {
            ExecuteLoad();
        }
    }

    public void ExecuteLoad()
    {
        saveDataManager.LoadGameAndPlay(saveData);
    }

    public void ButtonDelete()
    {
        displayInfo.text = "Click again to confirm delete.";
        btnLoad.gameObject.SetActive(false);
        btnSave.gameObject.SetActive(false);
        btnDelete.onClick.RemoveAllListeners();
        btnDelete.onClick.AddListener(() => ExecuteDelete());
    }

    public void ExecuteDelete()
    {
        saveDataManager.DeleteFileAndRefresh(saveData);
        InitialiseEmpty(saveDataManager);
    }

    public void ButtonSave()
    {
        if (saveData == null)
        {
            Debug.Log("Attempting to save game with no saveData.");
        }
        else
        {
            Debug.Log("Attempting to save game using " + saveData.fileName);
        }

        saveDataManager.SaveGameAndPlay(saveData);
    }
}
