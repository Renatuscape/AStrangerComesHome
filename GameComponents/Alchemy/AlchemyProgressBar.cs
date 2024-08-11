
using System.Collections;
using UnityEngine;

public class AlchemyProgressBar : MonoBehaviour
{
    public AlchemyMenu alchemyMenu;
    public RectTransform fillBar;
    public GameObject containerBG;
    public float percentageFill;
    public bool isEnabled;

    public void Initialise(SynthesiserData synthData)
    {
        percentageFill = CalculatePercentage();
        UpdateFillBarPosition();
        isEnabled = true;
    }

    private void Start()
    {
        fillBar.localScale = new Vector3(0, 1, 1);
    }

    private void Update()
    {
        if (isEnabled)
        {
            if (AlchemyMenu.synthData != null && AlchemyMenu.synthData.synthRecipe != null && AlchemyMenu.synthData.isSynthActive && AlchemyMenu.synthData.synthRecipe.workload > 0)
            {
                percentageFill = CalculatePercentage();
                UpdateFillBarPosition();

                if (AlchemyMenu.synthData.progressSynth >= AlchemyMenu.synthData.synthRecipe.workload)
                {
                    alchemyMenu.SynthComplete();
                }
            }
            else if (AlchemyMenu.synthData != null && !AlchemyMenu.synthData.isSynthActive)
            {
                percentageFill = 0;
                UpdateFillBarPosition();
            }
        }
    }

    float CalculatePercentage()
    {
        if (AlchemyMenu.synthData == null || AlchemyMenu.synthData.synthRecipe == null)
        {
            return 0;
        }
        float maxValue = AlchemyMenu.synthData.synthRecipe.workload;
        float currentValue = AlchemyMenu.synthData.progressSynth;
        float minValue = 0;

        // Calculate the percentage completion
        float percentage;

        if (currentValue == 0 || maxValue == 0)
        {
            return 0;
        }
        else
        {
            percentage = Mathf.Clamp01((currentValue - minValue) / (maxValue - minValue));
        }
        return percentage;
    }

    void UpdateFillBarPosition()
    {
        float xScale = Mathf.Lerp(0, 1, percentageFill);

        if (xScale >= 0 && xScale <= 1)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleBar(percentageFill));
        }
        else
        {
            Debug.Log("Progress bar tried to update to strange position.");
        }
    }

    IEnumerator ScaleBar(float xGoal)
    {
        float duration = 0.5f;
        float xStart = fillBar.localScale.x;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            float newX = Mathf.Lerp(xStart, xGoal, t);

            float roundedX = Mathf.Round(newX * 1000000f) / 1000000f;
            fillBar.localScale = new Vector3(roundedX, fillBar.localScale.y, fillBar.localScale.z);

            yield return null; // Wait for the next frame
        }

        fillBar.localScale = new Vector3(xGoal, fillBar.localScale.y, fillBar.localScale.z);
    }

    private void OnDisable()
    {
        isEnabled = false; // Ensure that the bar is initialised before it can start working
    }
}