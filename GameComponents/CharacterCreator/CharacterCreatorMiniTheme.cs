using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreatorMiniTheme : MonoBehaviour
{
    public Image chatBackground;
    public Slider transparencySlider;
    public Button btnToggleTheme;

    private void Start()
    {
        btnToggleTheme.onClick.AddListener(() =>
        {
            GlobalSettings.DarkTheme = !GlobalSettings.DarkTheme;
            UpdateBackground();
        });
        transparencySlider.onValueChanged.AddListener((float x) =>
        {
            GlobalSettings.DialogueTransparency = transparencySlider.value;
            UpdateBackground();
        });
    }
    void OnEnable()
    {
        transparencySlider.value = GlobalSettings.DialogueTransparency;
        UpdateBackground();
    }

    public void UpdateBackground()
    {
        if (GlobalSettings.DarkTheme)
        {
            chatBackground.color = new Color(StaticGameValues.darkThemeBackground.r, StaticGameValues.darkThemeBackground.g, StaticGameValues.darkThemeBackground.b, GlobalSettings.DialogueTransparency); 
        }
        else
        {
            chatBackground.color = new Color(StaticGameValues.lightThemeBackground.r, StaticGameValues.lightThemeBackground.g, StaticGameValues.lightThemeBackground.b, GlobalSettings.DialogueTransparency);
        }
    }
}
