
using UnityEngine;

public class AlchemyProgressBar : MonoBehaviour
{
    public AlchemyMenu alchemyMenu;
    public SynthesiserData synthData;
    public RectTransform fillBar;
    public GameObject containerBG;
    public float percentageFill;
    public bool isEnabled;

    public void Initialise(SynthesiserData synthData)
    {
        this.synthData = synthData;
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
            if (synthData != null && synthData.synthRecipe != null && synthData.isSynthActive && synthData.synthRecipe.workload > 0)
            {
                percentageFill = CalculatePercentage();
                UpdateFillBarPosition();

                if (synthData.progressSynth >= synthData.synthRecipe.workload)
                {
                    alchemyMenu.SynthComplete();
                }
            }
            else if (synthData != null && !synthData.isSynthActive)
            {
                percentageFill = 0;
                UpdateFillBarPosition();
            }
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

        float xScale = Mathf.Lerp(0, 1, percentageFill);

        if (xScale > -1000 && xScale < 1000)
        {
            // Smoothly move the fill bar to the target position
            fillBar.localScale = Vector3.Lerp(fillBar.localScale, new Vector3(xScale, 1, 1), Time.deltaTime * smoothness);
        }
        else
        {
            Debug.Log("Progress bar tried to update to strange position.");
        }
    }

    private void OnDisable()
    {
        isEnabled = false; // Ensure that the bar is initialised before it can start working
    }
}