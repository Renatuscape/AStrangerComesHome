using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    Mechanical,
    Magical,
}

public static class Upgrades
{
    public static List<Upgrade> allUpgrades = new();

    public static void DebugList()
    {
        Debug.LogWarning("Upgrades.DebugList() called");

        foreach (Upgrade upgrade in allUpgrades)
        {
            Debug.Log($"Skill ID: {upgrade.objectID}\tSkill Name: {upgrade.name}");
        }
    }

    public static Upgrade FindByID(string searchWord)
    {
        foreach (Upgrade upgrade in allUpgrades)
        {
            if (upgrade.objectID.Contains(searchWord))
            {
                return upgrade;
            }
        }
        return null;
    }
}
public class Upgrade : IRewardable
{
    public string objectID;
    public string name;
    public UpgradeType type;
    public int maxLevel = 10;
    public string description;
    public Texture2D image; //retrieve from ID + folder
    public Sprite sprite;
    public List<Texture2D> animationFrames; //for animation frames
    public bool notBuyable; //from ID, second to last letter N/B (not/buyable)
    public bool notSellable; //from ID, last letter N/S (not/sellable)

    public void AddToPlayer(int amount = 1)
    {
        if (amount > 0)
        {
            Player.Upgrade(objectID, amount);
        }
    }
}
public class UpgradeManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
