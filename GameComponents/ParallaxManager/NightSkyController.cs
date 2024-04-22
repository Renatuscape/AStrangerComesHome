using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NightSkyController : MonoBehaviour
{
    public DataManagerScript dataManager;
    public SpriteRenderer screen;
    public float timeOfDay;
    public float alphaValue;
    void Start()
    {
        screen = GetComponent<SpriteRenderer>();
        dataManager = TransientDataScript.gameManager.dataManager;
    }

    void Update()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            timeOfDay = dataManager.timeOfDay;
            alphaValue = 0;

            // Night
            if (timeOfDay < 0.3f || timeOfDay > 0.8f)
            {
                alphaValue = 1;
            }

            // Dawn
            else if (timeOfDay >= 0.3f && timeOfDay <= 0.4f)
            {
                alphaValue = Mathf.SmoothStep(1, 0, (timeOfDay - 0.3f) / 0.1f);
            }

            // Dusk
            else if (timeOfDay >= 0.7f && timeOfDay <= 0.8f)
            {
                alphaValue = (timeOfDay - 0.7f) / 0.1f;
            }

            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, alphaValue);
        }
    }
}
