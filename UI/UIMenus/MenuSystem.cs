using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MenuSystem : MonoBehaviour
{
    public TransientDataScript transientData;

    public Canvas menuCanvas;
    public GameObject startMenu;
    public GameObject debugMenu;
    public ShopMenu shopMenu;
    public GiftMenu giftMenu;
    public InteractMenu interactMenu;
    public GameObject journalMenu;
    public AlchemyMenu alchemyMenu;
    public GarageMenu garageMenu;
    public GuildMenu guildMenu;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        interactMenu.gameObject.SetActive(true); //Ensures that the static method has an instance to use
        interactMenu.gameObject.SetActive(false);
        debugMenu.SetActive(false);
    }

    void Update()
    {
        if (TransientDataScript.GameState == GameState.JournalMenu)
        {
            if (Input.GetKey(KeyCode.LeftControl) &&
                Input.GetKey(KeyCode.LeftShift) &&
                Input.GetKey(KeyCode.D))
            {
                debugMenu.SetActive(true);
            }
        }
        if (TransientDataScript.GameState != GameState.JournalMenu && debugMenu.activeInHierarchy)
        {
            debugMenu.SetActive(false);
        }

        //if (TransientDataScript.GameState != GameState.AlchemyMenu &&
        //    TransientDataScript.GameState != GameState.ShopMenu &&
        //    TransientDataScript.GameState != GameState.StartMenu &&
        //    TransientDataScript.GameState != GameState.MainMenu &&
        //    TransientDataScript.GameState != GameState.JournalMenu &&
        //    TransientDataScript.GameState != GameState.Dialogue)
        //{
        //    if (menuCanvas.enabled == true)
        //    {
        //        menuCanvas.enabled = false;
        //    }
        //}
        //else
        //{
        //    if (menuCanvas.enabled == false)
        //    {
        //        menuCanvas.enabled = true;

        //        Canvas.ForceUpdateCanvases();
        //        Canvas.ForceUpdateCanvases();
        //    }
        //}
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
