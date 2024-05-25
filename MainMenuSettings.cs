using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSettings : MonoBehaviour
{
    public GameObject settingsPanel;
    bool isPanelActive = false;

    private void Start()
    {
        settingsPanel.SetActive(false);
    }
    public void ToggleSettingsPanel()
    {
        isPanelActive = !isPanelActive;

        settingsPanel.SetActive(isPanelActive);
    }

    private void OnDisable()
    {
        isPanelActive = false;
        settingsPanel.SetActive(false);
    }
}
