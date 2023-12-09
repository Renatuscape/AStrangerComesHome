using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TransientDataScript : MonoBehaviour
{
    //public GameState gameState;
    public static GameState GameState { get; private set; }

    public Language language;
    //public List<MotherObject> objectIndex;
    //public List<Item> itemCodex;
    public List<GameObject> activePrefabs;

    [TextArea(20, 100)]
    public string gameStateLog = "Game State Changes";

    //CAMERA
    //public CameraView cameraView;
    public static CameraView CameraView { get; private set; }

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
        if (GameState != GameState.MainMenu && GameState != GameState.Loading && GameState != GameState.Dialogue)
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
    //NEW STATIC METHODS
    public static GameState GetGameState()
    {
        return GameState;
    }
    public static void SetCameraView(CameraView view)
    {
        CameraView = view;
    }

    public static void SetGameState(GameState newState, string callerScript, GameObject callerObject)
    {
        LogStateChange(callerScript, callerObject, newState);
        GameState = newState;
    }
    public static void ReturnToOverWorld(string name, GameObject gameObject)
    {
        SetGameState(GameState.Overworld, name, gameObject);
    }
    static void LogStateChange(string callerScript, GameObject callerObject, GameState newState)
    {
        GameObject.Find("TransientData").GetComponent<TransientDataScript>().gameStateLog += "\n" + Time.realtimeSinceStartup + ": " + callerScript + "(script) on " + callerObject.name + "(game object) changed the game state from " + GameState + " to " + newState + ".";
    }

}
