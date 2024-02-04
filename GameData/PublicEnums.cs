using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SYSTEM ENUMS
public enum GameState
{
    Loading,
    MainMenu,
    CharacterCreation,
    StartMenu,
    ShopMenu,
    JournalMenu,
    Overworld,
    PlayerHome,
    Dialogue,
    MapMenu,
    BankMenu
}

public enum Language
{
    English,
    Norwegian
}

public enum CameraView
{
    Normal,
    Cockpit,
    Lounge,
    Garden,
    Pet
}

public enum EngineState
{
    Off,
    FirstGear,
    SecondGear,
    ThirdGear,
    Reverse
}

//TIME ENUMS
public enum DayOfWeek
{
    Lunden,
    Martiden,
    Mercuiden,
    Ioviden,
    Venerden,
    Saturiden,
    Solden
}

//OBJECT ENUMS
public enum ItemType
{
    Seed,
    Plant,     //made from seeds
    Catalyst,  //made from plants
    Material,  //made from ingredients and catalysts
    Treasure,  //made from materials and catalysts
    Trade,     //has no function besides buy/sell at market
    Misc,      //modular and unique items, quest items, whatever. Not sold in any stores
    Book       //bought/found/rewarded, can be opened and read 
}

public enum ObjectType
{
    Item,
    EngineUpgrade,
    ManaUpgrade,
    Skill,
    Quest
}

public enum ItemRarity
{
    Junk = -1,
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Extraordinary = 3,
    Mythical = 4,
    Unique = 5
}


