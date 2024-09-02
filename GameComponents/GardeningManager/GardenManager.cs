using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

//HANDLES GARDEN PLANTER SETUP
public class GardenManager : MonoBehaviour
{
    public DataManagerScript dataManager;
    public GameObject confirmPlantingMenu;

    public int creation;
    public int gardening; // improves growth for all plants
    public int cultivation; // improves survival (within health)
    public int epistemology; // improves yield chances
    public int goetia; // improves survival by up to 20%. At level 5, this mysterious skill grants all plants an extra life. At level 10, two extra lives.
    public int unlockedPlanters;
    public bool plantersChecked;

    public float growthTimer;
    public float growthTick = 1;

    public List<Sprite> planterSprites;

    public float growth = 1.5f;

    private void Awake()
    {
        confirmPlantingMenu.SetActive(false);
    }

    void SyncSkills()
    {
        creation = Player.GetCount(StaticTags.Creation, "GardenManager");
        gardening = Player.GetCount(StaticTags.Gardening, "GardenManager");
        cultivation = Player.GetCount(StaticTags.Cultivation, "GardenManager");
        epistemology = Player.GetCount(StaticTags.Epistemology, "GardenManager");
        goetia = Player.GetCount(StaticTags.Goetia, "GardenManager");

        CheckPlanters();
    }

    void CheckPlanters()
    {
        UpdatePlanterData();
        plantersChecked = true;
    }

    //PlanterData CreatePlanter(string planterID)
    //{
    //    PlanterData newPlanter = new PlanterData();
    //    newPlanter.planterID = planterID;
    //    newPlanter.planterSpriteID = planterSprites[0].name;
    //    dataManager.planters.Add(newPlanter);

    //    return newPlanter;
    //}

    public void UpdatePlanterSprite()
    {
        TransientDataScript.gameManager.coachPlanters.CheckUnlockedPlanters();
    }

