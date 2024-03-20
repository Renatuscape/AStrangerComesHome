
using UnityEngine;

public class AlchemyProgressBar : MonoBehaviour
{
    public AlchemyMenu alchemyMenu;
    public SynthesiserData synthData;
    public GameObject fillBar;
    public GameObject containerBG;
    public float fullCoordinate;
    public float currentCoordinate;
    public float emptyCoordinate;
    public float percentageFill;
    public bool isEnabled;

    public void Initialise(SynthesiserData synthData)
    {
        fullCoordinate = containerBG.transform.position.x;
        emptyCoordinate = fullCoordinate - 700;
        this.synthData = synthData;
        percentageFill = CalculatePercentage();
        UpdateFillBarPosition();
        isEnabled = true;
    }

    private void Start()
    {
        fillBar.transform.position = new Vector3(emptyCoordinate, fillBar.transform.position.y, fillBar.transform.position.z);
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

    private void OnDisable()
    {
        isEnabled = false; // Ensure that the bar is initialised before it can start working
    }
}