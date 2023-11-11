using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Search;
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

    public SerializableDictionary<string, int> playerUpgrades = Player.upgrades;
    public SerializableDictionary<string, int> playerSkills = Player.skills;
    public SerializableDictionary<string, int> playerItems = Player.items;

    //GAME SETTINGS
    public float autoPlaySpeed;

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
    public Item seedA;
    public float progressSeedA;
    public int seedHealthA;

    //PLANTER - B
    public bool planterIsActiveB;
    public int planterSpriteB;
    public Item seedB;
    public float progressSeedB;
    public int seedHealthB;

    //PLANTER - C
    public bool planterIsActiveC;
    public int planterSpriteC;
    public Item seedC;
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

public static class Player
{
    public static SerializableDictionary<string, int> items = new();
    public static SerializableDictionary<string, int> skills = new();
    public static SerializableDictionary<string, int> upgrades = new();
    public static string name = "Morgan";
    public static string nameColour = "346159";
    public static string namePlate = NamePlate(name, nameColour);

    public static string NamePlate(string name, string hexColour)
    {
        return "<color=#" + hexColour + ">" + name + "</color>";
    }

    #region SKILL METHODS
    public static void AddItem(string searchID, int amount)
    {
        var xObject = Items.allItems.Find(x => x.objectID == searchID);
        int maxValue = 99;

        if (xObject != null)
        {
            maxValue = xObject.maxStack;
        }

        if (items.ContainsKey(searchID))
        {
            if (items[searchID] + amount <= maxValue && items[searchID] + amount >= 0)
            {
                items[searchID] += amount;
            }
            else
            {
                Debug.LogWarning($"Failed to add {amount} {searchID} to player inventory. Max stack allowance ({maxValue}) exceeded, no changes applied.");
            }
        }
        else
        {
            if (amount <= maxValue && amount >= 0)
            {
                var item = Items.FindByID("PLA000");
                items.Add(item.objectID, amount);
            }
            else
            {
                Debug.LogWarning($"Failed to add {amount} {searchID} to player inventory. Max stack allowance ({maxValue}) exceeded, no changes applied.");
            }

        }
    }

    public static int GetItemCount(string searchID)
    {

        if (items.ContainsKey(searchID))
        {
            return items[searchID];
        }
        else
        {
            return 0;
        }
    }
    #endregion
    #region SKILL METHODS
    public static void LevelUp(string searchID, int amount)
    {
        int maxValue = 10;

        if (skills.ContainsKey(searchID))
        {
            if (skills[searchID] + amount <= maxValue)
            {
                skills[searchID] += amount;
            }
            else
            {
                Debug.LogWarning($"Failed to increase {searchID} skill level by {amount}. Max level exceeded, no changes applied.");
            }
        }
        else
        {
            if (amount <= maxValue)
            {
                var skill = Skills.FindByID(searchID);
                skills.Add(skill.objectID, amount);
            }
            else
            {
                Debug.LogWarning($"Failed to increase {searchID} skill level by {amount}. Max level exceeded, no changes applied.");
            }

        }
    }

    public static int GetSkillLevel(string searchID)
    {

        if (skills.ContainsKey(searchID))
        {
            return skills[searchID];
        }
        else
        {
            return 0;
        }
    }
    #endregion
    #region UPGRADE METHODS
    public static void Upgrade(string searchID, int amount)
    {
        int maxValue = 10;

        if (upgrades.ContainsKey(searchID))
        {
            if (upgrades[searchID] + amount <= maxValue)
            {
                upgrades[searchID] += amount;
            }
            else
            {
                Debug.LogWarning($"Failed to upgrade {searchID} by {amount}. Max level exceeded, no changes applied.");
            }
        }
        else
        {
            if (amount <= maxValue)
            {
                var upgrade = Upgrades.FindByID(searchID);
                upgrades.Add(upgrade.objectID, amount);
            }
            else
            {
                Debug.LogWarning($"Failed to upgrade {searchID} by {amount}. Max level exceeded, no changes applied.");
            }

        }
    }

    public static int GetUpgradeLevel(string searchID)
    {

        if (upgrades.ContainsKey(searchID))
        {
            return upgrades[searchID];
        }
        else
        {
            return 0;
        }
    }
    #endregion 
}

public interface IRewardable
{
    public void AddToPlayer(int amount = 1);
}