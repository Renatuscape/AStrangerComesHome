using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    //ALL GAME COMPONENTS
    public TransientDataScript transientData;
    public DataManagerScript dataManager;

    public ItemManager itemManager;
    public SkillManager skillManager;
    public UpgradeManager upgradeManager;

    public GameObject gameComponentMaster;
    public GameObject mainMenuComponent;
    public GameObject characterCreatorComponent;
    public GameObject mapManagerComponent;
    public GameObject parallaxManagerComponent;
    public GameObject coachComponent;
    public GameObject salvageSpawnerComponent;
    public GameObject menuUIManagerComponent;
    public GameObject infoUIManagerComponent;
    public GameObject shopUIComponent;
    public GameObject timeManagerComponent;

    public List<GameObject> listOfGameComponents;

    public GameObject gameComponentParent;


    void Awake()
    {
        //Application.targetFrameRate = 60;

        TransientDataScript.SetGameState(GameState.Loading, name, gameObject);
        StartUpRoutine();
    }

    void StartUpRoutine()
    {
        ValidateJsonLoads();
        LoadScriptableObjects();

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
            SetUpNewGame();
        }
    }

    void ValidateJsonLoads()
    {

    }
    void LoadScriptableObjects()
    {

        string[] subfolders = new string[] { "Quests/", "Characters/" };

        List<MotherObject> allObjects = new List<MotherObject>();

        foreach (string subfolder in subfolders)
        {
            MotherObject[] objects = Resources.LoadAll<MotherObject>(subfolder);
            allObjects.AddRange(objects);
        }

        transientData.objectIndex.Clear();
        transientData.objectIndex.AddRange(allObjects);

        foreach (Character character in Characters.all)
        {
            character.NameSetup();
            Debug.Log("Move character name setup away from game manager.");
        }
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

    public void SetUpNewGame()
    {
        dataManager.playerName = "Morgan";
        dataManager.playerNameColour = "346159";
        dataManager.eyesHexColour = "FFFFFF";
        dataManager.hairHexColour = "FFFFFF";

        LoadGameComponents();

        dataManager.playerGold = 0;
        dataManager.totalGameDays = 0;
        dataManager.timeOfDay = 0;
        dataManager.mapPositionX = 0.5f;
        dataManager.mapPositionY = 0.5f;
        dataManager.passengerIsActiveA = false;
        dataManager.passengerIsActiveB = false;
        dataManager.planterIsActiveA = false;
        dataManager.planterIsActiveB = false;
        dataManager.planterIsActiveC = false;
        dataManager.isSynthActiveA = false;
        dataManager.isSynthActiveB = false;
        dataManager.isSynthActiveC = false;

        //Empty Player items, skills and upgrades
        Player.items = new();
        dataManager.playerItems = Player.items;
        Player.skills = new();
        dataManager.playerSkills = Player.skills;
        Player.upgrades = new();
        dataManager.playerUpgrades = Player.upgrades;


        TransientDataScript.SetGameState(GameState.CharacterCreation, name, gameObject);
        characterCreatorComponent.SetActive(true);
    }

    public void LoadGameComponents()
    {
        LoadScriptableObjects();

        foreach (GameObject component in listOfGameComponents)
        {
            component.SetActive(false);
        }

        foreach (GameObject component in listOfGameComponents)
        {
            component.SetActive(true);
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
