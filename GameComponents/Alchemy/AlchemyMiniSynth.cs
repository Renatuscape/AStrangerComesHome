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

    // Progress bar
    public GameObject fillBar;
    public GameObject containerBG;
    public float fullCoordinate;
    public float currentCoordinate;
    public float emptyCoordinate;
    public float percentageFill;
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

                manaDrainRate.text = synthData.synthRecipe.manaDrainRate.ToString();

                isEnabled = true;
            }
            else
            {
                manaDrainRate.text = "0";
                recipeTitle.text = "Inactive";
                isEnabled = false; // stay false. No need to update the progress bar if there's no recipe
            }

            SetStatusText();
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
        SetStatusText();
    }

    public void ResumeSynth()
    {
        synthData.isSynthPaused = false;
        SetStatusText();
    }

    void Update()
    {
        if (isEnabled)
        {
            if (synthData != null && synthData.synthRecipe != null && synthData.isSynthActive && synthData.synthRecipe.workload > 0)
            {
                percentageFill = CalculatePercentage();
                UpdateFillBarPosition();
            }
            else if (synthData != null && !synthData.isSynthActive)
            {
                percentageFill = 0;
                UpdateFillBarPosition();
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
            }
            else if (synthData.progressSynth < synthData.synthRecipe.workload)
            {
                statusText.text = "Synthesis in progress. Consuming mana.";
            }
            else
            {
                statusText.text = "Synthesis complete!";
            }
        }
        else
        {
            statusText.text = "Examine synthesiser to perform alchemy.";
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

    void UpdateFillBarPosition()
    {
        float smoothness = 2;
        // Calculate the new x-coordinate of the fill bar based on the percentage completion
        float targetXCoordinate = Mathf.Lerp(emptyCoordinate, fullCoordinate, percentageFill);

        if (targetXCoordinate > -1000 && targetXCoordinate < 1000)
        {
            // Smoothly move the fill bar to the target position
            fillBar.transform.position = Vector3.Lerp(fillBar.transform.position, new Vector3(targetXCoordinate, fillBar.transform.position.y, fillBar.transform.position.z), Time.deltaTime * smoothness);
        }
        else
        {
            Debug.Log("Progress bar tried to update to strange position.");
        }
    }
}
