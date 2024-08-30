using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GardenCoachPlanters : MonoBehaviour
{
    public GardenManager gardenManager;
    public DataManagerScript dataManager;
    public GardenPlanter planterA;
    public GardenPlanter planterB;
    public GardenPlanter planterC;

    public void Setup()
    {
        var planterDataA = dataManager.planters.FirstOrDefault(p => p.planterID == "planterA");
        if (planterDataA == null || string.IsNullOrEmpty(planterDataA.planterID))
        {
            planterDataA = new() { planterID = "planterA" };
            dataManager.planters.Add(planterDataA);
        }
        planterA.planterData = planterDataA;

        var planterDataB = dataManager.planters.FirstOrDefault(p => p.planterID == "planterB");

        if (planterDataB == null || string.IsNullOrEmpty(planterDataB.planterID))
        {
            planterDataB = new() { planterID = "planterB" };
            dataManager.planters.Add(planterDataB);
        }
        planterB.planterData = planterDataB;

        var planterDataC = dataManager.planters.FirstOrDefault(p => p.planterID == "planterC");
        if (planterDataC == null || string.IsNullOrEmpty(planterDataC.planterID))
        {
            planterDataC = new() { planterID = "planterC" };
            dataManager.planters.Add(planterDataC);
        }
        planterC.planterData = planterDataC;

        CheckUnlockedPlanters();
    }

    public void CheckUnlockedPlanters()
    {
        int unlockedCoachPlanters = Player.GetCount(StaticTags.UnlockedPlanters, name);

        if (unlockedCoachPlanters < 3)
        {
            planterC.HidePlanter();
        }
        else
        {
            planterC.UpdateFromPlanterData();
        }

        if (unlockedCoachPlanters < 2)
        {
            planterB.HidePlanter();
        }
        else
        {
            planterB.UpdateFromPlanterData();
        }

        if (unlockedCoachPlanters < 1)
        {
            planterA.HidePlanter();
        }
        else
        {
            planterA.UpdateFromPlanterData();
        }
    }

    public void ClickPlanter(PlanterData planterData)
    {
        CheckUnlockedPlanters();
        gardenManager.ClickPlanter(planterData);
    }
}
