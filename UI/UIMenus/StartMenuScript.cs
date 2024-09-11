using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{
    public GameObject controllerMenu;
    public SettingsMenu settingsMenu;

    void OnEnable()
    {
        controllerMenu.SetActive(false);
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.Open();
    }

    private void OnDisable()
    {
        TransientDataScript.gameManager.saveDataManager.gameObject.SetActive(false);
        controllerMenu.SetActive(false);
        settingsMenu.Close();
    }
}
