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

    public void ToggleControlMenu()
    {
        controllerMenu.SetActive(!controllerMenu.activeInHierarchy);
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.Open();
    }
}
