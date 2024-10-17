using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class GameManagerScript : MonoBehaviour
{
    public static bool setUpReady = false;

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
    public MemoryManager memoryManager;
    public GlobalTimerUpdater globalTimer;

    public GameObject gameComponentMaster;
    public GameObject mainMenuComponent;
    public GameObject characterCreatorComponent;
    public GameObject coachComponent;
    public GameObject salvageSpawnerComponent;
    public GameObject menuUIManagerComponent;
    public GameObject timeManagerComponent;
    public GameObject uiCanvasContainer;

    public GameObject gameComponentParent;
    List<GameObject> listOfGameComponents = new();

    public AutoMap mapComponent;
    public CameraController cameraComponent;
    public FontManager fontManager;
    public PortraitRenderer portraitRenderer;
    public StorySystem storySystem;
    public MenuSystem menuSystem;
    public AlchemyTracker alchemyTracker;
    public QuestTracker questTracker;
    public SpriteFactory spriteFactory;
    public Engine coachEngine;
    public ManaConverter manaConverter;
    public PassengerManager passengerManager;
    public StationManager stationManager;
    public SaveDataManager saveDataManager;
    public BankManager bankManager;
    public GardenCoachPlanters coachPlanters;

    void Awake()
    {
        Application.targetFrameRate = 60;

        TransientDataScript.SetGameState(GameState.Loading, name, gameObject);
        StartUpRoutine();
    }

    async void StartUpRoutine()
    {
        loadingCanvas.gameObject.SetActive(true);
        Debug.Log("RUNNING STARTUP ROUTINE");

        GlobalSettings.LoadSettings();

        await InitiateJsonManagers(); // Ensure all json files are loaded before doing anything else

        gameComponentParent.SetActive(true); // Enable game components
        foreach (Transform child in gameComponentParent.transform)
        {
            listOfGameComponents.Add(child.gameObject);
        }

        uiCanvasContainer.SetActive(true);
        foreach (Transform transform in uiCanvasContainer.transform)
        {
            transform.gameObject.SetActive(false);
            transform.gameObject.SetActive(true);
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
        setUpReady = true;
        Debug.Log("COMPLETED STARTUP ROUTINE");
    }

    async Task InitiateJsonManagers()
    {
        Debug.Log("INITIATING JSON MANAGERS");
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

        await recipeManager.StartLoading();
        Debug.Log("STARTUP: Loading recipes async completed");

        await bookManager.StartLoading();
        Debug.Log("STARTUP: Loading books async completed");

        await dialogueManager.StartLoading();
        Debug.Log("STARTUP: Loading dialogue async completed");

        await questManager.StartLoading();
        Debug.Log("STARTUP: Loading quests async completed");

        await memoryManager.StartLoading();
        Debug.Log("STARTUP: Loading memories async completed");

        await GuildRewardLoader.StartLoading(menuSystem.guildMenu);
        Debug.Log("STARTUP: Loading guild rewards async completed");

        DialogueTagParser.UpdateTags(dataManager);
    }

    public void CreateGameController()
    {
        GlobalSettings.LoadSettings();
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
        StartCoroutine(NewGameSetup());
    }

    private IEnumerator NewGameSetup()
    {
        dataManager.Reset();
        portraitRenderer.UpdatePlayerSprite();
        coachPlanters.Setup();

        yield return StartCoroutine(ResetGameComponentsCoroutine());

        // Add skills to the player inventory from the start
        Player.Add(StaticTags.Wandering, 10, true); // Wandering
        Player.Add(StaticTags.Fate, 8, true); // Fate

        // Add initial debt
        Player.Add(StaticTags.CurrentDebt, 5000, true);

        TransientDataScript.SetGameState(GameState.CharacterCreation, name, gameObject);
        characterCreatorComponent.SetActive(true);
        InitialiseMap();

        questTracker.StartTracking();
        alchemyTracker.StartTracking();
        passengerManager.Initialise();

        PassengerSatisfaction.CheckUnlockState();
        UpgradeWearTracker.CheckForBrokenCondition();
        mapComponent.PlaceMarker(new Vector3(-100, -100));
    }

    public void LoadRoutine()
    {
        StartCoroutine(LoadCoroutine());
    }

    private IEnumerator LoadCoroutine()
    {
        TransientDataScript.ForceClearWorldSpawns();
        TransientDataScript.SetGameState(GameState.Loading, name, gameObject);
        yield return StartCoroutine(ResetGameComponentsCoroutine());
        DialogueTagParser.UpdateTags(dataManager);

        InitialiseMap();

        coachPlanters.Setup();
        questTracker.StartTracking();
        alchemyTracker.StartTracking();
        passengerManager.Initialise();
        portraitRenderer.UpdatePlayerSprite();

        Character player = Characters.FindByTag("Traveller", gameObject.name);
        player.trueName = dataManager.playerName;
        player.hexColour = dataManager.playerNameColour;
        player.NameSetup();
        DialogueTagParser.UpdateTags(dataManager);

        foreach (var planter in dataManager.planters)
        {
            planter.weeds = 0;
        }

        PassengerSatisfaction.CheckUnlockState();
        UpgradeWearTracker.CheckForBrokenCondition();

        mapComponent.PlaceMarker(new Vector3(dataManager.mapPositionX, dataManager.mapPositionY));
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }

    private IEnumerator ResetGameComponentsCoroutine()
    {
        loadingCanvas.gameObject.SetActive(true);
        TransientDataScript.isDemoEnabled = false;

        WorldNodeTracker.ClearCharacterNodes();
        foreach (GameObject component in listOfGameComponents)
        {
            component.SetActive(false);
            yield return null;
            component.SetActive(true);
            yield return null;
        }

        uiCanvasContainer.SetActive(true);

        foreach (Transform transform in uiCanvasContainer.transform)
        {
            transform.gameObject.SetActive(false);
            yield return null;
            transform.gameObject.SetActive(true);
            yield return null;
        }

        foreach (Upgrade upgrade in Upgrades.all)
        {
            upgrade.isBroken = false;
        }

        coachEngine.Enable();
        manaConverter.Enable();
        globalTimer.Initialise();

        yield return new WaitForSeconds(0.2f);
        loadingCanvas.gameObject.SetActive(false);
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