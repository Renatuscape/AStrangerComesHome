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
        Player.Add(this, amount, maxLevel);
    }
    public int GetCountPlayer()
    {
        return Player.GetCount(objectID);
    }
}
