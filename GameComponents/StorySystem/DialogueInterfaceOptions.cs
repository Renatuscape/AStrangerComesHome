﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueInterfaceOptions : MonoBehaviour
{
    public GameObject buttonContainer;
    public Canvas chatLogCanvas;
    public TextMeshProUGUI chatHistory;

    public TextMeshProUGUI contentText;

    public List<Image> backgroundImages;
    public Color lightText;
    public Color darkText;
    public Color lightThemeBackground;
    public Color darkThemeBackground;

    public Button btnHistory;

    public Slider opacitySlider;

    public Button btnToggleSettings;
    public Button btnToggleTheme;
    public Button btnToggleSound;
    public Button btnCloseHistory;
    public Button btnOpenHistory;

    private void Start()
    {
        lightText = new Color(0.8509f, 0.8666f, 0.8980f);
        darkText = new Color(0.3921f, 0.2509f, 0.1294f);
        lightThemeBackground = new Color(0.9137f, 0.8705f, 0.8078f);
        darkThemeBackground = new Color(0.1882f, 0.1686f, 0.1607f);

        btnToggleSettings.onClick.AddListener(() => ToggleSettings());
        btnToggleTheme.onClick.AddListener(() => ToggleTheme());
        btnToggleSound.onClick.AddListener(() => ToggleSound());

        btnCloseHistory.onClick.AddListener(() => CloseHistory());
        btnOpenHistory.onClick.AddListener(() => OpenHistory());
    }
    private void OnEnable()
    {
        chatLogCanvas.gameObject.SetActive(false);
        chatHistory.text = "";

        GlobalSettings.LoadSettings();

        if (backgroundImages != null && backgroundImages.Count > 0)
        {
            opacitySlider.value = GlobalSettings.DialogueTransparency;
        }

        if (GlobalSettings.DarkTheme)
        {
            SetDarkTheme();
        }
        else
        {
            SetLightTheme();
        }

        if (GlobalSettings.DisableTextSound)
        {
            btnToggleSound.image.color = Color.gray;
        }
        else
        {
            btnToggleSound.image.color = Color.white;
        }
    }
    void SetLightTheme()
    {
        // Debug.Log("Chat theme set to light");
        Color newBGColour = new Color(lightThemeBackground.r, lightThemeBackground.g, lightThemeBackground.b, GlobalSettings.DialogueTransparency);

        foreach (Image img in backgroundImages)
        {
            img.color = newBGColour;
        }

        if (contentText.color != StaticGameValues.narrationText)
        {
            contentText.color = new Color(darkText.r, darkText.g, darkText.b, contentText.color.a);
        }
    }

    void SetDarkTheme()
    {
        // Debug.Log("Chat theme set to dark");
        Color newBGColour = new Color(darkThemeBackground.r, darkThemeBackground.g, darkThemeBackground.b, GlobalSettings.DialogueTransparency);

        foreach (Image img in backgroundImages)
        {
            img.color = newBGColour;
        }

        if (contentText.color != StaticGameValues.narrationText)
        {
            contentText.color = new Color(lightText.r, lightText.g, lightText.b, contentText.color.a);
        }
    }

    public void SliderValueChange()
    {
        if (backgroundImages != null && backgroundImages.Count > 0)
        {
            GlobalSettings.DialogueTransparency = opacitySlider.value;

            Color newBGColour = new Color(backgroundImages[0].color.r, backgroundImages[0].color.g, backgroundImages[0].color.b, GlobalSettings.DialogueTransparency);

            foreach (Image img in backgroundImages)
            {
                img.color = newBGColour;
            }

            GlobalSettings.SaveSettings();
        }
        else
        {
            Debug.LogWarning("No background images in the list. Make sure they are added correctly.");
        }
    }

    public void ToggleSettings()
    {
        buttonContainer.SetActive(!buttonContainer.activeInHierarchy);
    }

    public void ToggleTheme()
    {
        GlobalSettings.DarkTheme = !GlobalSettings.DarkTheme;
        GlobalSettings.SaveSettings();

        if (GlobalSettings.DarkTheme)
        {
            SetDarkTheme();
        }
        else
        {
            SetLightTheme();
        }
    }

    public void ToggleSound()
    {
        GlobalSettings.DisableTextSound = !GlobalSettings.DisableTextSound;
        GlobalSettings.SaveSettings();

        if (GlobalSettings.DisableTextSound)
        {
            btnToggleSound.image.color = Color.gray;
        }
        else
        {
            btnToggleSound.image.color = Color.white;
        }
    }

    public void CloseHistory()
    {
        chatLogCanvas.gameObject.SetActive(false);
        var sizeFitter = chatHistory.gameObject.GetComponent<ContentSizeFitter>();

        sizeFitter.enabled = false;
        sizeFitter.enabled = true;
        Canvas.ForceUpdateCanvases();

        sizeFitter.enabled = false;
        sizeFitter.enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    public void OpenHistory()
    {
        chatLogCanvas.gameObject.SetActive(true);
        var sizeFitter = chatHistory.gameObject.GetComponent<ContentSizeFitter>();

        sizeFitter.enabled = false;
        sizeFitter.enabled = true;
        Canvas.ForceUpdateCanvases();

        sizeFitter.enabled = false;
        sizeFitter.enabled = true;
        Canvas.ForceUpdateCanvases();
    }
}
