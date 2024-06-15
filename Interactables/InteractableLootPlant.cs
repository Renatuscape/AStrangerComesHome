using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableLootPlant : MonoBehaviour
{
    public string plantNodeID;
    public InteractablePlantBundle plantBundle;
    public bool showSproutOnEmpty;
    void Start()
    {
        
    }
}

[Serializable]
public class InteractablePlantBundle
{
    public string bundleID;
    public string seedID;
    public Item seed;
    public Item plant;

    public int maxPlantYield;
    public int maxSeedYield;
    public int maxBonusYield;
    public int spawnChance;
    public int seedDropChance;
    public int cooldown;
    public bool exactCooldown;
    public bool disableRespawn;
    public bool isPrioritised;
    public bool disableSeedLoot;
    public bool alwaysYieldMax;
    public bool yieldAllBonusLoot;
    public List<string> bonusLoot;
    public RequirementPackage requirements;

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
        }


        if (Player.claimedLoot.FirstOrDefault(l => l.objectID == bundleID) != null)
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
            maxPlantYield = 1;
        }

        if (!disableSeedLoot && maxSeedYield == 0)
        {
            maxSeedYield = 1;
        }
    }
}