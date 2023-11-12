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
    public GameObject itemManager;

    public List<GameObject> listOfGameComponents;

    public GameObject gameComponentParent;


    void Awake()
    {
        Application.targetFrameRate = 60;

        transientData.ChangeGameState(name, gameObject, GameState.Loading);
        //transientData.gameState = GameState.Loading;
        //Debug.Log(name + " changed GameState to " + GameState.Loading);
        StartUpRoutine();
    }

    void StartUpRoutine()
    {
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

    void LoadScriptableObjects()
    {

        string[] subfolders = new string[] { "Items/", "Upgrades/", "Skills/", "Quests/", "Characters/" };

        List<MotherObject> allObjects = new List<MotherObject>();

        foreach (string subfolder in subfolders)
        {
            MotherObject[] objects = Resources.LoadAll<MotherObject>(subfolder);
            allObjects.AddRange(objects);
        }

        transientData.objectIndex.Clear();
        transientData.objectIndex.AddRange(allObjects);

        foreach (MotherObject obj in allObjects)
        {
            if (obj is Character)
            {
                Character character = (Character)obj;
                character.NameSetup();
            }

            if (!dataManager.playerItems.ContainsKey(obj.name))
            {
                dataManager.playerItems.Add(obj.name, obj.dataValue);
            }
        }
    }

    public void ActivateMainMenu()
    {
        foreach (GameObject component in listOfGameComponents)
        {
            component.SetActive(false);
        }

        transientData.ChangeGameState(name, gameObject, GameState.MainMenu);
        //transientData.gameState = GameState.MainMenu;
        //Debug.Log(name + " changed GameState to " + GameState.MainMenu);
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


        transientData.ChangeGameState(name, gameObject, GameState.CharacterCreation);
        //transientData.gameState = GameState.NewGameMenu;
        characterCreatorComponent.SetActive(true);
        //Debug.Log(name + " changed GameState to " + GameState.NewGameMenu);
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
