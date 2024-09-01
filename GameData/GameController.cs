using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameManagerScript gm;
    private int screenSetting = 2;

    private readonly Dictionary<KeyCode, Action> keyBindings = new Dictionary<KeyCode, Action>();

    private void Start()
    {
        InitializeKeyBindings();
    }

    private void Update()
    {
        var gameState = TransientDataScript.GameState;

        if (gameState == GameState.Loading ||
            gameState == GameState.StartMenu ||
            gameState == GameState.MainMenu ||
            gameState == GameState.ShopMenu ||
            gameState == GameState.CharacterCreation ||
            gameState == GameState.Dialogue)
        {
            return;
        }

        ProcessInput();
    }

    private void InitializeKeyBindings()
    {
        keyBindings[KeyCode.Space] = ToggleCoachCamera;
        keyBindings[KeyCode.Escape] = HandleEscapeKey;
        keyBindings[KeyCode.Return] = ToggleEngineState;
        keyBindings[KeyCode.Alpha1] = () => ToggleEngineState(EngineState.FirstGear);
        keyBindings[KeyCode.Alpha2] = () => ToggleEngineState(EngineState.SecondGear);
        keyBindings[KeyCode.Alpha3] = () => ToggleEngineState(EngineState.ThirdGear);
        keyBindings[KeyCode.Backspace] = () => ToggleEngineState(EngineState.Reverse);
        keyBindings[KeyCode.E] = () => ToggleEngineState(EngineState.FirstGear);
        keyBindings[KeyCode.R] = () => ToggleEngineState(EngineState.Reverse);
        keyBindings[KeyCode.M] = () => ToggleMap(TransientDataScript.GameState != GameState.MapMenu);
        keyBindings[KeyCode.Tab] = ToggleJournalMenu;
        keyBindings[KeyCode.J] = ToggleJournalMenu;
        keyBindings[KeyCode.I] = ToggleJournalMenu; //() => ToggleJournalMenu(JournalMainPage.Inventory);
        keyBindings[KeyCode.Q] = ToggleJournalMenu; //() => ToggleJournalMenu(JournalMainPage.Quests);
    }

    private void ProcessInput()
    {
        foreach (var keyBinding in keyBindings)
        {
            if (Input.GetKeyDown(keyBinding.Key))
            {
                keyBinding.Value.Invoke();
                break;
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeResolution(1);
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeResolution(-1);
        }
    }

    private void ToggleCoachCamera()
    {
        if (TransientDataScript.GameState == GameState.Overworld)
        {
            if (TransientDataScript.CameraView != CameraView.Normal)
            {
                ToggleCoachCamera(false);
            }
            else
            {
                ToggleCoachCamera(true);
            }
        }
        else if (TransientDataScript.GameState == GameState.MapMenu)
        {
            ToggleMap(false);
        }
    }

    private void HandleEscapeKey()
    {
        switch (TransientDataScript.GameState)
        {
            case GameState.Overworld:
                ToggleCoachCamera(false);
                Debug.Log("Escape key was pressed");
                TransientDataScript.gameManager.menuSystem.startMenu.SetActive(true);
                break;
            case GameState.MapMenu:
                ToggleMap(false);
                break;
            case GameState.JournalMenu:
                TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
                break;
        }
    }

    private void ToggleEngineState()
    {
        ToggleEngineState(TransientDataScript.transientData.engineState != EngineState.Off ? EngineState.Off : EngineState.FirstGear);
    }

    private void ToggleEngineState(EngineState targetState)
    {
        if (TransientDataScript.GameState == GameState.Overworld || TransientDataScript.GameState == GameState.MapMenu)
        {
            TransientDataScript.transientData.engineState = TransientDataScript.transientData.engineState != targetState ? targetState : EngineState.Off;
        }
    }

    private void ToggleMap(bool enable)
    {
        if ((TransientDataScript.GameState == GameState.Overworld || TransientDataScript.GameState == GameState.MapMenu) && TransientDataScript.CameraView == CameraView.Normal)
        {
            TransientDataScript.SetGameState(enable ? GameState.MapMenu : GameState.Overworld, nameof(GameController), gameObject);
            gm.mapComponent.ToggleEnable(enable);
        }
    }

    private void ToggleCoachCamera(bool setClose)
    {
        if (TransientDataScript.GameState == GameState.Overworld)
        {
            if (setClose)
            {
                gm.cameraComponent.CameraClose();
            }
            else
            {
                gm.cameraComponent.CameraNormal();
            }
        }
    }

    private void ToggleJournalMenu()
    {
        if (TransientDataScript.GameState == GameState.Overworld)
        {
            TransientDataScript.SetGameState(GameState.JournalMenu, name, gameObject);
            TransientDataScript.gameManager.menuSystem.journalMenu.gameObject.SetActive(true);
        }
        else if (TransientDataScript.GameState == GameState.JournalMenu)
        {
            TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
        }
    }

    //private void ToggleJournalMenu(JournalMainPage mainPage)
    //{
    //    if (TransientDataScript.GameState == GameState.Overworld)
    //    {
    //        TransientDataScript.SetGameState(GameState.JournalMenu, name, gameObject);
    //        TransientDataScript.gameManager.menuSystem.journalMenu.SetActive(true);
    //        TransientDataScript.gameManager.menuSystem.journalMenu.GetComponent<Journal>().mainPage = mainPage;
    //    }
    //    else if (TransientDataScript.GameState == GameState.JournalMenu)
    //    {
    //        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    //    }
    //}

    private void ChangeResolution(int direction)
    {
        screenSetting = (screenSetting + direction) % 3;
        if (screenSetting < 0) screenSetting += 3;

        switch (screenSetting)
        {
            //case 0:
            //    Screen.SetResolution(960, 540, false);
            //    break;
            case 0:
                Screen.SetResolution(1440, 810, false);
                break;
            case 1:
                Screen.SetResolution(1920, 1080, false);
                break;
            case 2:
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                break;
        }

        Debug.Log(Screen.currentResolution);
    }
}