using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//HANDLES PLANT GROWTH AND HARVESTING
public enum WhichPlanter
{
    PlanterA,
    PlanterB,
    PlanterC
}

public class GardenManager : MonoBehaviour
{
    public DataManagerScript dataManager;
    public TransientDataScript transientData;
    public GameObject confirmPlantingMenu;

    public SpriteRenderer planterA;
    public SpriteRenderer plantSpriteA;
    public SpriteRenderer planterB;
    public SpriteRenderer plantSpriteB;
    public SpriteRenderer planterC;
    public SpriteRenderer plantSpriteC;

    public int gardening; //improves growth for all plants
    public int cultivation; //improves yield rate for multi-yield plants
    public int nurturing; //creates a small chance that plant health does not decrease
    public int earthsoul; //at level 5, this mysterious skill grants all plants an extra life. At level 10, two extra lives.
    public int unlockedPlanters;
    bool plantersChecked;

    public float growthTimer;
    public float growthTick = 1;

    public List<Sprite> planterSprites;

    public int gardeningLevel; //improves growth for all plants
    public int nurturingLevel; //improves yield rate for multi-yield plants
    public int cultivationLevel; //creates a small chance that plant health does not decrease
    public int earthsoulLevel; //at level 5, this mysterious skill grants all plants an extra life. At level 10, two extra lives.

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
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
        gardening = Player.GetCount("GAR000", "GardenManager");
        cultivation = Player.GetCount("GAR001", "GardenManager");
        nurturing = Player.GetCount("GAR002", "GardenManager");
        earthsoul = Player.GetCount("GAR003", "GardenManager");

