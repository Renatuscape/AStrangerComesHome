using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class GardeningPlanterPackage
{
    public PlanterData planterData;
    public SpriteRenderer plantSprite;
    public SpriteRenderer worldPlanter;
}

//HANDLES GARDEN PLANTER SETUP
public class GardenManager : MonoBehaviour
{
    public DataManagerScript dataManager;
    public GameObject confirmPlantingMenu;

    public SpriteRenderer planterA;
    public SpriteRenderer plantSpriteA;
    public SpriteRenderer planterB;
    public SpriteRenderer plantSpriteB;
    public SpriteRenderer planterC;
    public SpriteRenderer plantSpriteC;
    public List<GardeningPlanterPackage> planterPackages = new();

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
        CheckPlanters();
        confirmPlantingMenu.SetActive(false);
        SyncSkills();
    }

    private void OnEnable()
    {
        SyncSkills();
        UpdatePlanterSprite();
        plantSpriteA.sprite = null;
        plantSpriteB.sprite = null;
        plantSpriteC.sprite = null;
        GrowthTick();
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
        // ENSURE PLANTER DATA EXISTS
        unlockedPlanters = Player.GetCount(StaticTags.UnlockedPlanters, name);

        if (unlockedPlanters >= 1)
        {
            SetUpPlanter("planterA", plantSpriteA, planterA);
        }
        if (unlockedPlanters >= 2)
        {
            SetUpPlanter("planterB", plantSpriteB, planterB);
        }
        if (unlockedPlanters >= 3)
        {
            SetUpPlanter("planterC", plantSpriteC, planterC);
        }
        if (unlockedPlanters < 1)
        {
            planterA.gameObject.SetActive(false);
            planterB.gameObject.SetActive(false);
            planterC.gameObject.SetActive(false);

            if (dataManager.planters != null && dataManager.planters.Count > 0)
            {
                dataManager.planters.Clear();
            }
        }

        plantersChecked = true;
    }

    PlanterData CreatePlanter(string planterID)
    {
        PlanterData newPlanter = new PlanterData();
        newPlanter.planterID = planterID;
        newPlanter.planterSpriteID = planterSprites[0].name;
        dataManager.planters.Add(newPlanter);

        return newPlanter;
    }

    void SetUpPlanter(string planterDataID, SpriteRenderer plantSprite, SpriteRenderer worldPlanter)
    {
        if (dataManager.planters == null)
        {
            dataManager.planters = new();
        }

        var planterData = dataManager.planters.FirstOrDefault(p => p.planterID == planterDataID);

        if (planterData == null)
        {
            planterData = CreatePlanter(planterDataID);
        }

        var package = planterPackages.FirstOrDefault(p => p.planterData.planterID == planterData.planterID);

        if (package == null)
        {
            package = new();
            package.planterData = planterData;
            planterPackages.Add(package);
        }

        package.worldPlanter = worldPlanter;
        package.plantSprite = plantSprite;
        package.worldPlanter.gameObject.SetActive(true);
    }

    public void UpdatePlanterSprite()
    {
        if (planterSprites.Count >= 1)
        {
            foreach (var planter in planterPackages)
            {
                planter.worldPlanter.sprite = planterSprites.FirstOrDefault(s => s.name.Contains(planter.planterData.planterSpriteID));
            }
        }
    }

    public void GlobalPushGrow()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            GrowthTick();

            foreach (var planter in planterPackages)
            {
                if (!planter.planterData.isActive || planter.planterData.seed == null)
                {
                    planter.plantSprite = null;
                }
            }
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

        if (plantersChecked && unlockedPlanters > 0)
        {
            foreach(var planter in planterPackages)
            {
                if (planter.planterData.isActive && planter.planterData.seed != null)
                {
                    UpdatePlanterData(planter);
                }
            }
        }
    }

    float CalculateGrowth(PlanterData planter)
    {
        float amount = 1.5f + (gardening * 0.2f) + (creation * 0.2f) + (goetia * 0.2f);

        if (planter.weeds > 0)
        {
            amount = amount / planter.weeds;
        }

        return amount;
    }
    void UpdatePlanterData(GardeningPlanterPackage planter)
    {
        var seed = planter.planterData.seed;
        ref var progress = ref planter.planterData.progress;

        var maxGrowth = 100 * seed.health * seed.yield;

        if (progress <= maxGrowth)
        {
            growth = CalculateGrowth(planter.planterData);

            progress += growth;

            if (progress < maxGrowth * 0.3f)
            {
                planter.plantSprite.sprite = SpriteFactory.GetSprout(seed.objectID, 1);
            }
            else if (progress > maxGrowth * 0.3f && progress < maxGrowth * 0.6f)
            {
                planter.plantSprite.sprite = SpriteFactory.GetSprout(seed.objectID, 2);
            }
            else if (progress > maxGrowth * 0.6f)
            {
                planter.plantSprite.sprite = SpriteFactory.GetSprout(seed.objectID, 3);
            }
        }
        else
        {
            planter.plantSprite.sprite = seed.GetOutput().sprite;
        }
    }

    public void ClickPlanter(PlanterData planterData)
    {

        if (TransientDataScript.IsTimeFlowing() && TransientDataScript.CameraView == CameraView.Garden)
        {
            SyncSkills();

            GardeningPlanterPackage package = planterPackages.FirstOrDefault(p => p.planterData.planterID == planterData.planterID);

            if (package != null)
            {
                ProcessPlanterClick(package);
            }
        }
    }

    private void ProcessPlanterClick(GardeningPlanterPackage package)
    {
        Item seed = package.planterData.seed;
        PlanterData planterData = package.planterData;

        //IF THE PLANTER IS EMPTY
        if (!planterData.isActive)
        {
            confirmPlantingMenu.SetActive(true);
        }

        //IF THERE IS A SEED AND THE PLANTER IS ACTIVE
        else if (seed != null && planterData.isActive)
        {
            int maxGrowth = 100 * seed.health * seed.yield;
            Item outputPlant = seed.GetOutput();
            float deathLevel = 0;

            if (seed.health > 1)
            {
                deathLevel -= Mathf.Floor(goetia * 0.2f);
            }
            //IF THE PLANTER IS GROWING BUT NOT FINISHED
            if (planterData.progress < maxGrowth && planterData.isActive)
            {
                Debug.Log($"{seed.name} is growing. {planterData.progress}/{maxGrowth}");

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
                    float growthPercentage = planterData.progress / maxGrowth * 100;

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


                // Display info for the plant currently growing in this planter
                LogAlert.QueueTextAlert($"This {outputPlant.name} {growthState} It is growing at {growthRate} pace.");
            }

            //IF THE PLANTER IS READY TO BE HARVESTED
            if (planterData.progress >= maxGrowth)
            {
                var plantInInventory = Player.GetCount(outputPlant.objectID, name);

                if (plantInInventory + seed.yield <= outputPlant.maxValue)
                {
                    Debug.Log($"{seed.name} is ready! {planterData.progress}/{maxGrowth}");

                    //PLANT YIELD
                    var yield = seed.yield;

                    if (seed.yield > 1) //Plants with only one yield by default can only get a higher yield through the Earthsoul skill
                    {
                        yield = 1;

                        var failChance = 90 - (epistemology * 5) - (gardening * 3);

                        for (int i  = 0; i < yield; i++)
                        {
                            if (Random.Range(0, 100) > failChance)
                            {
                                yield++;
                            }
                        }
                    }

                    var toAdd = yield + (int)Mathf.Ceil(goetia * 0.2f);

                    if (toAdd < 1)
                    {
                        Debug.LogWarning("Yield was less than one for " + seed.name);
                        toAdd = 1;
                    }
                    Player.Add(outputPlant.objectID, toAdd); //Bonus 1 or 2 yield from Earthsoul mythical skill

                    //PLANT HEALTH
                    var rollForHealth = Random.Range(0, 100);
                    var survivalChance = 90 - (cultivation * 5) - (gardening * 2);

                    if (rollForHealth < goetia * 2)
                    {
                        LogAlert.QueueTextAlert($"A mystical force has vitalised this {outputPlant.name}!");
                    }
                    else if (seed.health > 1 && rollForHealth < survivalChance)
                    {
                        planterData.seedHealth = 0;
                    }
                    else
                    {
                        planterData.seedHealth--;
                    }

                    //CHECK IF PLANT IS DEAD
                    if (planterData.seedHealth > deathLevel)
                    {
                        planterData.progress = maxGrowth / 3;
                        UpdatePlanterData(package);
                    }

                    else if (planterData.seedHealth <= deathLevel)
                    {
                        planterData.progress = 0;
                        planterData.isActive = false;
                        package.plantSprite.sprite = null;
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
}
