using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //ALL GAME COMPONENTS
    public DataManagerScript dataManager;
    public Canvas loadingCanvas;

    public ItemManager itemManager;
    public SkillManager skillManager;
    public UpgradeManager upgradeManager;
    public CharacterManager characterManager;
    public QuestManager questManager;
    public DialogueManager dialogueManager;
    public LocationManager locationManager;
    public RegionManager regionManager;
    public RecipeManager recipeManager;
    public BookManager bookManager;

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
    public AlchemyTracker alchemyTracker;
    public QuestTracker questTracker;
    public SpriteFactory spriteFactory;

    void Awake()
    {
        Application.targetFrameRate = 120;

        TransientDataScript.SetGameState(GameState.Loading, name, gameObject);
        StartUpRoutine();
    }

    async void StartUpRoutine()
    {
        loadingCanvas.gameObject.SetActive(true);

        GlobalSettingsManager.LoadSettings();

        await InitiateJsonManagers(); // Ensure all json files are loaded before doing anything else

        gameComponentParent.SetActive(true); // Enable game components

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

        CreateGameController();

        loadingCanvas.gameObject.SetActive(false);
    }

    async Task InitiateJsonManagers()
    {
        await spriteFactory.WaitForBuildCompletionAsync();
        Debug.Log("Sprite Factory finished building.");

        regionManager.StartLoading();
        locationManager.StartLoading();
        upgradeManager.StartLoading();

        await itemManager.StartLoading();
        Debug.Log("STARTUP: Loading items async completed");

        await skillManager.StartLoading();
        Debug.Log("STARTUP: Loading skills async completed");

        await characterManager.StartLoading();
        Debug.Log("STARTUP: Loading characters async completed");

        await dialogueManager.StartLoading();
        Debug.Log("STARTUP: Loading dialogue async completed");

        await recipeManager.StartLoading();
        Debug.Log("STARTUP: Loading recipes async completed");

        await bookManager.StartLoading();
        Debug.Log("STARTUP: Loading books async completed");

        await questManager.StartLoading();
        Debug.Log("STARTUP: Loading quests async completed");

        DialogueTagParser.UpdateTags(dataManager);
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
        questTracker.StopTracking();

        foreach (GameObject component in listOfGameComponents)
        {
            component.SetActive(false);
        }

        TransientDataScript.SetGameState(GameState.MainMenu, name, gameObject);
        TransientDataScript.transientData.currentMana = 25;
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
        dataManager.alchemySynthesisers = new();
        dataManager.unlockedNames = new();
        dataManager.giftedThisWeek = new();

        Player.inventoryList = new();
        dataManager.inventoryList = new();
        Debug.Log($"Player inventory = new(). Count is {Player.inventoryList.Count}. DataManager inventory = {dataManager.inventoryList.Count}");
        dataManager.inventoryList = Player.inventoryList;


        //Add skills to the player inventory from the start
        Player.Add("ATT000", 10, true); // Wandering
        Player.Add("ATT001", 8, true); // Fate

        TransientDataScript.SetGameState(GameState.CharacterCreation, name, gameObject);
        characterCreatorComponent.SetActive(true);
        InitialiseMap();

        questTracker.StartTracking();
        alchemyTracker.StartTracking();
    }

    public void LoadRoutine()
    {
        ResetGameComponents();
        DialogueTagParser.UpdateTags(dataManager);
        InitialiseMap();

        questTracker.StartTracking();
        alchemyTracker.StartTracking();
    }

    public void ResetGameComponents()
    {
        CharacterNodeTracker.ClearCharacterNodes();
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