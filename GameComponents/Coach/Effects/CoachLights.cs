using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class CoachLights : MonoBehaviour
{
    public DataManagerScript dataManager;
    public GameObject lanternA;
    public GameObject lanternB;
    public bool isOff = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (dataManager.timeOfDay < 0.8 && dataManager.timeOfDay > 0.3)
        {
            if (!isOff)
            {
                StartCoroutine(FlickerOff(lanternA.GetComponent<Light2D>()));
                StartCoroutine(FlickerOff(lanternB.GetComponent<Light2D>()));
                isOff = true;
            }
        }
        else
        {
            if (isOff)
            {
                StartCoroutine(FlickerOn(lanternA.GetComponent<Light2D>()));
                StartCoroutine(FlickerOn(lanternB.GetComponent<Light2D>()));
                isOff = false;
            }
        }
    }

    public IEnumerator FlickerOff(Light2D light)
    {
        var originalIntensity = light.intensity;

        light.intensity = 1.2f;
        yield return new WaitForSeconds(0.1f);
        light.intensity = 0.5f;
        yield return new WaitForSeconds(0.1f);
        light.intensity = 1.0f;
        yield return new WaitForSeconds(0.1f);
        light.intensity = 0.8f;
        yield return new WaitForSeconds(0.15f);
        light.intensity = 0.5f;
        yield return new WaitForSeconds(0.15f);
        light.intensity = 0.3f;
        yield return new WaitForSeconds(0.15f);
        light.intensity = 0.1f;
        yield return new WaitForSeconds(0.15f);
        light.enabled = false;

        light.intensity = originalIntensity;
    }

    public IEnumerator FlickerOn(Light2D light)
    {
        var originalIntensity = light.intensity;

        light.enabled = enabled;
        light.intensity = 0.5f;
        yield return new WaitForSeconds(0.1f);
        light.intensity = 0.2f;
        yield return new WaitForSeconds(0.1f);
        light.intensity = 0.8f;
        yield return new WaitForSeconds(0.1f);
        light.intensity = 0f;
        yield return new WaitForSeconds(0.1f);
        light.intensity = 0.1f;
        yield return new WaitForSeconds(0.15f);
        light.intensity = 0.3f;
        yield return new WaitForSeconds(0.15f);
        light.intensity = 0.5f;
        yield return new WaitForSeconds(0.15f);
        light.intensity = 0.8f;
        yield return new WaitForSeconds(0.15f);
        light.intensity = originalIntensity;
    }
}
