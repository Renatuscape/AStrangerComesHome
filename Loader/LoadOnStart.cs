using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadOnStart : MonoBehaviour
{
    // List of loader objects with same parent
    public List<JsonLoader> loaders = new();
    public float loaderStepDelay = 0.2f; // For testing

    // Loading bar
    public Slider loadingBar;

    void Awake()
    {
        int toLoad = loaders.Count;
        loadingBar.maxValue = toLoad + 1;
        loadingBar.value = 0;

        LoadJsonData();
    }

    async void LoadJsonData()
    {
        foreach (var loader in loaders)
        {
            Log.Write($"Loading {loader.displayName} from {loader.path}");
            await loader.StartLoading();
            await Task.Delay(TimeSpan.FromSeconds(loaderStepDelay));
            loadingBar.value++;
        }

        // Loading has completed

        loadingBar.value++;
        Log.ToConsole();
    }
}
