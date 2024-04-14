using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.UI;

public class DialogueDisplay : MonoBehaviour
{
    public GameObject buttonContainer;
    public GameObject dialogueContainer;

    public GameObject leftNameDisplay;
    public GameObject rightNameDisplay;

    public GameObject historyDisplay;
    public TextMeshProUGUI chatHistory;

    public TextMeshProUGUI contentText;
    public TextMeshProUGUI leftNameText;
    public TextMeshProUGUI rightNameText;

    public Button btnToggleSettings;
    public Button btnToggleTheme;
    public Button btnSpeed;
    public Button btnAutoPlay;
    public Button btnHistory;

    public Slider opacitySlider;

    public Dialogue activeDialogue;
    public DialogueEvent activeEvent;
    public bool continueEnabled;
    public bool isPrinting;
    public bool autoEnabled;
    public int eventIndex;

    public List<Image> backgroundImages;
    public Color lightThemeBackground;
    public Color darkThemeBackground;

    public float printSpeed = 0.05f;

    public float autoDelay = 2;
    public float autoTimer;
    private void Start()
    {
        historyDisplay.SetActive(false);
        gameObject.SetActive(false);

        btnToggleSettings.onClick.AddListener(() => ToggleSettings());
        btnToggleTheme.onClick.AddListener(() => ToggleTheme());
        btnAutoPlay.onClick.AddListener(() => ToggleAuto());

        GlobalSettingsManager.LoadSettings();
        opacitySlider.value = GlobalSettings.dialogueTransparency;

        if (GlobalSettings.darkTheme)
        {
            SetDarkTheme();
        }
        else
        {
            SetLightTheme();
        }
    }

    private void Update()
    {
        if (autoEnabled && !isPrinting)
        {
            autoTimer += Time.deltaTime;

            if (autoTimer > autoDelay)
            {
                if (eventIndex < activeDialogue.dialogueEvents.Count)
                {
                    autoTimer = 0;
                    PrintEvent();
                }
            }
        }
    }

    // Concerns only the display of text. Portraits are handled by DialoguePortraitManager
    // DialogueMenu handles the quest and quest progression
    public void StartDialogue(Dialogue dialogue)
    {
        gameObject.SetActive(true);
        activeDialogue = dialogue;
        eventIndex = 0;
        chatHistory.text = "";

        PrintEvent();
    }

    public void PrintEvent()
    {
        activeEvent = activeDialogue.dialogueEvents[eventIndex];

        if (activeEvent.speaker.objectID == "ARC999")
        {
            rightNameDisplay.gameObject.SetActive(false);
            leftNameDisplay.gameObject.SetActive(false);
        }
        else
        {
            if (activeEvent.isLeft || activeEvent.speaker.objectID == "ARC000")
            {
                leftNameText.text = activeEvent.speaker.NamePlate();
                leftNameDisplay.gameObject.SetActive(true);
                rightNameDisplay.gameObject.SetActive(false);
            }
            else
            {
                rightNameText.text = activeEvent.speaker.NamePlate();
                rightNameDisplay.SetActive(true);
                leftNameDisplay.gameObject.SetActive(false);
            }

            chatHistory.text += "\n" + activeEvent.speaker.NamePlate() + "\n";
        }

        // Parse tags here instead of at start to get latest tags
        var parsedText = DialogueTagParser.ParseText(activeEvent.content);
        StartCoroutine(PrintContent(parsedText));
        chatHistory.text += parsedText + "\n";

        eventIndex++;

        if (eventIndex >= activeDialogue.dialogueEvents.Count)
        {
            continueEnabled = false;

            // Handle choices
        }
        else if (!autoEnabled)
        {
            continueEnabled = true;
        }

        Canvas.ForceUpdateCanvases();
    }

    IEnumerator PrintContent(string textToPrint)
    {
        printSpeed = 0.05f;
        isPrinting = true;
        contentText.text = "";

        var textArray = textToPrint.Split(' ');

        foreach (var text in textArray)
        {
            yield return new WaitForSeconds(printSpeed);
            contentText.text += text + " ";
        }

        isPrinting = false;
    }

    public void Continue()
    {
        if (continueEnabled && !isPrinting)
        {
            PrintEvent();
        }
        else if (isPrinting)
        {
            printSpeed = 0;
        }
    }

    public void ToggleSettings()
    {
        buttonContainer.SetActive(!buttonContainer.activeInHierarchy);
    }

    public void ToggleTheme()
    {
        GlobalSettings.darkTheme = !GlobalSettings.darkTheme;
        GlobalSettingsManager.SaveSettings();

        if (GlobalSettings.darkTheme)
        {
            SetDarkTheme();
        }
        else
        {
            SetLightTheme();
        }
    }

    public void ToggleAuto()
    {
        autoEnabled = !autoEnabled;
    }

    void SetLightTheme()
    {
        Color newBGColour = new Color(lightThemeBackground.r, lightThemeBackground.g, lightThemeBackground.b, GlobalSettings.dialogueTransparency);

        foreach (Image img in backgroundImages)
        {
            img.color = newBGColour;
        }

        contentText.color = Color.black;
    }

    void SetDarkTheme()
    {
        Color newBGColour = new Color(darkThemeBackground.r, darkThemeBackground.g, darkThemeBackground.b, GlobalSettings.dialogueTransparency);

        foreach (Image img in backgroundImages)
        {
            img.color = newBGColour;
        }

        contentText.color = Color.white;
    }

    public void SliderValueChange()
    {
        GlobalSettings.dialogueTransparency = opacitySlider.value;

        Color newBGColour = new Color(backgroundImages[0].color.r, backgroundImages[0].color.g, backgroundImages[0].color.b, GlobalSettings.dialogueTransparency);

        foreach (Image img in backgroundImages)
        {
            img.color = newBGColour;
        }

        GlobalSettingsManager.SaveSettings();
    }
}