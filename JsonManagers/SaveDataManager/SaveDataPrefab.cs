using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDataPrefab : MonoBehaviour
{
    public int saveSlot;
    public SaveDataInfo saveData;
    public SaveDataManager saveDataManager;
    public TextMeshProUGUI displayInfo;
    public Button btnSave;
    public Button btnLoad;
    public Button btnDelete;

    public void InitialiseWithData(int saveSlot, SaveDataInfo saveData, SaveDataManager saveDataManager)
    {
        this.saveSlot = saveSlot;
        this.saveData = saveData;
        this.saveDataManager = saveDataManager;

        Location location = Locations.FindByCoordinates(saveData.currentRegion, (int)saveData.mapPositionX, (int)saveData.mapPositionY, false);
        string locationName;

        if (location != null)
        {
            locationName = location.name;
        }
        else
        {
            locationName = Regions.FindByID(saveData.currentRegion).name;
        }

        displayInfo.text = $"<color=#{saveData.playerNameColour}><b>{saveData.playerName}</b></color>" +
            $"\n{(saveData.lastSaveTime != null ? saveData.lastSaveTime : "Unknown")}" +
            $"\nDays passed: {saveData.totalGameDays}" +
            $"\nLocation: {locationName}";

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

    public void InitialiseEmpty(int saveSlot, SaveDataManager saveDataManager)
    {
        this.saveDataManager = saveDataManager;
        this.saveSlot = saveSlot;

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
        InitialiseEmpty(saveSlot, saveDataManager);
    }

    public void ButtonSave()
    {
        if (saveData == null)
        {
            Debug.Log("Attempting to save game into empty slot.");
        }
        else
        {
            Debug.Log("Attempting to save game using " + saveData.fileName);
        }

        TransientDataScript.gameManager.dataManager.saveSlot = saveSlot;
        saveDataManager.AttemptSave(saveData, this, false);
    }

    public void ConfirmOverwrite()
    {
        displayInfo.text = $"A different game has been saved in this slot, or name has been changed. Click again to confirm overwrite.";
        btnSave.onClick.RemoveAllListeners();
        btnSave.onClick.AddListener(() => ExecuteOverwrite());
    }

    public void ExecuteOverwrite()
    {
        saveDataManager.AttemptSave(saveData, this, true);
    }
}
