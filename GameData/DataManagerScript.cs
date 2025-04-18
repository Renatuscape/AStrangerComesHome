using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DataManagerScript : MonoBehaviour
{
    //SAVE META DATA
    public string version;
    public string lastVersionSaved;
    public int saveSlot;
    public string lastSaveTime;
    public string saveID;

    //SETTINGS
    public bool autoPlayMemories;
    public bool saveOnDeath;
    public bool randomiseOnDeath;
    public bool disableCustomisationOnDeath;

    //PLAYER DATA
    public string playerName;
    public string playerGender;
    public string pronounSub;
    public string pronounObj;
    public string pronounGen;

    public string playerNameColour;

    public List<IdIntPair> inventoryList = Player.inventoryList;
    public List<IdIntPair> questProgression = Player.questProgression;
    public List<IdIntPair> upgradeWear = Player.upgradeWear;
    public List<IdIntPair> claimedLoot = Player.claimedLoot;

    //PLAYER SPRITE
    public PlayerSpriteData playerSprite; // transition from old index system to using this class

    public int headIndex;

    //JOURNEY DATA
    public string postLocationID;
    public string currentRegion;
    public float mapPositionX;
    public float mapPositionY;
    public float timeOfDay;
    public int totalGameDays;
    public List<string> giftedThisWeek;
    public List<string> unlockedNames;

    //PASSENGER DATA
    public PassengerData seatA;
    public PassengerData seatB;
    public List<IdIntPair> passengerFood;
    public List<PassengerReview> reviews;

    //CRAFTING DATA
    public List<SynthesiserData> alchemySynthesisers;
    public List<PlanterData> planters;

    public void Reset()
    {
        version = Application.version;
        lastVersionSaved = "none";
        saveID = Random.Range(100000, 999999).ToString();
        saveSlot = -1;

        // Settings
        autoPlayMemories = false;

        // Player Data
        playerName = "Morgan";
        playerNameColour = "597266";
        playerGender = "Male";
        pronounSub = "He";
        pronounObj = "Him";
        pronounGen = "His";
        playerSprite.ResetValues();
        headIndex = 0;

        totalGameDays = 0;
        timeOfDay = 0.3f;
        currentRegion = "REGION1";
        mapPositionX = 0f;
        mapPositionY = 0f;
        seatA = new() { seatID = "A"};
        seatB = new() { seatID = "B" }; ;
        postLocationID = "R0-LOCC0-CITY";

        Player.inventoryList = new();
        inventoryList = Player.inventoryList;

        Player.questProgression = new();
        questProgression = Player.questProgression;

        planters.Clear();
        alchemySynthesisers = new();
        giftedThisWeek = new();
        unlockedNames = new();
        claimedLoot = new();

        upgradeWear = new();

        foreach (var up in Upgrades.all)
        {
            TransientDataScript.gameManager.dataManager.upgradeWear.Add(new IdIntPair() { objectID = up.objectID, amount = 0 });
            Player.upgradeWear = TransientDataScript.gameManager.dataManager.upgradeWear;
        }

        alchemySynthesisers = new();
        planters = new()
        {
            new()
            {
                planterID = "PlanterA"
            },
            new()
            {
                planterID = "PlanterB"
            },
            new()
            {
                planterID = "PlanterC"
            }
        };
    }
}

[Serializable]
public class SynthesiserData
{
    public string synthesiserID;
    public bool isSynthActive;
    public Recipe synthRecipe;
    public float progressSynth;
    public bool isSynthPaused;
    public bool consumesMana;
}

[Serializable]
public class PlanterData
{
    public string planterID;
    public string planterSpriteID;
    public bool isActive;
    public Item seed;
    public float progress;
    public int seedHealth;
    public int weeds;

    public int GetMaxGrowth()
    {
        if (seed != null)
        {
            return 100 * (seed.health + seed.yield);
        }
        else
        {
            return -1;
        }
    }
}

[Serializable]
public class PassengerData
{
    public string seatID;
    public bool isActive;
    public string passengerName;
    public string characterID;
    public string spriteID;
    public Location origin;
    public Location destination;
    public int fare;
    public float satisfaction;
    public bool isFedToday;
    public bool leavesReview;
    public float eatingChance;
    public List<string> dialogueIDs;
}

[Serializable]
public class PassengerReview
{
    public int score;
    public int time;                // Time of review measured in days passed on review time
    public string passengerName;    // Store passenger name for print
    public string comment;          // Store generated comment for print
    public string originID;         // Store to get route statistics
    public string destinationID;
}