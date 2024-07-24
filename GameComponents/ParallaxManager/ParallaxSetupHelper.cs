using UnityEngine;

public class ParallaxSetupHelper
{
    public ParallaxManager parallaxManager;
    public void Initialise()
    {
        Debug.Log("Initialising parallax manager.");
        var layerRenderer = parallaxManager.layerRenderer = new();

        CreateRenderer(ref layerRenderer.sky, "Sky");
        CreateRenderer(ref layerRenderer.bg6, "BG6");
        CreateRenderer(ref layerRenderer.bg5, "BG5");
        CreateRenderer(ref layerRenderer.bg4, "BG4");
        CreateRenderer(ref layerRenderer.bg3, "BG3");
        CreateRenderer(ref layerRenderer.bg2, "BG2");
        CreateRenderer(ref layerRenderer.bg1, "BG1");
        CreateRenderer(ref layerRenderer.stationBack, "Station Back");
        CreateRenderer(ref layerRenderer.stationMid, "Station Mid");
        CreateRenderer(ref layerRenderer.stationFront, "Station Front");
        CreateRenderer(ref layerRenderer.road, "Road");
        CreateRenderer(ref layerRenderer.fg1, "FG1");
        CreateRenderer(ref layerRenderer.fg2, "FG2");

        SetUpController();
        parallaxManager.LoadRegion("REGION0");
        parallaxManager.isReady = true;

        void CreateRenderer(ref SpriteRenderer initialiseThisRenderer, string layerName)
        {
            initialiseThisRenderer = new GameObject(layerName).AddComponent<SpriteRenderer>();
            initialiseThisRenderer.sortingLayerName = layerName;
            initialiseThisRenderer.gameObject.transform.SetParent(parallaxManager.gameObject.transform, false);
        }
    }

    public void SetUpController()
    {
        Debug.Log("Setting up parallax controller.");
        float baseSpeed = 0.07f;
        var controller = parallaxManager.controller;
        var layerRenderer = parallaxManager.layerRenderer;

        AddLayerController(ref layerRenderer.bg6, (baseSpeed * 0.1f));
        AddLayerController(ref layerRenderer.bg5, (baseSpeed * 0.2f));
        AddLayerController(ref layerRenderer.bg4, (baseSpeed * 0.3f));
        AddLayerController(ref layerRenderer.bg3, (baseSpeed * 0.4f));
        AddLayerController(ref layerRenderer.bg2, (baseSpeed * 0.5f));
        AddLayerController(ref layerRenderer.bg1, (baseSpeed * 0.6f));
        AddLayerController(ref layerRenderer.stationBack, (baseSpeed * 0.7f));
        AddLayerController(ref layerRenderer.stationMid, (baseSpeed * 0.8f));
        AddLayerController(ref layerRenderer.stationFront, (baseSpeed * 0.9f));
        AddLayerController(ref layerRenderer.road, baseSpeed);
        AddLayerController(ref layerRenderer.fg1, (baseSpeed * 1.1f));
        AddLayerController(ref layerRenderer.fg2, (baseSpeed * 1.2f));

        controller.isPlaying = true;
    }

    public void AddLayerController(ref SpriteRenderer rend, float pMultiplier)
    {
        if (rend != null)
        {
            Debug.Log("Creating layer controller for " + rend.name);
            //if (rend.sprite != null)
            //{
            //    Debug.Log("Created layer using sprite " + rend.sprite.name);
            //}

            ParallaxRenderPackage newPackage = new() { parallaxMultiplier = pMultiplier };

            newPackage.rend = rend;

            parallaxManager.controller.backgroundLayers.Add(newPackage);
        }
    }
}