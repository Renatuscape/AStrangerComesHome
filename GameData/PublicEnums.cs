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
    BankMenu,
    AlchemyMenu
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
    Seed = 0,
    Plant = 1,     //made from seeds
    Material = 2, //made from ingredients and catalysts
    Catalyst = 3,  //made from plants
    Trade = 4,     //has no function besides buy/sell at market
    Book = 5,       //bought/found/rewarded, can be opened and read 
    Misc = 6,      //modular and unique items, quest items, whatever. Not sold in any stores
    Treasure = 7,  //made from materials and catalysts
    Script = 8      //only for data used by scripts
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
    Unique = 5,
    Script = 6,
}


