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
        return upgradeHealthPerLevel * (level * upgradeWearMultiplier);
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
            if (Player.upgradeWear == null || Player.upgradeWear.Count < 1)
            {
                foreach (var up in Upgrades.all)
                {
                    TransientDataScript.gameManager.dataManager.upgradeWear.Add(new IdIntPair() { objectID = up.objectID, amount = 0 });
                    Player.upgradeWear = TransientDataScript.gameManager.dataManager.upgradeWear;
                }
            }

            var randomUpgrade = Player.upgradeWear[Random.Range(0, Player.upgradeWear.Count - 1)];

            int upgradeLevel = Player.GetCount(randomUpgrade.objectID, "UpgradeWearTracker");
            float maxWear = CalculateMaxWear(upgradeLevel);
            int wearLevel;

            if (TransientDataScript.transientData.engineState != EngineState.Reverse)
            {
                wearLevel = (int)TransientDataScript.transientData.engineState;
            }
            else
            {
                wearLevel = 1;
            }


            randomUpgrade.amount += wearLevel;

            if (upgradeLevel >= maxWear * 0.75f)
            {
                var upgrade = Upgrades.FindByID(randomUpgrade.objectID);

                if (upgrade.isBroken)
                {
                    LogAlert.QueueTextAlert($"{upgrade.name} is broken and needs repairs.");
                    AudioManager.PlayUISound("tapGlassMuffled", 0.2f);
                    upgrade.isBroken = true;
                }

                if (upgradeLevel >= maxWear)
                {
                    Player.Remove(randomUpgrade.objectID, 1, true);

                    randomUpgrade.amount = 0;

                    LogAlert.QueueTextAlert($"{upgrade.name} has degraded by a level.");
                    AudioManager.PlayUISound("drumDoubleTap");
                }
            }
        }
    }
}