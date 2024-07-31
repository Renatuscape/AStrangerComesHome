using System.Linq;
using UnityEngine;

public static class ParallaxControllerHelper
{
    static ParallaxController instance;

    public static void SetInstance(ParallaxController controller)
    {
        instance = controller;
    }

    public static void AddObjectToParallax(GameObject parallaxObject, string layerName, float minX = -20, float maxX = 20, float passiveSpeed = 0)
    {
        // Debug.Log("Attempting to add parallax object to controller");
        if (instance != null && parallaxObject != null)
        {
            instance.AddObjectToParallax(parallaxObject, layerName, minX, maxX, passiveSpeed);
        }
    }

    public static void AddStationToParallax(StationParallaxData package)
    {
        // Debug.Log("Attempting to add parallax station to controller");

        if (instance != null && package != null)
        {
            instance.AddStationToParallax(package);
        }
    }

    public static float GetLayerSpeed(string layerName)
    {
        var layerData = instance.backgroundLayers.FirstOrDefault(l => l.rend.sortingLayerName.Contains(layerName));

        if (layerData != null)
        {
            // Debug.Log($"{layerName} returned a modifier of {layerData.parallaxMultiplier}");
            return layerData.parallaxMultiplier;
        }
        else
        {
            return 0;
        }
    }
}