    public void GlobalPushGrow()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            GrowthTick();
        }
    }

    void GrowthTick()
    {
        if (!plantersChecked)
        {
            SyncSkills();
        }
        else
        {
            plantersChecked = false;
        }

        foreach (var planter in dataManager.planters)
        {
            if (planter.isActive && planter.progress < planter.GetMaxGrowth())
            {
                growth = CalculateGrowth(planter);

                planter.progress += growth;
            }
        }

        UpdatePlanterData();
    }

    void UpdatePlanterData()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            TransientDataScript.gameManager.coachPlanters.CheckUnlockedPlanters();
        }
    }

    public void ClickPlanter(PlanterData planterData)
    {

        if (TransientDataScript.IsTimeFlowing() && TransientDataScript.CameraView == CameraView.Garden)
        {
            SyncSkills();
            ProcessPlanterClick(planterData);
            UpdatePlanterData();
        }
    }

    void ProcessPlanterClick(PlanterData planterData)
    {
        Item seed = planterData.seed;

        //IF THE PLANTER IS EMPTY
        if (!planterData.isActive)
        {
            confirmPlantingMenu.SetActive(true);
        }

        //IF THERE IS A SEED AND THE PLANTER IS ACTIVE
        else if (seed != null && planterData.isActive)
        {
            Item outputPlant = seed.GetOutput();

            //IF THE PLANTER IS GROWING BUT NOT FINISHED
            if (planterData.progress < planterData.GetMaxGrowth())
            {
                PrintPlantInfo(planterData);
            }
            else //IF THE PLANTER IS READY TO BE HARVESTED
            {
                var plantInInventory = Player.GetCount(outputPlant.objectID, name);

                if (plantInInventory + seed.yield <= outputPlant.maxValue)
                {
                    Debug.Log($"{seed.name} is ready! {planterData.progress}/{planterData.GetMaxGrowth()}");

                    //PLANT YIELD - EPISTEMOLOGY
                    //Plants with only one yield by default can only get a higher yield through the Goetia skill
                    var yield = seed.yield;

                    if (seed.yield > 1)
                    {
                        yield = 1;

                        var failChance = 85 - (epistemology * 5) - (gardening * 2);

                        for (int i = 0; i < yield; i++)
                        {
                            if (Random.Range(0, 100) > failChance)
                            {
                                yield++;
                            }

                            if (yield >= seed.yield)
                            {
                                break;
                            }
                        }
                    }

                    // BONUS YIELD FROM GOETIA
                    var toAdd = yield + Mathf.CeilToInt(goetia * 0.2f);

                    if (toAdd < 1)
                    {
                        Debug.LogWarning("Yield was less than one for " + seed.name);
                        toAdd = 1;
                    }

                    Player.Add(outputPlant.objectID, toAdd); //Bonus 1 or 2 yield from Earthsoul mythical skill

                    // BONUS HEALTH FROM GOETIA
                    var rollForHealth = Random.Range(0, 100);

                    if (rollForHealth < goetia * 1.5f)
                    {
                        LogAlert.QueueTextAlert($"A mystical force has revitalised this {outputPlant.name}!");
                    }
                    else
                    {
                        planterData.seedHealth--;
                    }

                    //CHECK IF PLANT IS DEAD
                    if (planterData.seedHealth > 0)
                    {
                        planterData.progress = planterData.GetMaxGrowth() / 3;
                    }

                    else if (planterData.seedHealth <= 0)
                    {
                        planterData.progress = 0;
                        planterData.isActive = false;
                    }
                }
                else
                {
                    LogAlert.QueueTextAlert($"I don't have space for {outputPlant.name}.");
                }
            }
        }
        else if (seed == null && planterData.isActive)
        {
            //Check planters for bugs
            planterData.isActive = false;
        }
    }

    float CalculateGrowth(PlanterData planter)
    {
        float amount = 1f + (gardening * 0.3f) + (creation * 0.2f) + (cultivation * 0.5f);

        if (planter.weeds > 0)
        {
            amount = amount / planter.weeds;
        }

        return amount;
    }

    void PrintPlantInfo(PlanterData planterData)
    {
        Debug.Log($"{planterData.seed.name} is growing. {planterData.progress}/{planterData.GetMaxGrowth()}");

        growth = CalculateGrowth(planterData);

        string growthRate = "a poor";

        if (growth >= 7.3)
        {
            growthRate = "a brilliant";
        }
        else if (growth >= 6)
        {
            growthRate = "a fantastic";
        }
        else if (growth >= 4)
        {
            growthRate = "a great";
        }
        else if (growth >= 2.5)
        {
            growthRate = "a good";
        }
        else if (growth >= 1.5)
        {
            growthRate = "an average";
        }

        string growthState = "was just planted.";

        if (planterData.progress > 0)
        {
            float growthPercentage = planterData.progress / planterData.GetMaxGrowth() * 100;

            if (growthPercentage >= 90)
            {
                growthState = "is almost ready!";
            }
            else if (growthPercentage >= 70)
            {
                growthState = "needs a little more time.";
            }
            else if (growthPercentage >= 50)
            {
                growthState = "is making good progress.";
            }
            else if (growthPercentage >= 25)
            {
                growthState = "is showing promise.";
            }
            else if (growthPercentage >= 1)
            {
                growthState = "has started sprouting.";
            }
        }

        string nameToPrint = planterData.seed.name;

        if (nameToPrint.Contains(" Seed"))
        {
            nameToPrint = nameToPrint.Replace(" Seed", "");
        }
        else if (nameToPrint.Contains(" Sprout"))
        {
            nameToPrint = nameToPrint.Replace(" Sprout", "");
        }
        else if (nameToPrint.Contains(" Sapling"))
        {
            nameToPrint = nameToPrint.Replace(" Sapling", "");
        }

        LogAlert.QueueTextAlert($"This {nameToPrint} {growthState} It is growing at {growthRate} pace and has {planterData.seedHealth} health.");
    }
}
