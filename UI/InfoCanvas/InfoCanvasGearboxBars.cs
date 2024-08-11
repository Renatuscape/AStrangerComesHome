using System.Collections;
using UnityEngine;
public class InfoCanvasGearboxBars : MonoBehaviour
{
    TransientDataScript transientData;
    public GameObject manaBar;
    public GameObject boostBar;
    public bool isEnabled;
    public float timer = 0;
    float animationTick = 0.1f;
    public void Start()
    {
        if (!isEnabled)
        {
            transientData = TransientDataScript.transientData;
            isEnabled = true;
        }
    }

    private void OnDisable()
    {
        manaBar.transform.localScale = new Vector3(0, manaBar.transform.localScale.y, manaBar.transform.localScale.z);
        boostBar.transform.localScale = new Vector3(0, boostBar.transform.localScale.y, boostBar.transform.localScale.z);
    }

    private void OnEnable()
    {
        if (isEnabled)
        {
            manaBar.transform.localScale = new Vector3(CalculatePercentage(transientData.currentMana, transientData.manapool), manaBar.transform.localScale.y, manaBar.transform.localScale.z);
            boostBar.transform.localScale = new Vector3(CalculatePercentage(transientData.engineBoost, transientData.maxEngineBoost), boostBar.transform.localScale.y, boostBar.transform.localScale.z);
        }
    }

    private void Update()
    {
        if (isEnabled && TransientDataScript.IsTimeFlowing())
        {
            timer += Time.deltaTime;
            
            if (timer > animationTick)
            {
                timer = 0;
                Tick();
            }
        }
    }
    public void Tick()
    {
        if (isEnabled)
        {
            UpdateFillBarPosition(manaBar.transform, transientData.currentMana, transientData.manapool);
            UpdateFillBarPosition(boostBar.transform, transientData.engineBoost, transientData.maxEngineBoost);            
        }
    }

    float CalculatePercentage(float currentValue, float maxValue)
    {
        if (currentValue == 0 || maxValue == 0)
        {
            return 0;
        }
        else
        {
            return Mathf.Clamp01(currentValue / maxValue);
        }
    }

    void UpdateFillBarPosition(Transform maskTransform, float currentValue, float maxValue)
    {
        float percentageFill = CalculatePercentage(currentValue, maxValue);
        float xScale = Mathf.Lerp(0, 1, percentageFill);

        if (xScale >= 0 && xScale <= 1)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleBar(maskTransform, percentageFill));
        }
        else
        {
            Debug.Log("Progress bar tried to update to strange position.");
        }
    }

    IEnumerator ScaleBar(Transform fillBar, float xGoal)
    {
        float duration = 0.15f;
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
}
