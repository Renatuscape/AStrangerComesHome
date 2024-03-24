using SaveLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManagerScript : MonoBehaviour
{
    //PLAYER DATA
    public string playerName;
    public string pronounSub;
    public string pronounObj;
    public string pronounGen;

    public string playerNameColour;
    public int playerGold;

    public List<IdIntPair> inventoryList = Player.inventoryList;

    //GAME SETTINGS
    public float autoPlaySpeed;
    public bool alwaysHideCoachExterior;

    //PLAYER SPRITE
    public int hairIndex;
    public int bodyIndex;
    public int headIndex;
    public int eyesIndex;
    public int mouthIndex;
    public int frameIndex;

    public string hairHexColour;
    public string eyesHexColour;

    //JOURNEY DATA - SAVE READY
    public string currentRegion;
    public float mapPositionX;
    public float mapPositionY;
    public float timeOfDay;
    public int totalGameDays;
    public List<string> giftedThisWeek;

    //PASSENGER DATA - A
    public bool passengerIsActiveA;
    public string passengerNameA;
    public Sprite passengerSpriteA;
    public Location passengerOriginA;
    public Location passengerDestinationA;
    public List<string> passengerChatListA;

    //PASSENGER DATA - B
    public bool passengerIsActiveB;
    public string passengerNameB;
    public Sprite passengerSpriteB;
    public Location passengerOriginB;
    public Location passengerDestinationB;
    public List<string> passengerChatListB;

    //PLANTER - A
    public bool planterIsActiveA;
    public int planterSpriteA;
    public string seedA;
    public float progressSeedA;
    public int seedHealthA;

    //PLANTER - B
    public bool planterIsActiveB;
    public int planterSpriteB;
    public string seedB;
    public float progressSeedB;
    public int seedHealthB;

    //PLANTER - C
    public bool planterIsActiveC;
    public int planterSpriteC;
    public string seedC;
    public float progressSeedC;
    public int seedHealthC;

    //ALCHEMY SYNTHESISERS
    public List<SynthesiserData> alchemySynthesisers;
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
    public bool planterIsActive;
    public int planterSprite;
    public string seed;
    public float progressSeed;
    public int seedHealth;
}