using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GuildRewardsMenu : MonoBehaviour
{
    public GameObject rewardContainerPassengers;
    public GameObject rewardContainerFare;
    public GameObject rewardContainerMisc;

    public List<Button> btnsCollectPassengers;
    public List<Button> btnsCollectFare;
    public List<Button> btnsCollectMisc;

    public List<GuildRewardTier> rewardTiersTotalPassengers;
    public List<GuildRewardTier> rewardTiersTotalFare;
    public List<GuildRewardTier> rewardTiersMisc;
    public bool isGuildmaster;
    public int totalPassengers;
    public int totalFare;

    private void OnDisable()
    {
        List<Button> buttons = new(btnsCollectPassengers);
        buttons.AddRange(btnsCollectFare);
        buttons.AddRange(btnsCollectMisc);

        foreach (var tier in buttons)
        {
            Destroy(tier.gameObject);
        }

        btnsCollectPassengers.Clear();
        btnsCollectFare.Clear();
        btnsCollectMisc.Clear();

    }
    public void Initialise(bool initiatedByGuildmaster, int totalPassengers, int totalFare)
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
        else
        {
            isGuildmaster = initiatedByGuildmaster;
            this.totalFare = totalFare;
            this.totalPassengers = totalPassengers;

            SetUpRewards();
            gameObject.SetActive(true);
        }
    }

    void SetUpRewards()
    {
        foreach (var tier in rewardTiersTotalPassengers)
        {
            var button = PrintTier(tier, "Passengers ferried:", btnsCollectPassengers, rewardContainerPassengers);
            tier.Setup(totalPassengers, button);
        }
        foreach (var tier in rewardTiersTotalFare)
        {
            var button = PrintTier(tier, "Total fare earned:", btnsCollectFare, rewardContainerFare);
            tier.Setup(totalFare, button);
        }

        int rewardCount = 1;
        foreach (var tier in rewardTiersMisc)
        {
            tier.requiredCount = rewardCount;
            rewardCount++;

            var button = PrintMiscTier(tier);
            tier.Setup(0, button);
        }
    }


    Button PrintTier(GuildRewardTier tier, string tierText, List<Button> buttonList, GameObject parentContainer)
    {
        Button newButton = GetButton($"{(string.IsNullOrEmpty(tier.description) ? "" : $"<b>{tier.description}</b>\n")}{tierText} {tier.requiredCount}");

        if (tier.playerCount < tier.requiredCount)
        {
            newButton.interactable = false;
        }
        else
        {
            if (!isGuildmaster)
            {
                newButton.onClick.AddListener(() =>
                {
                    LogAlert.QueueTextAlert("I can pick up this reward at the Guild headquarters in the Capital.");
                });
            }
            else
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
        }

        newButton.gameObject.transform.SetParent(parentContainer.transform);
        buttonList.Add(newButton);
        return newButton;
    }

    Button PrintMiscTier(GuildRewardTier tier)
    {
        Button newButton = GetButton($"<b>Bonus Reward #{tier.requiredCount}</b>\n{tier.description}");

        if (!RequirementChecker.CheckRequirements(tier.requirements))
        {
            newButton.interactable = false;
        }
        else
        {
            if (!isGuildmaster)
            {
                newButton.onClick.AddListener(() =>
                {
                    LogAlert.QueueTextAlert("I can pick up this reward at the Guild headquarters in the Capital.");
                });
            }
            else
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
        }

        newButton.gameObject.transform.SetParent(rewardContainerMisc.transform);
        btnsCollectMisc.Add(newButton);

        return newButton;
    }

    Button GetButton(string name)
    {
        return BoxFactory.CreateButton(name).GetComponent<Button>();
    }
}
