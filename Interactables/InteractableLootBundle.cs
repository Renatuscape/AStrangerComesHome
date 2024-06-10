using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

[Serializable]
public class InteractableLootBundle
{
    public string bundleID;
    public bool doNotRespawn;
    public bool isPrioritised;
    public int spawnChance;
    public int extraItemPenalty;
    public int coolDown;
    public bool exactCooldown;
    public RequirementPackage requirements;
    public List<LootCategory> categories;
    public List<IdIntPair> customContent;
    public bool yieldAllCustomContent;
    public int maxYieldPerItem;
    public int maxItems;
    public bool alwaysMaxYield;
    public bool alwaysMaxItems;
    public List<Item> viableLoot = new();
    public List<IdIntPair> lootOutput = new();

    public bool SetupAndCheck(string crateID)
    {
        bundleID = crateID + "_" + (doNotRespawn ? "disableRespawn" : "allowRespawn") + (exactCooldown ? "_ECD#" : "_CD#") + coolDown;

        if (spawnChance == 0)
        {
            spawnChance = 100;
        }
        if (maxYieldPerItem == 0)
        {
            maxYieldPerItem = 3;
        }
        if (maxItems == 0)
        {
            maxItems = 4;
        }
        if (extraItemPenalty == 0)
        {
            extraItemPenalty = 100 / maxItems;
        }

        BuildLootList();

        if ((categories == null || categories.Count < 1) && (customContent == null || customContent.Count < 1))
        {
            Debug.Log("Loot bundle had no content.");
            return false;
        }
        else if (Player.claimedLoot.FirstOrDefault(l => l.objectID == bundleID) != null)
        {
            Debug.Log("Loot bundle exists in the claimed loot list.");
            return false;
        }
        else if (!RequirementChecker.CheckPackage(requirements))
        {
            Debug.Log("Loot bundle did not pass requirements package.");
            return false;
        }
        else if (UnityEngine.Random.Range(0, 100) > spawnChance)
        {
            Debug.Log("Loot bundle did not pass spawn chance.");
            return false;
        }

        Debug.Log("Loot bundle passed all checks. Returning true.");
        return true;
    }

    void BuildLootList()
    {
        if (categories.Count > 0)
        {
            viableLoot = Items.all.Where(item =>
            {
                foreach (var category in categories)
                {
                    // CHECK IF ITEM MATCHES TYPE AND CATEGORY
                    if (item.type == category.type && category.rarities.Contains(item.rarity) && !item.notBuyable)
                    {
                        return true;
                    }

                    // IF NOT, CHECK IF ITEM EXISTS IN CUSTOM CONTENT LIST
                    else if (customContent.Count > 0 && !yieldAllCustomContent) // Don't mix custom content into this list if all is being yielded
                    {
                        if (customContent.FirstOrDefault(e => e.objectID == item.objectID) != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return false;
            }).ToList();
        }
    }
    void RollForLoot()
    {
        int itemsToYield = 0;

        if (alwaysMaxItems)
        {
            itemsToYield = maxItems;
        }
        else
        {
            var itemChance = 100;
            bool rollFailed = false;

            while (!rollFailed)
            {
                if (UnityEngine.Random.Range(0, 100) < itemChance)
                {
                    itemChance -= extraItemPenalty;
                    itemsToYield++;
                }
                else
                {
                    rollFailed = true;
                }
            }
        }

        if (yieldAllCustomContent)
        {
            foreach (var item in customContent)
            {
                lootOutput.Add(item);
            }

            itemsToYield += customContent.Count;
        }

        while (lootOutput.Count < itemsToYield)
        {
            lootOutput.Add(RollItemSlot());
        }
    }

    IdIntPair RollItemSlot()
    {
        var lootCategory = categories[UnityEngine.Random.Range(0, categories.Count)];
        List<ItemRarity> rarities = lootCategory.rarities.OrderBy(rarity => (int)rarity).ToList();
        int rarityIndex = 0;
        bool rarityRollFailed = false;
        var currentRarity = rarities[rarityIndex];

        while (rarityIndex < rarities.Count - 1 && !rarityRollFailed)
        {
            if (AttemptToIncreaseRarity(currentRarity))
            {
                rarityIndex++;
                currentRarity = rarities[rarityIndex];
            }
            else
            {
                rarityRollFailed = true;
            }
        }

        if (rarityIndex >= rarities.Count)
        {
            Debug.Log("Rarity index was higher than rarities count. Setting to max.");
            rarityIndex = rarities.Count - 1;
        }

        var items = viableLoot.FindAll(i => i.rarity == rarities[rarityIndex]);

        if (items.Count == 0)
        {
            Debug.Log("No item was returned of the given rarity: " + rarities[rarityIndex] + ". Choosing random item.");

            var item = viableLoot[UnityEngine.Random.Range(0, items.Count)];

            viableLoot.Remove(item);
            return new IdIntPair() { objectID = item.objectID, amount = maxYieldPerItem };
        }
        else
        {
            var item = items[UnityEngine.Random.Range(0, items.Count)];

            viableLoot.Remove(item);
            return new IdIntPair() { objectID = item.objectID, amount = maxYieldPerItem };
        }
    }

    bool AttemptToIncreaseRarity(ItemRarity rarity)
    {
        int increaseChance = 0;

        if (rarity == ItemRarity.Junk)
        {
            increaseChance = 75;
        }
        else if (rarity == ItemRarity.Common)
        {
            increaseChance = 35;
        }
        else if (rarity == ItemRarity.Uncommon)
        {
            increaseChance = 25;
        }
        else if (rarity == ItemRarity.Rare)
        {
            increaseChance = 10;
        }
        else if (rarity == ItemRarity.Extraordinary)
        {
            increaseChance = 3;
        }

        if (UnityEngine.Random.Range(0, 100) < increaseChance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void YieldOutput()
    {
        RollForLoot();
        Debug.Log("Outputting yield for " + bundleID + " and adding to Player.claimedLoot.");

        if (lootOutput.Count > 0)
        {
            foreach (var entry in lootOutput)
            {
                if (!alwaysMaxItems)
                {
                    entry.amount = UnityEngine.Random.Range(1, entry.amount);
                }

                Debug.Log("Adding generated yield entry to player: " + entry.objectID + " " + entry.amount);
                Player.Add(entry);
            }
        }

        Player.claimedLoot.Add(new() { objectID = bundleID, amount = 0 });
    }

    [Serializable]
    public class LootCategory
    {
        public ItemType type;
        public List<ItemRarity> rarities;
    }
}
