using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadOnStart : MonoBehaviour
{
    // List of loader objects with same parent
    public List<JsonLoader> loaders = new();

    // Loading bar
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;

    public static LoadOnStart instance;

    float loaderStepDelay = 0.2f; // For testing

    void Awake()
    {
        instance = this;
        loaders = new List<JsonLoader>()
        {
            new JsonLoaderRegions(),
            new JsonLoaderItems(),
            new JsonLoaderSkills(),
            new JsonLoaderUpgrades(),
            new JsonLoaderCharacters(),
        };

        int toLoad = loaders.Count;
        loadingBar.maxValue = toLoad + 2;

        LoadJsonData();
    }

    async void LoadJsonData()
    {
        loadingBar.value = 0;

        DisplayLoadMessage("Building sprite library");

        while (SpriteFactory.instance == null)
        {
            await Task.Delay(TimeSpan.FromSeconds(loaderStepDelay));
        }

        await SpriteFactory.instance.WaitForBuildCompletionAsync();
        await Task.Delay(TimeSpan.FromSeconds(loaderStepDelay));
        loadingBar.value++;

        foreach (var loader in loaders)
        {
            DisplayLoadMessage($"Organising {loader.displayName}");
            Log.Write(loader.path);
            await loader.StartLoading();
            await Task.Delay(TimeSpan.FromSeconds(loaderStepDelay));
            loadingBar.value++;
        }

        DisplayLoadMessage($"Starting up");
        loadingBar.value++;

        GameManagerScript.jsonDataLoaded = true;

        await Task.Delay(TimeSpan.FromSeconds(loaderStepDelay));
        SceneManager.LoadScene("GameScene");
    }

    void DisplayLoadMessage(string message)
    {
        Log.Write(message);
        if (loadingText != null)
        {
            loadingText.text = message;
        }
    }
}
