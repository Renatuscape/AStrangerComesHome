using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public TransientDataScript transientData;

    public int engineBoostEfficiency;
    public int engineFuelEfficiency;
    public int engineBoostMax;
    public int engineClickPotency;

    float baseSpeed = 0.5f;
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

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();

        transientData.engineState = EngineState.Off;
        transientData.currentSpeed = 0;
        currentBoost = 0;
        InvokeRepeating("SpeedManager", 0, 0.01f);
        InvokeRepeating("BoostDecrease", 0, 0.02f);
        InvokeRepeating("ManaConsumption", 0, 0.02f);
    }

    private void Start()
    {
        boostMax = 50 + (5 * engineBoostMax); //Calculated here first, then whenever OnMouseDown is called
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
            manaConsumptionDebuff = 1f + (10 - engineFuelEfficiency) / 30;

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

    void Update()
    {
        if (TransientDataScript.GameState == GameState.Overworld || TransientDataScript.GameState == GameState.MapMenu) //Keys only work during these states
        {
            //KEY INPUT LISTENERS
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SyncUpgrades();

                if (transientData.engineState != EngineState.FirstGear)
                    transientData.engineState = EngineState.FirstGear;
                else
                    transientData.engineState = EngineState.Off;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (transientData.engineState != EngineState.SecondGear)
                    transientData.engineState = EngineState.SecondGear;
                else
                    transientData.engineState = EngineState.Off;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SyncUpgrades();

                if (transientData.engineState != EngineState.ThirdGear)
                    transientData.engineState = EngineState.ThirdGear;
                else
                    transientData.engineState = EngineState.Off;
            }
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Backspace))
            {
                if (transientData.engineState == EngineState.Off)
                    transientData.engineState = EngineState.Reverse;
                else
                    transientData.engineState = EngineState.Off;
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
            {
                SyncUpgrades();

                if (transientData.engineState != EngineState.Off)
                    transientData.engineState = EngineState.Off;
                else
                    transientData.engineState = EngineState.FirstGear;
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
}