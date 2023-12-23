using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //ALL GAME COMPONENTS
    public DataManagerScript dataManager;

    public GameObject itemManager;
    public GameObject skillManager;
    public GameObject upgradeManager;
    public GameObject characterManager;
    public GameObject dialogueManager;
    public GameObject questManager;

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
            SetUpNewGame();
        }
    }

    void InitiateJsonManagers()
    {
        itemManager.SetActive(true);
        skillManager.SetActive(true);
        upgradeManager.SetActive(true);
        characterManager.SetActive(true);
        dialogueManager.SetActive(true);
        questManager.SetActive(true);
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
        dataManager.playerNameColour = "597266";
        dataManager.eyesHexColour = "87DF5C";
        dataManager.hairHexColour = "896C5C";

        ResetGameComponents();

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

        Player.inventoryList = new();
        dataManager.inventoryList = Player.inventoryList;

        //Add all the skills to the player inventory from the start
        foreach (Skill skill in Skills.all.Where(s => s.type == SkillType.Attribute))
        {
            Player.inventoryList.Add(new IdIntPair { objectID = skill.objectID, amount = 1 });
        }

        TransientDataScript.SetGameState(GameState.CharacterCreation, name, gameObject);
        characterCreatorComponent.SetActive(true);
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
}

public static class ListExtenstions
{
    public static void AddMany<T>(this List<T> list, params T[] elements)
    {
        list.AddRange(elements);
    }
}
