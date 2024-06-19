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

    float secondTick = 1;
    public float secondTimer;
    bool isRunning = false;

    public void Initialise()
    {
        animationTimer = animationTick;
        threeSecondTimer = threeSecondTick;
        secondTimer = secondTick;
        isRunning = true;
    }
    void Update()
    {
        if (isRunning && TransientDataScript.IsTimeFlowing())
        {
            animationTimer += Time.deltaTime;
            threeSecondTimer += Time.deltaTime;
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

    void PushSecond()
    {
        ManaConverter.GlobalPushManaRegen();
        PassengerManager.GlobalPushPassengerSpawn();
    }
    void PushThreeSeconds()
    {
        gardenManager.GlobalPushGrow();

        UpgradeWearTracker.GlobalPushWearUpgrade();
    }
}