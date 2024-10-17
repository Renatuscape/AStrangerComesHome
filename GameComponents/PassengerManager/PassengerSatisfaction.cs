using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerSatisfaction : MonoBehaviour
{
    public static PassengerSatisfaction instance;
    public static bool isEnabled;
    public DataManagerScript dataManager;
    public CoachFoodStock coachStock;

    private void Start()
    {
        instance = this;

        PassengerFoodManager.Setup();
    }

    public static void CheckUnlockState()
    {
        if (Player.GetCount(StaticTags.OnBoardService, "PassengerSatisfaction") > 0)
        {
            instance.Activate();
        }
        else
        {
            instance.Deactivate();
        }
    }

    void Activate()
    {
        coachStock.Setup();
        isEnabled = true;
    }
    void Deactivate()
    {
        coachStock.DisableStock();
        isEnabled = false;
    }

    void Daily()
    {
        if (isEnabled)
        {
            if (dataManager.seatA != null && dataManager.seatA.isActive)
            {
                ReduceSatisfaction(dataManager.seatA);
            }

            if (dataManager.seatB != null && dataManager.seatB.isActive)
            {
                ReduceSatisfaction(dataManager.seatB);
            }
        }
    }

    void Hourly()
    {
        if (isEnabled)
        {
            PassengerFoodManager.Hourly();
        }
    }

    void ReduceSatisfaction(PassengerData passenger)
    {
        passenger.isFedToday = false;

        if (passenger.satisfaction > 0)
        {
            passenger.satisfaction--;
        }
    }

    public static void DailyTick()
    {
        if (instance != null)
        {
            instance.Daily();
        }
    }

    public static void HourlyTick()
    {
        if (instance != null)
        {
            instance.Hourly();
        }
    }

    public static void ForceUpdateViableInventory()
    {
        if (instance != null)
        {
            PassengerFoodManager.FindViableFoodItems();
        }
    }
}