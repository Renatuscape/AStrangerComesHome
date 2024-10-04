using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;

[System.Serializable]
public class GuildRewardsData //Necessary for Unity to read the .json contents as an object
{
    public List<GuildRewardTier> totalPassengers;
    public List<GuildRewardTier> totalFare;
    public List<GuildRewardTier> misc;
}

public static class GuildRewardLoader
{
    public static bool isLoaded = false;
    public static GuildMenu guildData;
    public static Task StartLoading(GuildMenu guildMenu)
    {
        guildData = guildMenu;

        if (!isLoaded)
        {
            List<Task> loadingTasks = new List<Task>();

            Task loadingTask = LoadFromJsonAsync("GuildRewards.json");
            loadingTasks.Add(loadingTask);

            return Task.WhenAll(loadingTasks);
        }
        else
        {
            return null;
        }
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
                    guildData.loadedRewards = dataWrapper;

                    foreach (var tier in guildData.loadedRewards.totalPassengers)
                    {
                        tier.tierID = "GuildLootTier_" + tier.tierID;
                    }
                    foreach (var tier in guildData.loadedRewards.misc)
                    {
                        tier.tierID = "GuildLootTier_" + tier.tierID;
                    }
                    foreach (var tier in guildData.loadedRewards.totalFare)
                    {
                        tier.tierID = "GuildLootTier_" + tier.tierID;
                    }

                    isLoaded = true;
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
