using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting;

public static class GuildRewardLoader
{
    public static GuildRewardsMenu guildRewards;
    public static Task StartLoading(GuildRewardsMenu guildRewardsMenu)
    {
        guildRewards = guildRewardsMenu;
        List<Task> loadingTasks = new List<Task>();

        Task loadingTask = LoadFromJsonAsync("GuildRewards.json");
        loadingTasks.Add(loadingTask);

        return Task.WhenAll(loadingTasks);
    }

    [System.Serializable]
    public class GuildRewardsData //Necessary for Unity to read the .json contents as an object
    {
        public List<GuildRewardTier> totalPassengers;
        public List<GuildRewardTier> totalFare;
        public List<GuildRewardTier> misc;
    }

    public static async Task LoadFromJsonAsync(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/GameData/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
            GuildRewardsData dataWrapper = JsonUtility.FromJson<GuildRewardsData>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.totalPassengers.Count > 0)
                {
                    var tiers = new List<GuildRewardTier>(guildRewards.rewardTiersMisc);
                    tiers.AddRange(dataWrapper.totalPassengers);
                    tiers.AddRange(dataWrapper.totalFare);

                    foreach (var tier in tiers)
                    {
                        tier.tierID = "GuildLootTier_" + tier.tierID;
                    }

                    guildRewards.rewardTiersTotalPassengers = dataWrapper.totalPassengers;
                    guildRewards.rewardTiersTotalFare = dataWrapper.totalFare;
                    guildRewards.rewardTiersMisc = dataWrapper.misc;
                }
                else
                {
                    Debug.LogError("Total passengers was empty in JSON data. Check JSON structure and that class is serializable.");
                }
            }
            else
            {
                Debug.LogError("JSON data is malformed. No wrapper found?");
                Debug.Log(jsonData); // Log the JSON data for inspection
            }
        }
        else
        {
            Debug.LogError("JSON file not found: " + jsonPath);
        }
    }
}
