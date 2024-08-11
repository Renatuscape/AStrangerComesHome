using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Toggle alwaysHideCoachExteriorToggle;
    public Dropdown resolutionDropdown;
    public GameObject content;

    public void Open()
    {
        content.SetActive(true);
    }

    public void Close()
    {
        content.SetActive(false);
    }

    void Start()
    {
        alwaysHideCoachExteriorToggle.onValueChanged.AddListener(ToggleAlwaysHideCoachExterior);
    }

    void OnEnable()
    {
        alwaysHideCoachExteriorToggle.isOn = GlobalSettings.AlwaysHideCoachExterior;
    }

    void ToggleAlwaysHideCoachExterior(bool toggle)
    {
        GlobalSettings.AlwaysHideCoachExterior = toggle;
        GlobalSettings.SaveSettings();
    }
}
