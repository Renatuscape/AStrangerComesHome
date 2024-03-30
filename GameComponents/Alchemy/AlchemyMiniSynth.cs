using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyMiniSynth : MonoBehaviour
{
    public SynthesiserData synthData;
    public CoachCabinetMenu cabinetMenu;

    public TextMeshProUGUI recipeTitle;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI manaDrainRate;
    public TextMeshProUGUI currentMana;

    // Progress bar
    public Slider progressBar;
    public bool isEnabled;

    // Buttons
    public Button btnExamine;
    public Button btnPause;
    public Button btnResume;

    private void Start()
    {
        btnPause.onClick.AddListener(() => PauseSynth());
        btnResume.onClick.AddListener(() => ResumeSynth());
        btnExamine.onClick.AddListener(() => Examine());
    }

    public void Initialise(SynthesiserData synthData, CoachCabinetMenu cabinetMenu)
    {
        this.synthData = synthData;
        this.cabinetMenu = cabinetMenu;

        if (synthData != null)
        {
            btnExamine.enabled = true;

            if (synthData.isSynthActive && synthData.synthRecipe != null)
            {
                if (Player.GetCount(synthData.synthRecipe.objectID, name) > 0)
                {
                    recipeTitle.text = synthData.synthRecipe.name.ToLower().Replace(" recipe", "");
                }
                else
                {
                    recipeTitle.text = "Unknown Recipe";
                }

                isEnabled = true;
            }
            else
            {
                progressBar.value = 0;
                recipeTitle.text = "Inactive";
                isEnabled = false; // stay false. No need to update the progress bar if there's no recipe
            }

            SetStatusText();
            ToggleButtons();
            UpdateSliderValue(synthData.progressSynth);
        }
    }

    public void Examine()
    {
        Debug.Log($"Sending {synthData.synthesiserID} to alchemy menu.");
        TransientDataCalls.gameManager.menuSystem.alchemyMenu.InitialiseBySynthesiser(synthData);
        cabinetMenu.CloseCabinet();
    }

    public void PauseSynth()
    {
        synthData.isSynthPaused = true;
        ToggleButtons();
        SetStatusText();
        isEnabled = false;
    }

    public void ResumeSynth()
    {
        synthData.isSynthPaused = false;
        ToggleButtons();
        SetStatusText();
        isEnabled = true;
    }

    void ToggleButtons()
    {
        if (synthData.isSynthActive)
        {
            if (synthData.isSynthPaused)
            {
                btnResume.interactable = true;
                btnPause.interactable = false;
            }
            else
            {
                btnResume.interactable = false;
                btnPause.interactable = true;
            }
        }
        else
        {
            btnResume.interactable = false;
            btnPause.interactable = false;
        }
    }

    void Update()
    {
        if (isEnabled)
        {
            if (synthData != null && synthData.synthRecipe != null && synthData.isSynthActive && synthData.synthRecipe.workload > 0)
            {
                UpdateSliderValue(synthData.progressSynth);
                currentMana.text = $"{(int)TransientDataCalls.transientData.currentMana}/{(int)TransientDataCalls.transientData.manapool}";
            }
            else if (synthData == null || !synthData.isSynthActive)
            {
                UpdateSliderValue(0);
            }

            if (synthData.isSynthPaused)
            {
                SetStatusText();
                ToggleButtons();
                isEnabled = false;
            }
        }

        currentMana.text = $"{(int)TransientDataCalls.transientData.currentMana}/{(int)TransientDataCalls.transientData.manapool}";
    }

    void SetStatusText()
    {
        if (synthData.isSynthActive)
        {
            if (synthData.isSynthPaused)
            {
                statusText.text = "Synthesis has been paused.";
                manaDrainRate.text = $"({synthData.synthRecipe.manaDrainRate})";
            }
            else if (synthData.progressSynth < synthData.synthRecipe.workload)
            {
                statusText.text = "Synthesis in progress. Consuming mana.";
                manaDrainRate.text = $"- {synthData.synthRecipe.manaDrainRate} drain";
            }
            else
            {
                statusText.text = "Synthesis complete!";
                manaDrainRate.text = $"";
            }
        }
        else
        {
            statusText.text = "Examine synthesiser to perform alchemy.";
            manaDrainRate.text = $"";
        }
    }

    void UpdateSliderValue(float currentValue)
    {
        float maxValue;
        float minValue = 0;

        if (synthData != null && synthData.synthRecipe != null)
        {
            maxValue = synthData.synthRecipe.workload;
        }
        else
        {
            maxValue = 100;
            currentValue = 0;
        }

        // Calculate the percentage
        float percentage = Mathf.Clamp01((currentValue - minValue) / (maxValue - minValue));

        // Calculate the target value for the slider
        float targetValue = Mathf.Lerp(0, 1, percentage); // Assuming the slider's range is from 0 to 1

        // Smoothly move the slider to the target value
        progressBar.value = Mathf.Lerp(progressBar.value, targetValue, Time.deltaTime * 2);
    }
}
