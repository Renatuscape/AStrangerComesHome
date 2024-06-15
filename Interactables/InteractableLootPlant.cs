using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableLootPlant : MonoBehaviour
{
    public string plantNodeID;
    public InteractablePlantBundle plantBundle;
    public bool disableSproutOnFail;
    public bool disableVesselOnFail;
    public SpriteRenderer plantRenderer;
    public List<SpriteRenderer> vesselRenderers;
    public BoxCollider2D col;
    public bool isEnabled;
    void Start()
    {
        col = GetComponent<BoxCollider2D>();

        plantNodeID = gameObject.name;

        if (plantBundle != null && plantBundle.CheckBundle(plantNodeID))
        {
            plantBundle.Setup();
            plantRenderer.sprite = plantBundle.plant.sprite;
            isEnabled = true;
        }
        else
        {
            col.enabled = false;

            if (disableSproutOnFail)
            {
                plantRenderer.enabled = false;
            }
            if (disableVesselOnFail)
            {
                plantRenderer.enabled = false;

                foreach (var rend in  vesselRenderers)
                {
                    rend.enabled = false;
                }
            }

            if (!disableVesselOnFail && !disableSproutOnFail)
            {
                int maxGrowth = plantBundle.cooldown;
                int progress = plantBundle.playerEntry != null ? plantBundle.playerEntry.amount : 0;

                Item seed = plantBundle.seed;

                if (progress < maxGrowth * 0.3f)
                {
                    plantRenderer.sprite = SpriteFactory.GetSprout(seed.objectID, 1);
                }
                else if (progress > maxGrowth * 0.3f && progress < maxGrowth * 0.6f)
                {
                    plantRenderer.sprite = SpriteFactory.GetSprout(seed.objectID, 2);
                }
                else if (progress > maxGrowth * 0.6f)
                {
                    plantRenderer.sprite = SpriteFactory.GetSprout(seed.objectID, 3);
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if (plantBundle != null && isEnabled)
        {
            plantBundle.YieldLoot();
        }
    }
}

[Serializable]
public class InteractablePlantBundle : InteractableBundleData
{
    public string seedID;
    public Item seed;
    public Item plant;
    public IdIntPair playerEntry;

    public int maxPlantYield;
    public int maxSeedYield;
    public int seedDropChance;
    public bool disableSeedLoot;
    public bool dropOnlySeed;

    public void YieldLoot()
    {
        lootOutput.Clear();

        IdIntPair plantEntry;
        IdIntPair seedEntry;

        if (!dropOnlySeed)
        {
            plantEntry = new() { objectID = plant.objectID, amount = maxPlantYield};
            lootOutput.Add(plantEntry);

            if (!disableSeedLoot )
            {
                if (Random.Range(0, 100) < seedDropChance)
                {
                    seedEntry = new() { objectID = seed.objectID, amount = maxSeedYield };
                    lootOutput.Add(seedEntry);
                }
            }
        }
        else
        {
            seedEntry = new() { objectID = seed.objectID, amount = maxSeedYield};
            lootOutput.Add(seedEntry);
        }

        if (customContent != null && customContent.Count > 0)
        {
            if (yieldAllCustomContent)
            {
                lootOutput.AddRange(customContent);
            }
            else
            {
                int itemCount = 1;
                List<IdIntPair> potentialLoot = new(customContent);

                while (true)
                {
                    if (potentialLoot.Count == 0) { break; }

                    int yieldChance = 100 - (extraItemPenalty * itemCount);

                    if (Random.Range(0, 100) < yieldChance)
                    {
                        itemCount++;

                        var randomEntry = potentialLoot[Random.Range(0, potentialLoot.Count)];
                        lootOutput.Add(randomEntry);
                        potentialLoot.Remove(randomEntry);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        foreach (var entry in lootOutput)
        {
            Player.Add(entry.objectID, alwaysMaxYield ? entry.amount : Random.Range(1, entry.amount + 1));
        }
    }

    public bool CheckBundle(string plantNodeID)
    {
        bundleID = plantNodeID + "_" + seedID + "_" + (disableRespawn ? "disableRespawn" : "allowRespawn") + (exactCooldown ? "_ECD#" : "_CD#") + cooldown;
        seed = Items.FindByID(seedID);

        if (seed == null)
        {
            Debug.Log("CheckBundle could not find seed with ID " + seedID);
            return false;
        }
        else
        {
            plant = Items.FindByID(seed.outputID);

            if (plant == null)
            {
                Debug.Log("CheckBundle could not find plant with ID " + seed.outputID);
                return false;
            }
        }

        playerEntry = Player.claimedLoot.FirstOrDefault(l => l.objectID == bundleID);

        if (playerEntry != null)
        {
            Debug.Log("Loot bundle exists in the claimed loot list.");
            return false;
        }
        else if (!RequirementChecker.CheckPackage(requirements))
        {
            Debug.Log("Loot bundle did not pass requirements package.");
            return false;
        }
        else if (UnityEngine.Random.Range(0, 100) > spawnChance)
        {
            Debug.Log("Loot bundle did not pass spawn chance.");
            return false;
        }

        Debug.Log("Loot bundle passed all checks. Returning true.");
        return true;
    }

    public void Setup()
    {
        if (maxPlantYield == 0)
        {
            maxPlantYield = seed.yield;
        }
        if (!disableSeedLoot)
        {
            if (maxSeedYield == 0)
            {
                maxSeedYield = 1;
            }
            if (seedDropChance == 0)
            {
                seedDropChance = 30;
            }
        }
        if (spawnChance == 0)
        {
            spawnChance = 100;
        }
        if (cooldown == 0)
        {
            cooldown = 1 + seed.yield + seed.health;
        }
        if (maxYieldPerItem == 0)
        {
            maxYieldPerItem = 3;
        }
        if (maxItems == 0)
        {
            maxItems = 3;
        }
        if (extraItemPenalty == 0)
        {
            extraItemPenalty = 100 / (maxItems + 1);
        }
    }
}