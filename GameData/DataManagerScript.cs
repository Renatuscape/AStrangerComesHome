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
    //public SerializableDictionary<string, int> playerItems = Player.items;
    //public SerializableDictionary<string, int> playerSkills = Player.skills;
    //public SerializableDictionary<string, int> playerUpgrades = Player.upgrades;
    //public SerializableDictionary<string, int> playerQuests = Player.quests;
    //public SerializableDictionary<string, int> playerFriendships = Player.friendships;

    public List<IdIntPair> inventoryList = Player.inventoryList;

    //GAME SETTINGS
    public float autoPlaySpeed;
    public bool alwaysHideCoachExterior;

    //PLAYER SPRITE
    public int hairIndex;
    public int bodyIndex;
    public int headIndex;
    public string hairHexColour;
    public string eyesHexColour;
    public SerializableDictionary<string, bool> faceMods;

    //JOURNEY DATA - SAVE READY
    public float mapPositionX;
    public float mapPositionY;
    public float timeOfDay;
    public int totalGameDays;

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

    //ALCHEMY SYNTHESISER - A
    public bool isSynthActiveA;
    public Item synthItemA;
    public float progressSynthA;
    public bool isSynthPausedA;

    //ALCHEMY SYNTHESISER - B
    public bool isSynthActiveB;
    public Item synthItemB;
    public float progressSynthB;
    public bool isSynthPausedB;

    //ALCHEMY SYNTHESISER - C
    public bool isSynthActiveC;
    public Item synthItemC;
    public float progressSynthC;
    public bool isSynthPausedC;

}