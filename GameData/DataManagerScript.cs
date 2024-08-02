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

    //SETTINGS
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

    //CRAFTING DATA
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

    public string hairHexColour;
    public string hairAccentHexColour;
    public string accessoryHexColour;

    public string eyesHexColour;
    public string lipTintHexColour;

    public string cloakHexColour;
    public string vestHexColour;
    public string tightsHexColour;

    public float lipTintTransparency;
    public bool enableAccessory;
    public bool enableAccent;

    public void ResetValues()
    {
        hairID = "HairA0";
        bodyID = "BodyA0";
        headID = "Default";
        eyesID = "EyesA0";

        hairHexColour = "83695C";
        hairAccentHexColour = "E0C49F";
        eyesHexColour = "88DA69";
        accessoryHexColour = "A8AB98";
        lipTintHexColour = "CAAB80";
        cloakHexColour = "808E94";
        vestHexColour = "BDA8A7";
        tightsHexColour = "919F98";

        lipTintTransparency = 0;
        enableAccessory = false;
        enableAccent = false;
    }
}

[Serializable]
public class PlayerPreset
{
    public string presetName;
    public string playerName;
    public string playerGender;
    public string pronounSub;
    public string pronounObj;
    public string pronounGen;

    public string playerNameColour;
    public int headIndex;

    public PlayerSpriteData appearance;

    public void PopulateFromDataManager(DataManagerScript dataManager)
    {
        playerName = dataManager.playerName;
        playerGender = dataManager.playerGender;
        pronounGen = dataManager.pronounGen;
        pronounObj = dataManager.pronounObj;
        pronounSub = dataManager.pronounSub;
        playerNameColour = dataManager.playerNameColour;
        headIndex = dataManager.headIndex;
        appearance = dataManager.playerSprite;
    }

    public void SaveToDataManager(DataManagerScript dataManager, bool savePersonalia, bool colourOnly)
    {
        if (savePersonalia)
        {
            dataManager.playerName = playerName;
            dataManager.playerGender = playerGender;
            dataManager.pronounGen = pronounGen;
            dataManager.pronounObj = pronounObj;
            dataManager.pronounSub = pronounSub;

            dataManager.playerNameColour = playerNameColour;
        }

        if (!colourOnly)
        {
            dataManager.headIndex = headIndex;

            dataManager.playerSprite = appearance;
        }
        else
        {
            dataManager.playerSprite.eyesHexColour = appearance.eyesHexColour;
            dataManager.playerSprite.hairHexColour = appearance.hairHexColour;
            dataManager.playerSprite.hairAccentHexColour = appearance.hairAccentHexColour;
            dataManager.playerSprite.accessoryHexColour = appearance.accessoryHexColour;
            dataManager.playerSprite.tightsHexColour = appearance.tightsHexColour;
            dataManager.playerSprite.cloakHexColour = appearance.cloakHexColour;
            dataManager.playerSprite.vestHexColour = appearance.vestHexColour;
            dataManager.playerSprite.lipTintHexColour = appearance.lipTintHexColour;
            dataManager.playerSprite.lipTintTransparency = appearance.lipTintTransparency;
        }
    }
}