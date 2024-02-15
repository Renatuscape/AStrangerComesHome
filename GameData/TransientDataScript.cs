using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TransientDataScript : MonoBehaviour
{
    public static GameState GameState { get; private set; }

    public Language language;
    public List<GameObject> activePrefabs;

    [TextArea(20, 50)]
    public string gameStateLog = "Game State Changes";

    //CAMERA
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
    public Location currentLocation = null;

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

    //*** CONTROLLER LISTENER ***



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

    public void SpawnLocation(Location location)
    {
        Debug.Log($"Ready to spawn {location.name} {location.objectID} when logic is implemented.");
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
        GameObject.Find("TransientData").GetComponent<TransientDataScript>().DisableFloatText();
    }
    public static void ReturnToOverWorld(string name, GameObject gameObject)
    {
        SetGameState(GameState.Overworld, name, gameObject);
    }

    public static DayOfWeek GetWeekDay()
    {
        return GameObject.Find("TransientData").GetComponent<TransientDataScript>().weekDay;
    }

    public static void TravelByGate(Gate gate)
    {
        Debug.Log("Travel by Gate called at TransientDataScript");
        GameObject.Find("GameManager").GetComponent<GameManagerScript>().mapComponent.TravelByGate(gate);
        AudioManager.SkipNow();
    }

    public static bool IsTimeFlowing() //in this state, mana regenerates and plants grow. Controllers are enabled
    {
        if (GameState == GameState.Overworld
            || GameState == GameState.JournalMenu
            || GameState == GameState.BankMenu
            || GameState == GameState.ShopMenu
            || GameState == GameState.Dialogue
            || GameState == GameState.StartMenu)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static void LogStateChange(string callerScript, GameObject callerObject, GameState newState)
    {
        GameObject.Find("TransientData").GetComponent<TransientDataScript>().gameStateLog += "\n" + Time.realtimeSinceStartup + ": " + callerScript + "(script) on " + callerObject.name + "(game object) changed the game state from " + GameState + " to " + newState + ".";
    }
}


public static class MouseTracker
{
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));

        return worldPosition;
    }
}
