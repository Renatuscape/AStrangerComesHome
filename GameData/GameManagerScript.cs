using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public GlobalTimerUpdater globalTimer;

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
    public Engine coachEngine;
    public ManaConverter manaConverter;
    public PassengerManager passengerManager;
    public SaveDataManager saveDataManager;

    void Awake()
    {
        Application.targetFrameRate = 60;

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
        setUpReady = true;
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

        await recipeManager.StartLoading();
        Debug.Log("STARTUP: Loading recipes async completed");

        await bookManager.StartLoading();
        Debug.Log("STARTUP: Loading books async completed");

        await dialogueManager.StartLoading();
        Debug.Log("STARTUP: Loading dialogue async completed");

        await questManager.StartLoading();
        Debug.Log("STARTUP: Loading quests async completed");

        await GuildRewardLoader.StartLoading(menuSystem.guildMenu.rewardsMenu);
        Debug.Log("STARTUP: Loading guild rewards async completed");

        //await saveDataManager.StartLoading();
        //Debug.Log("STARTUP: Loading save data completed");

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
        StartCoroutine(SaveCoroutine());
    }

    private IEnumerator SaveCoroutine()
    {
        dataManager.version = Application.version;
        dataManager.lastVersionSaved = "none";
        dataManager.saveID = Random.Range(100000, 999999).ToString();
        dataManager.saveSlot = -1;
        dataManager.playerName = "Morgan";
        dataManager.playerNameColour = "597266";
        dataManager.playerGender = "Male";
        dataManager.playerSprite.ResetValues();
        portraitRenderer.UpdatePlayerSprite();

        yield return StartCoroutine(ResetGameComponentsCoroutine());

        dataManager.totalGameDays = 0;
        dataManager.timeOfDay = 0.3f;
        dataManager.currentRegion = "REGION1";
        dataManager.mapPositionX = 0f;
        dataManager.mapPositionY = 0f;
        dataManager.seatA = new();
        dataManager.seatB = new();
        dataManager.postLocationID = "R0-LOCC0-CITY";

        Player.inventoryList.Clear();
        dataManager.questProgression.Clear();
        dataManager.inventoryList.Clear();
        dataManager.planters.Clear();
        dataManager.alchemySynthesisers.Clear();
        dataManager.unlockedNames.Clear();
        dataManager.giftedThisWeek.Clear();
        dataManager.claimedLoot.Clear();

        SetUpUpgradeWear();
        dataManager.inventoryList = Player.inventoryList;
        dataManager.questProgression = Player.questProgression;

        //Add skills to the player inventory from the start
        Player.Add(StaticTags.Wandering, 10, true); // Wandering
        Player.Add(StaticTags.Fate, 8, true); // Fate

        TransientDataScript.SetGameState(GameState.CharacterCreation, name, gameObject);
        characterCreatorComponent.SetActive(true);
        InitialiseMap();

        questTracker.StartTracking();
        alchemyTracker.StartTracking();
        passengerManager.Initialise();
    }

    void SetUpUpgradeWear()
    {
        TransientDataScript.gameManager.dataManager.upgradeWear = new();

        foreach (var up in Upgrades.all)
        {
            TransientDataScript.gameManager.dataManager.upgradeWear.Add(new IdIntPair() { objectID = up.objectID, amount = 0 });
            Player.upgradeWear = TransientDataScript.gameManager.dataManager.upgradeWear;
        }
    }

    public void LoadRoutine()
    {
        StartCoroutine(LoadCoroutine());
    }

    private IEnumerator LoadCoroutine()
    {
        TransientDataScript.SetGameState(GameState.Loading, name, gameObject);
        yield return StartCoroutine(ResetGameComponentsCoroutine());
        DialogueTagParser.UpdateTags(dataManager);

        if (Player.upgradeWear == null || Player.upgradeWear.Count < 1)
        {
            SetUpUpgradeWear();
        }

        InitialiseMap();

        questTracker.StartTracking();
        alchemyTracker.StartTracking();
        passengerManager.Initialise();
        portraitRenderer.UpdatePlayerSprite();

        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }

    private IEnumerator ResetGameComponentsCoroutine()
    {
        loadingCanvas.gameObject.SetActive(true);
        TransientDataScript.isDemoEnabled = false;

        CharacterNodeTracker.ClearCharacterNodes();
        foreach (GameObject component in listOfGameComponents)
        {
            component.SetActive(false);
            yield return null;
            component.SetActive(true);
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