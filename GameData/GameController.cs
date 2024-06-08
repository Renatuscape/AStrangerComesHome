using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameManagerScript gm;

    private void Update()
    {
        if (TransientDataScript.GameState != GameState.Loading &&
            TransientDataScript.GameState != GameState.MainMenu &&
            TransientDataScript.GameState != GameState.ShopMenu &&
            TransientDataScript.GameState != GameState.CharacterCreation &&
            TransientDataScript.GameState != GameState.Dialogue)
        {
            if (Input.GetKeyDown(KeyCode.Space))
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (TransientDataScript.GameState == GameState.Overworld)
                {
                    ToggleCoachCamera(false);
                    Debug.Log("escape key was pressed");
                    TransientDataScript.gameManager.menuSystem.startMenu.SetActive(true);
                }
                else if (TransientDataScript.GameState == GameState.MapMenu)
                {
                    ToggleMap(false);
                }
                else if (TransientDataScript.GameState == GameState.JournalMenu)
                {
                    TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                if (TransientDataScript.GameState == GameState.Overworld ||
                    TransientDataScript.GameState == GameState.MapMenu)
                {
                    if (TransientDataScript.transientData.engineState != EngineState.Off)
                    {
                        TransientDataScript.transientData.engineState = EngineState.Off;
                    }
                    else
                    {
                        TransientDataScript.transientData.engineState = EngineState.FirstGear;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                if (TransientDataScript.GameState == GameState.Overworld ||
                    TransientDataScript.GameState == GameState.MapMenu)
                {
                    if (TransientDataScript.transientData.engineState != EngineState.FirstGear)
                    {
                        TransientDataScript.transientData.engineState = EngineState.FirstGear;
                    }
                    else
                    {
                        TransientDataScript.transientData.engineState = EngineState.Off;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                if (TransientDataScript.GameState == GameState.Overworld ||
                   TransientDataScript.GameState == GameState.MapMenu)
                {
                    if (TransientDataScript.transientData.engineState != EngineState.SecondGear)
                    {
                        TransientDataScript.transientData.engineState = EngineState.SecondGear;
                    }
                    else
                    {
                        TransientDataScript.transientData.engineState = EngineState.Off;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                if (TransientDataScript.GameState == GameState.Overworld ||
                    TransientDataScript.GameState == GameState.MapMenu)
                {
                    if (TransientDataScript.transientData.engineState != EngineState.ThirdGear)
                    {
                        TransientDataScript.transientData.engineState = EngineState.ThirdGear;
                    }
                    else
                    {
                        TransientDataScript.transientData.engineState = EngineState.Off;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (TransientDataScript.GameState == GameState.Overworld ||
                    TransientDataScript.GameState == GameState.MapMenu)
                {
                    if (TransientDataScript.transientData.engineState != EngineState.Reverse)
                    {
                        TransientDataScript.transientData.engineState = EngineState.Reverse;
                    }
                    else
                    {
                        TransientDataScript.transientData.engineState = EngineState.Off;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (TransientDataScript.GameState == GameState.Overworld ||
                    TransientDataScript.GameState == GameState.MapMenu)
                {
                    if (TransientDataScript.transientData.engineState != EngineState.Off)
                    {
                        TransientDataScript.transientData.engineState = EngineState.Off;
                    }
                    else
                    {
                        TransientDataScript.transientData.engineState = EngineState.FirstGear;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                if (TransientDataScript.GameState == GameState.Overworld ||
                    TransientDataScript.GameState == GameState.MapMenu)
                {
                    if (TransientDataScript.transientData.engineState != EngineState.Reverse)
                    {
                        TransientDataScript.transientData.engineState = EngineState.Reverse;
                    }
                    else
                    {
                        TransientDataScript.transientData.engineState = EngineState.Off;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                if (TransientDataScript.GameState == GameState.Overworld)
                {
                    ToggleMap(true);
                }
                else if (TransientDataScript.GameState == GameState.MapMenu)
                {
                    ToggleMap(false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.J))
            {
                if (TransientDataScript.GameState == GameState.Overworld)
                {
                    TransientDataScript.SetGameState(GameState.JournalMenu, name, gameObject);
                    TransientDataScript.gameManager.menuSystem.journalMenu.SetActive(true);
                }
                else if (TransientDataScript.GameState == GameState.JournalMenu)
                {
                    TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
                }
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                if (TransientDataScript.GameState == GameState.Overworld)
                {
                    TransientDataScript.SetGameState(GameState.JournalMenu, name, gameObject);
                    TransientDataScript.gameManager.menuSystem.journalMenu.SetActive(true);
                    TransientDataScript.gameManager.menuSystem.journalMenu.GetComponent<Journal>().mainPage = JournalMainPage.Inventory;
                }
                else if (TransientDataScript.GameState == GameState.JournalMenu)
                {
                    TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                if (TransientDataScript.GameState == GameState.Overworld)
                {
                    TransientDataScript.SetGameState(GameState.JournalMenu, name, gameObject);
                    TransientDataScript.gameManager.menuSystem.journalMenu.SetActive(true);
                    TransientDataScript.gameManager.menuSystem.journalMenu.GetComponent<Journal>().mainPage = JournalMainPage.Quests;
                }
                else if (TransientDataScript.GameState == GameState.JournalMenu)
                {
                    TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
                }
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            screenSetting++;
            Debug.Log("Change resolution up registered.");
            SetScreenResolution();
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            screenSetting--;
            Debug.Log("Change resolution down registered.");
            SetScreenResolution();
        }
    }

    void ToggleMap(bool enable)
    {
        if (enable)
        {
            TransientDataScript.SetGameState(GameState.MapMenu, nameof(GameController), gameObject);
            gm.mapComponent.ToggleEnable(true);
        }
        else
        {
            TransientDataScript.SetGameState(GameState.Overworld, nameof(GameController), gameObject);
            gm.mapComponent.ToggleEnable(false);
        }
    }

    void ToggleCoachCamera(bool setClose)
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

    int screenSetting = 2;
    void SetScreenResolution()
    {
        int max = 3;
        if (screenSetting < 0)
        {
            screenSetting = max;
        }
        else if (screenSetting > max)
        {
            screenSetting = 0;
        }

        if (screenSetting == max)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else if (screenSetting == 2)
        {
            Screen.SetResolution(1920, 1080, false);
        }
        else if (screenSetting == 1)
        {
            Screen.SetResolution(1440, 810, false);
        }
        else if (screenSetting == 0)
        {
            Screen.SetResolution(960, 540, false);
        }

        Debug.Log(Screen.currentResolution);
    }
}