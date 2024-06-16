using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UpgradeWearTracker
{
    public static int upgradeHealthPerLevel = 500;
    public static float upgradeWearMultiplier = 0.8f;

    public static float CalculateMaxWear(int level)
    {
        return upgradeHealthPerLevel * ((level +1) * upgradeWearMultiplier);
    }

    public static int CalculateRepairPrice(string upgradeID)
    {
        int upgradeWear = Player.upgradeWear.FirstOrDefault(e => e.objectID == upgradeID).amount;
        int wearPrice = (int)Mathf.Ceil(upgradeWear * 0.3f);

        return wearPrice;
    }

    public static bool RepairUpgrade(string upgradeID, int repairPrice)
    {
        var upgradeEntry = Player.upgradeWear.FirstOrDefault(e => e.objectID == upgradeID);

        if (upgradeEntry != null)
        {
            if (MoneyExchange.Purchase(repairPrice))
            {
                upgradeEntry.amount = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.LogWarning("Could not find upgrade entry with ID " + upgradeID);
            return false;
        }
    }

    public static void GlobalPushWearUpgrade()
    {
        if (TransientDataScript.transientData.currentSpeed > 0)
        {
            var randomUpgrade = Player.upgradeWear[Random.Range(0, Player.upgradeWear.Count)];

            int upgradeLevel = Player.GetCount(randomUpgrade.objectID, "UpgradeWearTracker");
            float maxWear = CalculateMaxWear(upgradeLevel);
            int wearLevel;
            // Debug.Log("Calculated maxWear: " + maxWear + ". Attempting to check current amount against " + (maxWear * 0.9f));

            if (upgradeLevel > 0 || randomUpgrade.amount < maxWear * 0.9f)
            {
                if (TransientDataScript.transientData.engineState != EngineState.Reverse)
                {
                    wearLevel = (int)TransientDataScript.transientData.engineState;
                }
                else
                {
                    wearLevel = 1;
                }

                randomUpgrade.amount += wearLevel;
            }

            if (randomUpgrade.amount >= maxWear * 0.75f)
            {
                var upgrade = Upgrades.FindByID(randomUpgrade.objectID);

                if (!upgrade.isBroken)
                {
                    LogAlert.QueueTextAlert($"{upgrade.name} is broken and needs repairs.");
                    AudioManager.PlaySoundEffect("tapGlassMuffled", 0.2f);
                    upgrade.isBroken = true;
                }

                if (randomUpgrade.amount >= maxWear && upgradeLevel > 0)
                {
                    Player.Remove(randomUpgrade.objectID, 1, true);

                    randomUpgrade.amount = 0;

                    LogAlert.QueueTextAlert($"{upgrade.name} has degraded by a level.");
                    AudioManager.PlaySoundEffect("drumDoubleTap");
                }
            }
        }
    }
}
