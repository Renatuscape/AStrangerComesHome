using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MonoBehaviour
{
    public TransientDataScript transientData;

    public GameObject startMenu;
    public ShopMenu shopMenu;
    public InteractMenu interactMenu;
    public GameObject journalMenu;
    public AlchemyMenu alchemyMenu;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        interactMenu.gameObject.SetActive(true); //Ensures that the static method has an instance to use
        interactMenu.gameObject.SetActive(false);
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

            else if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.J))
            {
                if (TransientDataScript.GameState == GameState.JournalMenu)
                {
                    TransientDataScript.SetGameState(GameState.Overworld, "Menu UI Manager", gameObject);
                    Debug.Log(name + " changed GameState to " + GameState.Overworld);
                }
                else if (TransientDataScript.GameState == GameState.Overworld)
                {
                    TransientDataScript.SetGameState(GameState.JournalMenu, "Menu UI Manager", gameObject);
                    journalMenu.SetActive(true);
                }
            }

            if (TransientDataScript.GameState == GameState.ShopMenu && !shopMenu.gameObject.activeInHierarchy)
            {
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
