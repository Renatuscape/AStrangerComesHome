using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class GuildRewardsMenu : MonoBehaviour
{
    public PageinatedList pageinatedList;

    public List<GuildRewardPrefab> tierPrefabs;
    public bool isGuildmaster;
    public int totalPassengers;
    public int totalFare;

    public void Initialise(bool initiatedByGuildmaster, GuildRewardsData rewardsData) // Run when on opening the guild menu
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
        else
        {
            isGuildmaster = initiatedByGuildmaster;
            
            gameObject.SetActive(true);
            SetUpRewards(rewardsData);
        }
    }

    void SetUpRewards(GuildRewardsData rewardsData)
    {
        CleanUp();

        List<IdIntPair> fareTierIDs = new();
        List<IdIntPair> passengerTierIDs = new();
        List<IdIntPair> miscTierIDs = new();

        foreach (var tier in rewardsData.totalFare)
        {
            string tierName = $"{tier.requirements[0].amount} Shillings Earned";
            fareTierIDs.Add(new IdIntPair() { objectID = tier.tierID, description = tierName});
        }
        foreach (var tier in rewardsData.totalPassengers)
        {
            string tierName = $"{tier.requirements[0].amount} Passengers Served";
            passengerTierIDs.Add(new IdIntPair() { objectID = tier.tierID, description = tierName });
        }
        foreach (var tier in rewardsData.misc)
        {
            miscTierIDs.Add(new IdIntPair() { objectID = tier.tierID, description = $"{tier.tierName}: <color=#A45807>{tier.description}</color>"});
        }

        var listCategory = new List<ListCategory>() {
            new() { categoryName = "Total Fare", listContent = fareTierIDs},
            new() { categoryName = "Total Passengers", listContent = passengerTierIDs},
            new() { categoryName = "Bonuses", listContent = miscTierIDs},
        };

        var prefabs = pageinatedList.InitialiseWithCategories(listCategory);

        List<GuildRewardTier> allRewardsData = new();
        allRewardsData.AddRange(rewardsData.totalFare);
        allRewardsData.AddRange(rewardsData.totalPassengers);
        allRewardsData.AddRange(rewardsData.misc);

        foreach (var prefab in prefabs)
        {
            var prefabScript = prefab.AddComponent<GuildRewardPrefab>();
            var listScript = prefab.GetComponent<ListItemPrefab>();

            var matchingTierData = allRewardsData.FirstOrDefault(r => r.tierID == listScript.entry.objectID);

            if (matchingTierData == null)
            {
                Debug.Log("GWM: Matching tier data was null for list item " + listScript.entry.objectID);
            }

            tierPrefabs.Add(prefabScript);
            prefabScript.Setup(listScript, matchingTierData);
        }

        CheckAllTiers();
    }

    public void CheckAllTiers()
    {
        foreach (var item in tierPrefabs)
        {
            item.CheckState();
        }
    }

    public void CleanUp()
    {
        tierPrefabs.Clear();
        pageinatedList.ClearPrefabs();
    }
}
