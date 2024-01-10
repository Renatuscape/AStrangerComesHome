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
            if (TransientDataScript.GameState == GameState.Overworld)
            {
                if (Input.GetKeyDown("space"))
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
                if (Input.GetKeyDown("escape"))
                {
                    ToggleCoachCamera(false);
                }
                else if (Input.GetKeyDown("return"))
                {
                    Debug.Log("return key was pressed");
                }
                else if (Input.GetKeyDown("1"))
                {
                    Debug.Log("1 key was pressed");
                }
                else if (Input.GetKeyDown("2"))
                {
                    Debug.Log("2 key was pressed");
                }
                else if (Input.GetKeyDown("3"))
                {
                    Debug.Log("3 key was pressed");
                }
                else if (Input.GetKeyDown(KeyCode.M))
                {
                    ToggleMap(true);
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    Debug.Log("R key was pressed");
                }
            }
            else if (TransientDataScript.GameState == GameState.MapMenu)
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    ToggleMap(false );
                }
                else if (Input.GetKeyDown("escape"))
                {
                    ToggleMap(false);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown("escape"))
            {
                Debug.Log("escape key was pressed");
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