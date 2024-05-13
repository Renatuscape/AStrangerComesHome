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
            TransientDataScript.GameState != GameState.StartMenu &&
            TransientDataScript.GameState != GameState.JournalMenu &&
            TransientDataScript.GameState != GameState.CharacterCreation &&
            TransientDataScript.GameState != GameState.Dialogue)
        {
            if (Input.GetKeyDown("space"))
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
            if (Input.GetKeyDown("escape"))
            {
                if (TransientDataScript.GameState == GameState.Overworld)
                {
                    ToggleCoachCamera(false);
                    Debug.Log("escape key was pressed");
                }
                else if (TransientDataScript.GameState == GameState.MapMenu)
                {
                    ToggleMap(false);
                }
            }
            else if (Input.GetKeyDown("return"))
            {
                if (TransientDataScript.GameState == GameState.Overworld ||
                    TransientDataScript.GameState == GameState.MapMenu)
                {
                    if (TransientDataScript.transientData.engineState != EngineState.Off)
                    {
                        TransientDataScript.transientData.engineState = EngineState.Off;
                    }
                }
            }
            else if (Input.GetKeyDown("1") || Input.GetKeyDown(KeyCode.Keypad1))
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
            else if (Input.GetKeyDown("2") || Input.GetKeyDown(KeyCode.Keypad2))
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
            else if (Input.GetKeyDown("3") || Input.GetKeyDown(KeyCode.Keypad3))
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
}