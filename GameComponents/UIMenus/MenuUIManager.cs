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
        if (transientData.gameState != GameState.Loading && transientData.gameState != GameState.MainMenu && transientData.gameState != GameState.CharacterCreation && transientData.gameState != GameState.Dialogue)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Escape key registered in MenuUIManager.");
                if (transientData.cameraView == CameraView.Normal && (transientData.gameState == GameState.StartMenu || transientData.gameState == GameState.JournalMenu || transientData.gameState == GameState.MapMenu || transientData.gameState == GameState.ShopMenu))
                {
                    EnableOverworld();
                }
                else if (transientData.cameraView != CameraView.Normal)
                {
                    transientData.cameraView = CameraView.Normal;
                }
                else if (transientData.gameState == GameState.Overworld)
                    startMenu.SetActive(true);
            }

            else if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
            {
                if (transientData.gameState == GameState.JournalMenu)
                {
                    transientData.gameState = GameState.Overworld;
                    Debug.Log(name + " changed GameState to " + GameState.Overworld);
                }
                else if (transientData.gameState == GameState.Overworld)
                    journalMenu.SetActive(true);
            }

            if (transientData.gameState == GameState.ShopMenu && !shopMenu.activeInHierarchy)
            {
                shopMenu.SetActive(true);
                transientData.engineState = EngineState.Off;
            }
        }
    }

    public void EnableOverworld()
    {
        if (transientData.gameState != GameState.CharacterCreation)
        {
            transientData.ChangeGameState(name, gameObject, GameState.Overworld);
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
