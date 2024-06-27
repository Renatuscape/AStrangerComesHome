using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class InteractableBundle
{
    public string bundleID;
    public int spawnChance;
    public int cooldown;
    public int maxYieldPerItem;
    public int maxItems;
    public int extraItemPenalty;
    public bool alwaysMaxYield;
    public bool alwaysMaxItems;
    public bool exactCooldown;
    public bool disableRespawn;
    public bool isPrioritised;
    public bool yieldAllCustomContent;
    public RequirementPackage requirements;
    public List<IdIntPair> customContent;
    public List<IdIntPair> lootOutput = new();

    public void SaveNodeToPlayer()
    {
        Player.claimedLoot.Add(new() { objectID = bundleID, amount = 0 });
    }

    public bool CheckIfLootable()
    {
        return Player.claimedLoot.FirstOrDefault(e => e.objectID == bundleID) == null;
    }
}
