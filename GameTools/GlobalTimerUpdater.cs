using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTimerUpdater : MonoBehaviour
{
    public NightSkyController nightSkyController;

    public float animationTimer;
    float animationTick = 0.15f;
    public float secondTimer;
    float secondTick = 1;

    private void Start()
    {
        animationTimer = animationTick;
        secondTimer = secondTick;
    }
    void Update()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            animationTimer += Time.deltaTime;
            secondTimer += Time.deltaTime;

            if (animationTimer >= animationTick)
            {
                animationTimer = 0;
                PushAnimation();
            }

            if (secondTimer >= secondTick)
            {
                secondTimer = 0;
                PushSecond();
            }

            PushEveryFrame();
        }
    }

    void PushEveryFrame()
    {
        nightSkyController.StarFlicker();
    }
    void PushAnimation()
    {
        nightSkyController.UpdateNightSky();
    }

    void PushSecond()
    {

    }
}