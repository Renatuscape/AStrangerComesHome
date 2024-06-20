using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GuildRewardsMenu : MonoBehaviour
{
    public List<Button> btnsCollectPassengers;
    public List<Button> btnsCollectFare;
    public List<Button> btnsCollectMisc;

    public List<GuildRewardTier> rewardTiersTotalPassengers;
    public List<GuildRewardTier> rewardTiersTotalFare;
    public List<GuildRewardTier> rewardTiersMisc;
    public bool isGuildmaster;
    public int totalPassengers;
    public int totalFares;

    public void OpenRewardsMenu(bool initiatedByGuildmaster, int totalPassengers, int totalFare)
    {
        isGuildmaster = initiatedByGuildmaster;
        this.totalFares = totalFare;
        this.totalPassengers = totalPassengers;

        SetUpRewards();
        gameObject.SetActive(true);
    }

    void SetUpRewards()
    {
        foreach (var tier in rewardTiersTotalPassengers)
        {
            tier.Setup("totalPassengers_" + tier.requiredCount, totalPassengers);
            PrintTier(tier, "Passengers ferried:", btnsCollectPassengers);
        }
        foreach (var tier in rewardTiersTotalFare)
        {
            tier.Setup("totalFare_" + tier.requiredCount, totalFares);
            PrintTier(tier, "Total fare earned:", btnsCollectFare);
        }

        int rewardIndex = 1;
        foreach (var tier in rewardTiersMisc)
        {
            tier.requiredCount = rewardIndex;
            tier.Setup("miscRewards_" + rewardIndex, 0);
            rewardIndex++;
            PrintMiscTier(tier);
        }
    }


    void PrintTier(GuildRewardTier tier, string tierText, List<Button> buttonList)
    {
        Button newButton;

        if (tier.claimed)
        {
            newButton = GetButton($"<color=#718c81><s>{tierText} {tier.requiredCount} </s></color>");
            newButton.interactable = false;
        }
        else
        {
            newButton = GetButton($"{tierText} {tier.requiredCount}");

            if (!isGuildmaster)
            {
                newButton.interactable = false;
            }
        }

        if (newButton.interactable)
        {
            newButton.onClick.AddListener(() =>
            {
                if (tier.playerCount >= tier.requiredCount)
                {
                    newButton.interactable = false;
                    tier.Claim();
                }
            });
        }

        buttonList.Add(newButton);
    }

    void PrintMiscTier(GuildRewardTier tier)
    {
        Button newButton;

        if (tier.claimed)
        {
            newButton = GetButton($"<color=#718c81><s>Bonus Reward #{tier.requiredCount} </s></color>");
            newButton.interactable = false;
        }
        else
        {
            newButton = GetButton($"<b>Bonus Reward #{tier.requiredCount}</b>\n{tier.description}");

            if (!isGuildmaster)
            {
                newButton.interactable = false;
            }
        }

        if (newButton.interactable)
        {
            newButton.onClick.AddListener(() =>
            {
                if (RequirementChecker.CheckRequirements(tier.requirements))
                {
                    newButton.interactable = false;
                    tier.Claim();
                }
            });
        }

        btnsCollectMisc.Add(newButton);
    }

    Button GetButton(string name)
    {
        return BoxFactory.CreateButton(name).GetComponent<Button>();
    }

    [Serializable]
    public class GuildRewardTier
    {
        public string tierID;
        public string description;
        public int requiredCount;
        public int playerCount;
        public List<IdIntPair> requirements;
        public List<IdIntPair> rewards;
        public bool claimed;

        public void Setup(string tierID, int playerCount)
        {
            this.tierID = "GuildLootTier_" + tierID;
            this.playerCount = playerCount;

            if (Player.claimedLoot.FirstOrDefault(l => l.objectID == tierID) != null)
            {
                claimed = true;
            }
        }

        public void Claim()
        {
            if (!claimed)
            {
                Player.claimedLoot.Add(new() { objectID = tierID, amount = 1 });
                claimed = true;
            }
        }
    }
}