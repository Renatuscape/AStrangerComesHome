using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public TransientDataScript transientData;

    public int engineBoostEfficiency;
    public int engineFuelEfficiency;
    public int engineBoostMax;
    public int engineClickPotency;

    float baseSpeed = 0.6f;
    float baseFuelConsumption = 0.01f;

    float secondGearMultiplier = 2.5f;
    float thirdGearMultiplier = 3.5f;
    float reverseMultiplier = 0.8f;

    float speedDecreaseRate = 0.03f; //Cushion speed decrease
    float speedIncreaseRate = 0.015f; //Rate at which speed is increased when changing gear

    public float currentBoost;
    public float boostDecrease;
    public float boostMax;
    public float manaConsumptionDebuff;
    public float targetSpeed;

    public float speedTimer;
    public float effectTimer;
    public float speedTick;
    public float effectTick;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();

        transientData.engineState = EngineState.Off;
        transientData.currentSpeed = 0;
        currentBoost = 0;
        speedTick = 0.01f;
        effectTick = 0.025f;
    }

    private void Start()
    {
        boostMax = 50 + (3 * engineBoostMax); //Calculated here first, then whenever OnMouseDown is called
    }

    private void OnEnable()
    {
        SyncUpgrades();
    }

    void SyncUpgrades()
    {
        engineBoostEfficiency = Player.GetCount("MEC000", "Engine"); //Capacitor - boost depletes slower
        engineBoostMax = Player.GetCount("MEC001", "Engine"); //Brass Chamber - more boost can be stored
        engineClickPotency = Player.GetCount("MEC002", "Engine"); //Crankshaft - click is more potent
        engineFuelEfficiency = Player.GetCount("MEC003", "Engine"); //Spark Tubes - engine uses less mana
    }

    void Update()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            speedTimer += Time.deltaTime;
            effectTimer += Time.deltaTime;

            if (speedTimer > speedTick)
            {
                speedTimer = 0;
                SpeedManager();
            }

            if (effectTimer > effectTick)
            {
                effectTimer = 0;
                ManaConsumption();
                BoostDecrease();
            }
        }
    }

    public void OnMouseDown()
    {
        if (TransientDataScript.GameState == GameState.Overworld || TransientDataScript.GameState == GameState.ShopMenu)
        {
            boostMax = 100 + (20 * engineBoostMax);

            if (TransientDataScript.GameState == GameState.Overworld && currentBoost < boostMax)
                currentBoost = currentBoost + (5 + (0.5f * engineClickPotency));
        }
    }

    void BoostDecrease()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            if (currentBoost > 0)
            {
                boostDecrease = 0.3f - (0.02f * engineBoostEfficiency);
                currentBoost = currentBoost - boostDecrease;
            }
            else if (currentBoost < 0)
                currentBoost = 0;

            if (currentBoost > boostMax)
                currentBoost = boostMax;
        }
    }
    void SpeedManager()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            if (TransientDataScript.GameState != GameState.Overworld &&
                TransientDataScript.GameState != GameState.MapMenu &&
                TransientDataScript.GameState != GameState.JournalMenu &&
                TransientDataScript.GameState != GameState.StartMenu)
            {
                transientData.engineState = EngineState.Off;
            }

            //SET TARGET SPEED
            switch (transientData.engineState)
            {
                case EngineState.FirstGear:
                    targetSpeed = baseSpeed + (currentBoost / 40);
                    break;

                case EngineState.SecondGear:
                    targetSpeed = (baseSpeed * secondGearMultiplier) + (currentBoost / 50);
                    break;

                case EngineState.ThirdGear:
                    targetSpeed = (baseSpeed * thirdGearMultiplier) + (currentBoost / 50);
                    break;

                case EngineState.Reverse:
                    targetSpeed = 0 - ((baseSpeed * reverseMultiplier) + (currentBoost / 70)) / 2;
                    break;

                default:
                    targetSpeed = 0;
                    break;
            }

            //INCREASE/DECREASE CURRENT SPEED
            if (transientData.currentSpeed < targetSpeed)
                transientData.currentSpeed += speedIncreaseRate;
            if (transientData.currentSpeed > targetSpeed)
                transientData.currentSpeed -= speedDecreaseRate;

            //ENSURE STOP
            if (transientData.engineState == EngineState.Off)
            {
                if (transientData.currentSpeed >= -0.1f && transientData.currentSpeed <= 0.1f)
                    transientData.currentSpeed = 0f;
            }
        }
    }

    void ManaConsumption()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            manaConsumptionDebuff = 2f + (10 - engineFuelEfficiency) / 30;

            //SWITCH TO FIRST GEAR BEFORE MANA RUNS OUT
            if (transientData.currentMana < 10 && transientData.engineState != EngineState.Off && transientData.engineState != EngineState.Reverse)
            {
                transientData.engineState = EngineState.FirstGear;
            }

            //TURN THE ENGINE OFF IF THERE IS NO MANA
            if (transientData.currentMana <= 0)
                transientData.engineState = EngineState.Off;

            switch (transientData.engineState)
            {
                case EngineState.FirstGear:
                    transientData.currentMana = transientData.currentMana - baseFuelConsumption * manaConsumptionDebuff;
                    break;
                case EngineState.SecondGear:
                    transientData.currentMana = transientData.currentMana - baseFuelConsumption * (5 * manaConsumptionDebuff);
                    break;
                case EngineState.ThirdGear:
                    transientData.currentMana = transientData.currentMana - baseFuelConsumption * (10 * manaConsumptionDebuff);
                    break;
                case EngineState.Reverse:
                    transientData.currentMana = transientData.currentMana - baseFuelConsumption * (manaConsumptionDebuff * 0.5f);
                    break;
                default:
                    break;
            }
        }
    }
}