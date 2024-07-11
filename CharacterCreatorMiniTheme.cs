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
            GlobalSettings.darkTheme = !GlobalSettings.darkTheme;
            UpdateBackground();
        });
        transparencySlider.onValueChanged.AddListener((float x) =>
        {
            GlobalSettings.dialogueTransparency = transparencySlider.value;
            UpdateBackground();
        });

        UpdateBackground();
    }
    void OnEnable()
    {
        transparencySlider.value = GlobalSettings.dialogueTransparency;
    }

    public void UpdateBackground()
    {
        if (GlobalSettings.darkTheme)
        {
            chatBackground.color = new Color(StaticGameValues.darkThemeBackground.r, StaticGameValues.darkThemeBackground.g, StaticGameValues.darkThemeBackground.b, GlobalSettings.dialogueTransparency); 
        }
        else
        {
            chatBackground.color = new Color(StaticGameValues.lightThemeBackground.r, StaticGameValues.lightThemeBackground.g, StaticGameValues.lightThemeBackground.b, GlobalSettings.dialogueTransparency);
        }
    }
}
