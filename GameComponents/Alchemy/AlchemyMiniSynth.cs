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

    public void Initialise(SynthesiserData synthData, CoachCabinetMenu cabinetMenu)
    {
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
                recipeTitle.text = "Inactive";
                isEnabled = false; // stay false. No need to update the progress bar if there's no recipe
            }

            SetStatusText();
            ToggleButtons();
        }
    }

    public void Examine()
    {
        TransientDataCalls.gameManager.menuSystem.alchemyMenu.Initialise(synthData);
        cabinetMenu.CloseCabinet();
    }

    public void PauseSynth()
    {
        synthData.isSynthPaused = true;
        ToggleButtons();
        SetStatusText();
        currentMana.text = "";
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
                btnResume.enabled = true;
                btnPause.enabled = false;
            }
            else
            {
                btnResume.enabled = false;
                btnPause.enabled = true;
            }
        }
        else
        {
            btnResume.enabled = false;
            btnPause.enabled = false;
        }
    }

    void Update()
    {
        if (isEnabled)
        {
            if (synthData != null && synthData.synthRecipe != null && synthData.isSynthActive && synthData.synthRecipe.workload > 0)
            {
                progressBar.value = CalculatePercentage();
                currentMana.text = $"{TransientDataCalls.transientData.currentMana}/{TransientDataCalls.transientData.manapool}";
            }
            else if (synthData == null || !synthData.isSynthActive)
            {
                progressBar.value = 0;
                currentMana.text = $"{TransientDataCalls.transientData.currentMana}/{TransientDataCalls.transientData.manapool}";
            }
        }
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
                manaDrainRate.text = $"- {synthData.synthRecipe.manaDrainRate}/tick";
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

    float CalculatePercentage()
    {
        if (synthData == null || synthData.synthRecipe == null)
        {
            return 0;
        }
        float maxValue = synthData.synthRecipe.workload;
        float currentValue = synthData.progressSynth;
        float minValue = 0;

        // Calculate the percentage completion
        float percentage = Mathf.Clamp01((currentValue - minValue) / (maxValue - minValue));
        return percentage;
    }
}
