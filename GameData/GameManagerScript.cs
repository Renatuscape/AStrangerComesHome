using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //ALL GAME COMPONENTS
    public DataManagerScript dataManager;

    public GameObject itemManager;
    public GameObject skillManager;
    public GameObject upgradeManager;
    public GameObject characterManager;
    public GameObject questManager;
    public GameObject dialogueManager;
    public GameObject locationManager;
    public GameObject regionManager;
    public GameObject recipeManager;

    public GameObject gameComponentMaster;
    public GameObject mainMenuComponent;
    public GameObject characterCreatorComponent;
    public GameObject parallaxManagerComponent;
    public GameObject coachComponent;
    public GameObject salvageSpawnerComponent;
    public GameObject menuUIManagerComponent;
    public GameObject infoUIManagerComponent;
    public GameObject timeManagerComponent;

    public List<GameObject> listOfGameComponents;

    public GameObject gameComponentParent;

    public AutoMap mapComponent;
    public CameraController cameraComponent;
    public FontManager fontManager;
    public PortraitRenderer portraitRenderer;
    public StorySystem storySystem;
    public MenuSystem menuSystem;

    void Awake()
    {
        //Application.targetFrameRate = 60;

        TransientDataScript.SetGameState(GameState.Loading, name, gameObject);
        StartUpRoutine();
    }

    void StartUpRoutine()
    {

        InitiateJsonManagers();

        gameComponentParent.SetActive(true); //if the game starts with this disabled, you can assure loading objects first

        foreach (Transform child in gameComponentParent.transform)
        {
            listOfGameComponents.Add(child.gameObject);
        }

        if (mainMenuComponent != null) //check if the main menu exists
        {
            ActivateMainMenu();
        }
        else //If there is no main menu, run new game routine directly
        {
            NewGameRoutine();
        }

        Invoke("CreateGameController", 1f);
    }

    void InitiateJsonManagers()
    {
        regionManager.SetActive(true);
        locationManager.SetActive(true);
        itemManager.SetActive(true);
        skillManager.SetActive(true);
        upgradeManager.SetActive(true);
        characterManager.SetActive(true);
        dialogueManager.SetActive(true);
        questManager.SetActive(true);
        recipeManager.SetActive(true);
    }

    public void CreateGameController()
    {
        GlobalSettingsManager.LoadSettings();
        fontManager.enabled = true;
        fontManager.LoadFonts();
        fontManager.enabled = false;
        portraitRenderer.gameObject.SetActive(true);
        portraitRenderer.gameObject.SetActive(false);
        GameObject Controller = new();
        Controller.name = "Controller";
        Controller.AddComponent<GameController>();
        Controller.GetComponent<GameController>().gm = this;
    }
    public void ActivateMainMenu()
    {
        foreach (GameObject component in listOfGameComponents)
        {
            component.SetActive(false);
        }

        TransientDataScript.SetGameState(GameState.MainMenu, name, gameObject);
        menuUIManagerComponent.SetActive(true);
        mainMenuComponent.SetActive(true);
    }

    public void NewGameRoutine()
    {
        dataManager.playerName = "Morgan";
        dataManager.playerNameColour = "597266";
        dataManager.eyesHexColour = "87DF5C";
        dataManager.hairHexColour = "896C5C";

        ResetGameComponents();

        dataManager.playerGold = 0;
        dataManager.totalGameDays = 0;
        dataManager.timeOfDay = 0.3f;
        dataManager.currentRegion = "REGION1";
        dataManager.mapPositionX = 0f;
        dataManager.mapPositionY = 0f;
        dataManager.passengerIsActiveA = false;
        dataManager.passengerIsActiveB = false;
        dataManager.planterIsActiveA = false;
        dataManager.planterIsActiveB = false;
        dataManager.planterIsActiveC = false;
        dataManager.isSynthActiveA = false;
        dataManager.isSynthActiveB = false;
        dataManager.isSynthActiveC = false;

        Player.inventoryList = new();
        dataManager.inventoryList = new();
        Debug.Log($"Player inventory = new(). Count is {Player.inventoryList.Count}. DataManager inventory = {dataManager.inventoryList.Count}");
        dataManager.inventoryList = Player.inventoryList;


        //Add all the skills to the player inventory from the start
        foreach (Skill skill in Skills.all.Where(s => s.type == SkillType.Attribute))
        {
            Player.inventoryList.Add(new IdIntPair { objectID = skill.objectID, amount = 1 });
        }

        TransientDataScript.SetGameState(GameState.CharacterCreation, name, gameObject);
        characterCreatorComponent.SetActive(true);
        InitialiseMap();
    }

    public void LoadRoutine()
    {
        ResetGameComponents();
        DialogueTagParser.UpdateTags(dataManager);
        InitialiseMap();
    }

    public void ResetGameComponents()
    {
        foreach (GameObject component in listOfGameComponents)
        {
            component.SetActive(false);
        }

        foreach (GameObject component in listOfGameComponents)
        {
            component.SetActive(true);
        }
    }

    public void InitialiseMap()
    {
        Debug.Log("Initialising map");
        if (TransientDataScript.GameState != GameState.CharacterCreation)
        {
            TransientDataScript.SetGameState(GameState.Loading, this.name, gameObject);
        }

        if (string.IsNullOrWhiteSpace(dataManager.currentRegion))
        {
            Debug.Log($"Region was null or whitespace: {dataManager.currentRegion}. Setting to REGION1 and default coordinates.");
            dataManager.currentRegion = "REGION1";
            dataManager.mapPositionX = 0;
            dataManager.mapPositionY = 0;
        }

        mapComponent.mapScroller = new(mapComponent);
        mapComponent.mapBuilder = new(mapComponent, mapComponent.mapContainer);

        Region region = Regions.FindByID(dataManager.currentRegion);
        //Debug.Log($"Loading {dataManager.currentRegion}. Found {region.objectID}");

        if (region is not null)
        {
            mapComponent.ChangeMap(region, dataManager.mapPositionX, dataManager.mapPositionY);
        }

        else
        {
            Debug.Log($"Region by the ID {dataManager.currentRegion} not found.");
        }


        if (TransientDataScript.GameState != GameState.CharacterCreation)
        {
            TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
        }
    }
}

public static class ListExtenstions
{
    public static void AddMany<T>(this List<T> list, params T[] elements)
    {
        list.AddRange(elements);
    }
}

[Serializable]
public class ItemIntPair
{
    public Item item;
    public int amount;
}