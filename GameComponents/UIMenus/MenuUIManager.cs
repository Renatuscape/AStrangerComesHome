using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    public TransientDataScript transientData;

    public GameObject startMenu;
    public GameObject shopMenu;
    public GameObject journalMenu;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TransientDataScript.GameState != GameState.Loading
            && TransientDataScript.GameState != GameState.MainMenu
            && TransientDataScript.GameState != GameState.CharacterCreation
            && TransientDataScript.GameState != GameState.Dialogue)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Escape key registered in MenuUIManager.");
                if (TransientDataScript.CameraView == CameraView.Normal
                    && (TransientDataScript.GameState == GameState.StartMenu
                    || TransientDataScript.GameState == GameState.JournalMenu
                    || TransientDataScript.GameState == GameState.MapMenu
                    || TransientDataScript.GameState == GameState.ShopMenu))
                {
                    EnableOverworld();
                }
                else if (TransientDataScript.CameraView != CameraView.Normal)
                {
                    TransientDataScript.SetCameraView(CameraView.Normal);
                }
                else if (TransientDataScript.GameState == GameState.Overworld)
                    startMenu.SetActive(true);
            }

            else if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
            {
                if (TransientDataScript.GameState == GameState.JournalMenu)
                {
                    TransientDataScript.SetGameState(GameState.Overworld);
                    Debug.Log(name + " changed GameState to " + GameState.Overworld);
                }
                else if (TransientDataScript.GameState == GameState.Overworld)
                    journalMenu.SetActive(true);
            }

            if (TransientDataScript.GameState == GameState.ShopMenu && !shopMenu.activeInHierarchy)
            {
                shopMenu.SetActive(true);
                transientData.engineState = EngineState.Off;
            }
        }
    }

    public void EnableOverworld()
    {
        if (TransientDataScript.GameState != GameState.CharacterCreation)
        {
            TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
