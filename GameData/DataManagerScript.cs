using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManagerScript : MonoBehaviour
{
    //SAVE META DATA
    public string version;
    public string lastVersionSaved;
    public int saveSlot;
    public string lastSaveTime;
    public string saveID;


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

    //GAME SETTINGS
    public float autoPlaySpeed;
    public bool alwaysHideCoachExterior;

    //PLAYER SPRITE
    public PlayerSpriteData playerSprite; // transition from old index system to using this class

    public int bodyIndex;
    public int headIndex;
    public int eyesIndex;
    public int mouthIndex;

    public string eyesHexColour;
    public string frameID;

    //JOURNEY DATA - SAVE READY
    public string postLocationID;
    public string currentRegion;
    public float mapPositionX;
    public float mapPositionY;
    public float timeOfDay;
    public int totalGameDays;
    public List<string> giftedThisWeek;
    public List<string> unlockedNames;

    //PASSENGER DATA - A
    public PassengerData seatA;
    public PassengerData seatB;

    //ALCHEMY SYNTHESISERS
    public List<SynthesiserData> alchemySynthesisers;
    public List<PlanterData> planters;
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
    public float maxGrowth;
    public float progress;
    public int seedHealth;
    public int weeds;
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
    public List<string> dialogueIDs;
}

[Serializable]
public class PlayerSpriteData
{
    public string hairID;
    public string bodyID;
    public string headID;
    public string eyesID;
    public string lipTintID;

    public string hairHexColour;
    public string accessoryHexColour;
    public string eyesHexColour;
    public string lipTintHexColour;
    public float lipTintTransparency;
    public bool enableAccessory;

    public void ResetValues()
    {
        hairID = "default";
        bodyID = "default";
        headID = "default";
        eyesID = "default";
        lipTintID = "none";

        hairHexColour = "83695C";
        eyesHexColour = "88DA69";
        accessoryHexColour = "85BFB1";
        lipTintHexColour = "CAAB80";
        lipTintTransparency = 0;
        enableAccessory = false;
    }
}