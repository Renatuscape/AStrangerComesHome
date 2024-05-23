using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTimerUpdater : MonoBehaviour
{
    public NightSkyController nightSkyController;
    public GardenManager gardenManager;

    public float animationTimer;
    float animationTick = 0.15f;
    public float threeSecondTimer;
    float threeSecondTick = 3;

    private void Start()
    {
        animationTimer = animationTick;
        threeSecondTimer = threeSecondTick;
    }
    void Update()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            animationTimer += Time.deltaTime;
            threeSecondTimer += Time.deltaTime;

            if (animationTimer >= animationTick)
            {
                animationTimer = 0;
                PushAnimation();
            }

            if (threeSecondTimer >= threeSecondTick)
            {
                threeSecondTimer = 0;
                PushThreeSeconds();
            }

            PushEveryFrame();
        }
    }

    void PushEveryFrame()
    {
        nightSkyController.GlobalPushStarFlicker();
    }
    void PushAnimation()
    {
        nightSkyController.GlobalPushRefreshSky();
    }

    void PushThreeSeconds()
    {
        gardenManager.GlobalPushGrow();
    }
}