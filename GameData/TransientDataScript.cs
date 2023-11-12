using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TransientDataScript : MonoBehaviour
{
    public GameState gameState;
    public Language language;
    public List<MotherObject> objectIndex;
    public List<Item> itemCodex;
    public List<GameObject> activePrefabs;

    [TextArea(20, 100)]
    public string gameStateLog = "Game State Changes";

    //CAMERA
    public CameraView cameraView;

    //UI FUNCTIONS
    public PushAlertManager pushAlertManager;
    public MenuFloatTextScript floatText;

    //COACH DATA
    public float currentSpeed;
    public float currentMana;
    public float manapool;
    public float engineBoost;
    public EngineState engineState;

    //PLAYER LOCATION DATA
    public Region currentRegion;
    public Location currentLocation;

    //TIME DATA
    public int timeFlowSpeed = 1;
    public DayOfWeek weekDay;
    public int date;
    public int month;
    public int year;

    //UI CONTENT
    public string mouseToolTip;
    public string infoBox;
    public Shop currentShop;


    //*** PUBLIC METHODS ***
    //UI TEXT
    //Eventually these methods should take text IDs that refer to a .json file
    public void PushAlert(string alert)
    {
        pushAlertManager.QueueAlert(alert);
    }
    public void PrintFloatText(string content)
    {
        if (gameState != GameState.MainMenu && gameState != GameState.Loading && gameState != GameState.Dialogue)
        {
            floatText.PrintFloatText(content);
        }
        else
            DisableFloatText();
    }
    public void DisableFloatText()
    {
        floatText.DisableFloatText();
    }

    //SYSTEM METHODS
    public void PurgePrefabs()
    {
        foreach (GameObject prefab in activePrefabs)
        {
            if (prefab != null)
            {
                Destroy(prefab);
            }
        }
    }
    public void ReturnToOverWorld()
    {
        ChangeGameState(name, gameObject, GameState.Overworld);
        //gameState = GameState.Overworld;
        //Debug.Log(name + " changed GameState to " + GameState.Overworld);
    }

    public void ChangeGameState(string callerScript, GameObject callerObject, GameState newState)
    {
        gameStateLog += "\n" + Time.realtimeSinceStartup + ": " + callerScript + "(script) on " + callerObject.name + "(game object) changed the game state from " + gameState + " to " + newState + ".";
        gameState = newState;
    }

    //ITEM METHODS
    public Item GetItemByID(string searchID)
    {
        foreach (Item item in itemCodex)
        {
            if (item.objectID.Contains(searchID))
            {
                return item;
            }
        }

        Debug.Log("No item ID found containing " + searchID);
        return new Item();
    }
}
