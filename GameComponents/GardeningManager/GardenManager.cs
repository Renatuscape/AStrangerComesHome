using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        confirmPlantingMenu.SetActive(false);
        SyncPlayerSkills();
    }

    private void OnEnable()
    {
        SyncPlayerSkills();
        UpdatePlanterSprite();
        plantSpriteA.sprite = null;
        plantSpriteB.sprite = null;
        plantSpriteC.sprite = null;
        GrowthTick();
    }

    private void SyncPlayerSkills()
    {
        gardeningLevel = Player.GetSkillLevel("GAR000");
        cultivationLevel = Player.GetSkillLevel("GAR001");
        nurturingLevel = Player.GetSkillLevel("GAR002");
        nurturingLevel = Player.GetSkillLevel("GAR003");
    }

    private Item FindPlantFromSeed(Item seed)
    {
        var plantID = seed.outputID;
        Item plantOutput = Items.allItems.Find(plant => plant.objectID == plantID);
        return plantOutput;
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

    void FixedUpdate()
    {
        growthTimer += Time.fixedDeltaTime; //add skill adjustments

        if (growthTimer >= growthTick)
        {
            GrowthTick();
            growthTimer = 0;
        }
    }

    void GrowthTick()
    {
        SyncPlayerSkills();

        if (dataManager.planterIsActiveA && dataManager.seedA != null)
            UpdatePlanterData(ref plantSpriteA, ref dataManager.seedA, ref dataManager.progressSeedA);

        if (dataManager.planterIsActiveB && dataManager.seedB != null)
            UpdatePlanterData(ref plantSpriteB, ref dataManager.seedB, ref dataManager.progressSeedB);

        if (dataManager.planterIsActiveC && dataManager.seedC != null)
            UpdatePlanterData(ref plantSpriteC, ref dataManager.seedC, ref dataManager.progressSeedC);

    }

    void UpdatePlanterData(ref SpriteRenderer plantSprite, ref Item seed, ref float progressSeed)
    {
        int maxGrowth = seed.health * seed.yield;
        if (progressSeed <= maxGrowth)
        {
            progressSeed += 1 + (gardeningLevel * 0.2f); // GARDENING SKILL INCREASES GROWTH PER TICK

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
            plantSprite.sprite = FindPlantFromSeed(seed).sprite;
        }
    }

    public void ClickPlanter(WhichPlanter planter)
    {
        if (transientData.cameraView == CameraView.Garden)
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

    private void ProcessPlanterClick(ref bool planterIsActive, ref float growthProgress, ref Item seed, ref int seedHealth, SpriteRenderer plantRenderer)
    {
        int maxGrowth = seed.health * seed.yield;

        //IF THE PLANTER IS EMPTY
        if (!planterIsActive)
        {
            confirmPlantingMenu.SetActive(true);
        }

        //IF THERE IS A SEED AND THE PLANTER IS ACTIVE
        else if (planterIsActive && seed != null)
        {
            //IF THE PLANTER IS GROWING BUT NOT FINISHED
            if (growthProgress < maxGrowth && planterIsActive)
            {
                string growthRate = "decent";
                if (gardeningLevel < 3)
                    growthRate = "normal";
                else if (gardeningLevel > 6)
                    growthRate = "great";

                // Display info for the plant currently growing in this planter
                Debug.Log($"This {seed.name} is growing at a {growthRate} pace.");
            }

            //IF THE PLANTER IS READY TO BE HARVESTED
            if (growthProgress >= maxGrowth)
            {
                //PLANT YIELD
                var yield = seed.yield;

                if (seed.yield > 1) //Plants with only one yield by default can only get a higher yield through the Earthsoul skill
                {
                    yield = Random.Range(0, seed.yield);

                    if (yield < seed.yield)
                    {
                        yield += (int)Mathf.Floor(cultivationLevel * 0.5f); //Cultivation used here. Every two levels guarantees one drop

                        if (yield > seed.yield)
                            yield = seed.yield + 1; //with high enough cultivation, you can get an additional drop!
                    }
                }
                Player.AddItem(FindPlantFromSeed(seed).objectID, yield + (int)Mathf.Floor(earthsoulLevel * 0.2f)); //Bonus 1 or 2 yield from Earthsoul mythical skill

                //PLANT HEALTH
                var rollForHealth = Random.Range(0, 100);

                if (rollForHealth > nurturingLevel * 3)
                {
                    seedHealth--;
                    Debug.Log("The plant has lost health at a roll of " + rollForHealth + " against " + nurturingLevel * 3 + ".");
                }
                else
                {
                    Debug.Log("Plant miraculously retains its health at a roll of " + rollForHealth + " against " + nurturingLevel * 5 + ". Add some effect or notice to the game.");
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
        }
    }
}
