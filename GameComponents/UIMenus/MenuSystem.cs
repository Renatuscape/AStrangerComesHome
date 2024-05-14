using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MonoBehaviour
{
    public TransientDataScript transientData;

    public GameObject startMenu;
    public ShopMenu shopMenu;
    public GiftMenu giftMenu;
    public InteractMenu interactMenu;
    public GameObject journalMenu;
    public AlchemyMenu alchemyMenu;
    public GarageMenu garageMenu;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        interactMenu.gameObject.SetActive(true); //Ensures that the static method has an instance to use
        interactMenu.gameObject.SetActive(false);
    }

    public void CloseAnyMenu()
    {
        if (TransientDataScript.GameState != GameState.Loading
        && TransientDataScript.GameState != GameState.MainMenu
        && TransientDataScript.GameState != GameState.CharacterCreation
        && TransientDataScript.GameState != GameState.Dialogue)
        {
            EnableOverworld();
        }
    }
    void Update()
    {
        if (TransientDataScript.GameState != GameState.Loading
            && TransientDataScript.GameState != GameState.MainMenu
            && TransientDataScript.GameState != GameState.CharacterCreation
            && TransientDataScript.GameState != GameState.Dialogue)
        {
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    Debug.Log("Escape key registered in MenuUIManager.");
            //    if (TransientDataScript.CameraView == CameraView.Normal
            //        && (TransientDataScript.GameState == GameState.StartMenu
            //        || TransientDataScript.GameState == GameState.JournalMenu
            //        || TransientDataScript.GameState == GameState.MapMenu
            //        || TransientDataScript.GameState == GameState.ShopMenu))
            //    {
            //        EnableOverworld();
            //    }
            //    else if (TransientDataScript.CameraView != CameraView.Normal)
            //    {
            //        TransientDataScript.SetCameraView(CameraView.Normal);
            //    }
            //}

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
