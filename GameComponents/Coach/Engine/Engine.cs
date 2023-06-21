using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public TransientDataScript transientData;

    public MotherObject engineBoostEfficiency;
    public MotherObject engineFuelEfficiency;
    public MotherObject engineBoostMax;
    public MotherObject engineClickPotency;

    float baseSpeed = 2f;
    float baseFuelConsumption = 0.01f;

    float secondGearMultiplier = 2f;
    float thirdGearMultiplier = 3f;
    float reverseMultiplier = 0.4f;

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
        boostMax = 50 + (5 * engineBoostMax.dataValue); //Calculated here first, then whenever OnMouseDown is called
    }

    void BoostDecrease()
    {
        if (transientData.gameState == GameState.Overworld || transientData.gameState == GameState.ShopMenu || transientData.gameState == GameState.Dialogue || transientData.gameState == GameState.PlayerHome || transientData.gameState == GameState.MapMenu)
        {
            if (currentBoost > 0)
            {
                boostDecrease = 0.3f - (0.02f * engineBoostEfficiency.dataValue);
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
        if (transientData.gameState == GameState.Overworld || transientData.gameState == GameState.ShopMenu || transientData.gameState == GameState.Dialogue || transientData.gameState == GameState.PlayerHome || transientData.gameState == GameState.MapMenu)
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
        if (transientData.gameState == GameState.Overworld || transientData.gameState == GameState.ShopMenu || transientData.gameState == GameState.Dialogue || transientData.gameState == GameState.PlayerHome || transientData.gameState == GameState.MapMenu)
        {
            manaConsumptionDebuff = 1f + (10 - engineFuelEfficiency.dataValue) / 30;

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
        if (transientData.gameState == GameState.Overworld || transientData.gameState == GameState.MapMenu) //Keys only work during these states
        {
            //KEY INPUT LISTENERS
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
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
                if (transientData.engineState != EngineState.Off)
                    transientData.engineState = EngineState.Off;
                else
                    transientData.engineState = EngineState.FirstGear;
            }
        }
    }

    public void OnMouseDown()
    {
        if (transientData.gameState == GameState.Overworld || transientData.gameState == GameState.ShopMenu)
        {
            boostMax = 100 + (20 * engineBoostMax.dataValue);

            if (transientData.gameState == GameState.Overworld && currentBoost < boostMax)
                currentBoost = currentBoost + (5 + (0.5f * engineClickPotency.dataValue));
        }
    }
}