        CheckPlanters();
    }

    void CheckPlanters()
    {
        unlockedPlanters = Player.GetCount("SCR004-SCR-NN", name);

        if (unlockedPlanters >= 1)
        {
            planterA.gameObject.SetActive(true);
        }
        if (unlockedPlanters >= 2)
        {
            planterB.gameObject.SetActive(true);
        }
        if (unlockedPlanters >= 3)
        {
            planterC.gameObject.SetActive(true);
        }
        if (unlockedPlanters < 1)
        {
            planterA.gameObject.SetActive(false);
            planterB.gameObject.SetActive(false);
            planterC.gameObject.SetActive(false);
        }
        plantersChecked = true;
    }
    public void UpdatePlanterSprite()
    {
        if (planterSprites.Count >= 1)
        {
            planterA.sprite = planterSprites[dataManager.planterSpriteA];
            planterB.sprite = planterSprites[dataManager.planterSpriteB];
            planterC.sprite = planterSprites[dataManager.planterSpriteC];
        }
    }

    private void Update()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            growthTimer += Time.fixedDeltaTime; //add skill adjustments

            if (growthTimer >= growthTick)
            {
                GrowthTick();
                growthTimer = 0;
            }

            if (!dataManager.planterIsActiveA || dataManager.seedA == null)
            {
                plantSpriteA.sprite = null;
            }
            if (!dataManager.planterIsActiveB || dataManager.seedB == null)
            {
                plantSpriteB.sprite = null;
            }
            if (!dataManager.planterIsActiveC || dataManager.seedC == null)
            {
                plantSpriteC.sprite = null;
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

        if (unlockedPlanters > 0)
        {
            if (dataManager.planterIsActiveA && !string.IsNullOrEmpty(dataManager.seedA))
                UpdatePlanterData(ref plantSpriteA, ref dataManager.seedA, ref dataManager.progressSeedA);

            if (unlockedPlanters > 1 && dataManager.planterIsActiveB && !string.IsNullOrEmpty(dataManager.seedB))
                UpdatePlanterData(ref plantSpriteB, ref dataManager.seedB, ref dataManager.progressSeedB);

            if (unlockedPlanters > 2 && dataManager.planterIsActiveC && !string.IsNullOrEmpty(dataManager.seedC))
                UpdatePlanterData(ref plantSpriteC, ref dataManager.seedC, ref dataManager.progressSeedC);
        }
    }

    void UpdatePlanterData(ref SpriteRenderer plantSprite, ref string seedID, ref float progressSeed)
    {
        var seed = Items.FindByID(seedID);

        if (seed == null)
        {
            Debug.LogWarning("Seed not found: " + seedID);
        }

        else
        {
            var maxGrowth = 100 * seed.health * seed.yield;

            if (progressSeed <= maxGrowth)
            {
                progressSeed += 1 + (gardening * 0.2f); // GARDENING SKILL INCREASES GROWTH PER TICK

                if (progressSeed < maxGrowth * 0.3f)
                {
                    plantSprite.sprite = seed.stage1;
                }
                else if (progressSeed > maxGrowth * 0.3f && progressSeed < maxGrowth * 0.6f)
                {
                    plantSprite.sprite = seed.stage2;
                }
                else if (progressSeed > maxGrowth * 0.6f)
                {
                    plantSprite.sprite = seed.stage3;
                }
            }
            else
            {
                plantSprite.sprite = seed.GetOutput().sprite;
            }
        }

    }

    public void ClickPlanter(WhichPlanter planter)
    {
        SyncSkills();

        if (TransientDataScript.CameraView == CameraView.Garden)
        {
            if (dataManager.planterIsActiveA && dataManager.seedA == null)
            {
                dataManager.planterIsActiveA = false;
                Debug.Log("SeedA was missing. Planter deactivated.");
            }
            if (dataManager.planterIsActiveB && dataManager.seedB == null)
            {
                dataManager.planterIsActiveB = false;
                Debug.Log("SeedB was missing. Planter deactivated.");
            }
            if (dataManager.planterIsActiveC && dataManager.seedC == null)
            {
                dataManager.planterIsActiveC = false;
                Debug.Log("SeedC was missing. Planter deactivated.");
            }

            switch (planter)
            {
                case WhichPlanter.PlanterA:
                    ProcessPlanterClick(ref dataManager.planterIsActiveA, ref dataManager.progressSeedA, ref dataManager.seedA, ref dataManager.seedHealthA, plantSpriteA);
                    break;
                case WhichPlanter.PlanterB:
                    ProcessPlanterClick(ref dataManager.planterIsActiveB, ref dataManager.progressSeedB, ref dataManager.seedB, ref dataManager.seedHealthB, plantSpriteB);
                    break;
                case WhichPlanter.PlanterC:
                    ProcessPlanterClick(ref dataManager.planterIsActiveC, ref dataManager.progressSeedC, ref dataManager.seedC, ref dataManager.seedHealthC, plantSpriteC);
                    break;
                default:
                    break;
            }
        }
    }

    private void ProcessPlanterClick(ref bool planterIsActive, ref float growthProgress, ref string seedID, ref int seedHealth, SpriteRenderer plantRenderer)
    {
        Item seed;

        if (!string.IsNullOrEmpty(seedID))
        {
            seed = Items.FindByID(seedID);
        }
        else
        {
            seed = null;
        }

        Debug.Log($"Processinng click. Planter is active {planterIsActive}, growth progress {growthProgress}, seed ID {seedID}, seed object: {(seed != null ? seed.name : "null")}");

        //IF THE PLANTER IS EMPTY
        if (!planterIsActive)
        {
            confirmPlantingMenu.SetActive(true);
        }

        //IF THERE IS A SEED AND THE PLANTER IS ACTIVE
        else if (seed != null && planterIsActive)
        {
            var maxGrowth = 100 * seed.health * seed.yield;
            var outputPlant = seed.GetOutput();

            //IF THE PLANTER IS GROWING BUT NOT FINISHED
            if (growthProgress < maxGrowth && planterIsActive)
            {
                Debug.Log($"{seed.name} is growing. {growthProgress}/{maxGrowth}");

                string growthRate = "decent";
                if (gardening < 3)
                    growthRate = "normal";
                else if (gardening > 6)
                    growthRate = "great";

                // Display info for the plant currently growing in this planter
                TransientDataScript.PushAlert($"This {outputPlant.name} is growing at a {growthRate} pace.");
            }

            //IF THE PLANTER IS READY TO BE HARVESTED
            if (growthProgress >= maxGrowth)
            {
                var plantInInventory = Player.GetCount(outputPlant.objectID, name);

                if (plantInInventory + seed.yield <= outputPlant.maxValue)
                {
                    Debug.Log($"{seed.name} is ready! {growthProgress}/{maxGrowth}");

                    //PLANT YIELD
                    var yield = seed.yield;

                    if (seed.yield > 1) //Plants with only one yield by default can only get a higher yield through the Earthsoul skill
                    {
                        yield = Random.Range(0, seed.yield);

                        if (yield < seed.yield)
                        {
                            yield += (int)Mathf.Floor(cultivation * 0.5f); //Cultivation used here. Every two levels guarantees one drop

                            if (yield > seed.yield)
                                yield = seed.yield + 1; //with high enough cultivation, you can get an additional drop!
                        }
                    }
                    outputPlant.AddToPlayer(yield + (int)Mathf.Floor(earthsoul * 0.2f)); //Bonus 1 or 2 yield from Earthsoul mythical skill

                    //PLANT HEALTH
                    var rollForHealth = Random.Range(0, 100);

                    if (rollForHealth > nurturing * 3)
                    {
                        seedHealth--;
                        Debug.Log("The plant has lost health at a roll of " + rollForHealth + " against " + nurturing * 3 + ".");
                    }
                    else
                    {
                        TransientDataScript.PushAlert($"This plant is extra resilient!");
                        Debug.Log("Plant miraculously retains its health at a roll of " + rollForHealth + " against " + nurturing * 5 + ". Add some effect or notice to the game.");
                    }

                    //CHECK IF PLANT IS DEAD
                    if (seedHealth > 0)
                    {
                        growthProgress = maxGrowth / 3; //skill could help here?
                    }

                    else if (seedHealth <= 0)
                    {
                        growthProgress = 0;
                        planterIsActive = false;
                        plantRenderer.sprite = null;
                    }
                }
                else
                {
                    TransientDataScript.PushAlert($"I don't have space for {outputPlant.name}.");
                }
            }
        }
        else if (seed == null && planterIsActive)
        {
            //Check planters for bugs

            if (string.IsNullOrEmpty(dataManager.seedA) && dataManager.planterIsActiveA)
            {
                Debug.LogWarning("Planter A is active but there is no seed. Deactivating planter.");
                dataManager.planterIsActiveA = false;
            }
            if (string.IsNullOrEmpty(dataManager.seedB) && dataManager.planterIsActiveB)
            {
                Debug.LogWarning("Planter B is active but there is no seed. Deactivating planter.");
                dataManager.planterIsActiveA = false;
            }
            if (string.IsNullOrEmpty(dataManager.seedC) && dataManager.planterIsActiveC)
            {
                Debug.LogWarning("Planter C is active but there is no seed. Deactivating planter.");
                dataManager.planterIsActiveA = false;
            }
        }
    }
}
