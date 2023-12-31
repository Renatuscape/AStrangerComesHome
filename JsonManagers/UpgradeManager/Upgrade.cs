using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    Mechanical,
    Magical
}

[System.Serializable]
public class Upgrade
{
    public string objectID;
    public string name;
    public int basePrice; //automatically calculated from type, rarity and ID
    public int maxLevel = 10;
    public UpgradeType type; //automatically from ID
    public Texture2D image; //retrieve from ID + folder
    public Sprite sprite;
    public string description;

    public void AddToPlayer(int amount = 1)
    {
        Player.AddDynamicObject(this, amount, "Upgrade");
    }
    public int GetCountPlayer()
    {
        return Player.GetCount(objectID, "Upgrade");
    }
}

public static class Upgrades
{
    public static List<Upgrade> all = new();

    public static void DebugList()
    {
        Debug.LogWarning("Upgrades.DebugList() called");

        foreach (Upgrade upgrade in all)
        {
            Debug.Log($"Skill ID: {upgrade.objectID}\tUpgrade Name: {upgrade.name}");
        }
    }

    public static void DebugAllUpgrades()
    {
        foreach (Upgrade skill in all)
        {
            skill.AddToPlayer(10);
            skill.AddToPlayer(25);
            skill.AddToPlayer(100);
        }
    }

    public static Upgrade FindByID(string searchWord)
    {
        if (string.IsNullOrWhiteSpace(searchWord))
        {
            Debug.LogWarning("Search term was empty, returned null. Ensure correct ID in calling script.");
            return null;
        }
        foreach (Upgrade upgrade in all)
        {
            if (upgrade.objectID.Contains(searchWord))
            {
                return upgrade;
            }
        }
        //Debug.LogWarning("No upgrade found with ID contianing this search term: " + searchWord);
        return null;
    }

    public static int GetMax(string searchWord)
    {
        foreach (Upgrade upgrade in all)
        {
            if (upgrade.objectID.Contains(searchWord))
            {
                return upgrade.maxLevel;
            }
        }
        return 10;
    }
}
