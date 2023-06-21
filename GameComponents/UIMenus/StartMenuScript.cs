using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{
    public GameObject controllerMenu;
    public GameObject settingsMenu;
    public CanvasScaler canvasScaler;

    private void Awake()
    {
        if (Screen.width <= 1920)
            canvasScaler.scaleFactor = 2;
        else if (Screen.width > 1920 && Screen.width < 3840)
            canvasScaler.scaleFactor = 3;
        else if (Screen.width >= 3840)
            canvasScaler.scaleFactor = 4;
    }
    void OnEnable()
    {
        controllerMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleControlMenu()
    {
        controllerMenu.SetActive(!controllerMenu.activeInHierarchy);
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeInHierarchy);
    }
}
