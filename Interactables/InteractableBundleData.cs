using System;
using System.Collections.Generic;

[Serializable]
public class InteractableBundleData
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
}
