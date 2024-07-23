
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public ParallaxController controller;
    public ParallaxRenderer layerRenderer;
    public List<ParallaxLayerPackage> layerPackages;
    public SpriteRenderer fadeCurtain;
    public bool isReady = false;

    void Start()
    {
        if (!isReady)
        {
            Initialise();
        }
    }

    public void LoadRegion(string regionID)
    {
        var package = layerPackages.FirstOrDefault(p => p.regionID == regionID);
        if (package == null)
        {
            package = layerPackages[0];
        }
        StopAllCoroutines();
        StartCoroutine(LoadSpritePackage(package));
    }

    IEnumerator LoadSpritePackage(ParallaxLayerPackage package)
    {
        Debug.Log("Attempting to load sprite package for " + package.regionID);

        fadeCurtain.gameObject.SetActive(true);
        fadeCurtain.color = Color.black;
        TransientDataScript.transientData.currentSpeed = 0;
        yield return null;
        layerRenderer.ApplyPackage(package);

        foreach (var data in controller.backgroundLayers)
        {
            if (data.rend.sprite == package.bg1)
            {
                data.passiveSpeed = package.passiveSpeedBg1;
            }
            else if (data.rend.sprite == package.bg2)
            {
                data.passiveSpeed = package.passiveSpeedBg2;
            }
            else if (data.rend.sprite == package.bg3)
            {
                data.passiveSpeed = package.passiveSpeedBg3;
            }
            else if (data.rend.sprite == package.bg4)
            {
                data.passiveSpeed = package.passiveSpeedBg4;
            }
            else if (data.rend.sprite == package.bg5)
            {
                data.passiveSpeed = package.passiveSpeedBg5;
            }
            else if (data.rend.sprite == package.bg6)
            {
                data.passiveSpeed = package.passiveSpeedBg6;
            }
            else if (data.rend.sprite == package.fg1)
            {
                data.passiveSpeed = package.passiveSpeedFg1;
            }
            else if (data.rend.sprite == package.fg2)
            {
                data.passiveSpeed = package.passiveSpeedFg2;
            }

            controller.UpdateSpriteSizes();
        }

        StartCoroutine(FadeCurtainAndDisable());
    }

    IEnumerator FadeCurtainAndDisable()
    {
        Debug.Log("Attempting to fade curtain.");
        float alpha = fadeCurtain.color.a;
        
        yield return new WaitForSeconds(0.3f);

        while (fadeCurtain.color.a > 0)
        {
            alpha -= 0.015f;
            yield return new WaitForSeconds(0.01f);
            fadeCurtain.color = new Color(0, 0, 0, alpha);
        }

        fadeCurtain.gameObject.SetActive(false);
    }

    public void Initialise()
    {
        layerRenderer = new();
        CreateRenderer(ref layerRenderer.sky, "Sky");
        CreateRenderer(ref layerRenderer.bg6, "BG6");
        CreateRenderer(ref layerRenderer.bg5, "BG5");
        CreateRenderer(ref layerRenderer.bg4, "BG4");
        CreateRenderer(ref layerRenderer.bg3, "BG3");
        CreateRenderer(ref layerRenderer.bg2, "BG2");
        CreateRenderer(ref layerRenderer.bg1, "BG1");
        CreateRenderer(ref layerRenderer.stationBack, "StationBack");
        CreateRenderer(ref layerRenderer.stationMid, "StationMid");
        CreateRenderer(ref layerRenderer.stationFront, "StationFront");
        CreateRenderer(ref layerRenderer.road, "Road");
        CreateRenderer(ref layerRenderer.fg1, "FG1");
        CreateRenderer(ref layerRenderer.fg2, "FG2");

        SetUpController();
        LoadRegion("REGION0");
        isReady = true;

        void CreateRenderer(ref SpriteRenderer initialiseThisRenderer, string layerName)
        {
            initialiseThisRenderer = new GameObject(layerName).AddComponent<SpriteRenderer>();
            initialiseThisRenderer.sortingLayerName = layerName;
            initialiseThisRenderer.gameObject.transform.SetParent(gameObject.transform, false);
        }
    }

    public void SetUpController()
    {
        float baseSpeed = 0.07f;

        controller.AddLayer(ref layerRenderer.bg6, (baseSpeed * 0.1f));
        controller.AddLayer(ref layerRenderer.bg5, (baseSpeed * 0.2f));
        controller.AddLayer(ref layerRenderer.bg4, (baseSpeed * 0.3f));
        controller.AddLayer(ref layerRenderer.bg3, (baseSpeed * 0.4f));
        controller.AddLayer(ref layerRenderer.bg2, (baseSpeed * 0.5f));
        controller.AddLayer(ref layerRenderer.bg1, (baseSpeed * 0.6f));
        controller.AddLayer(ref layerRenderer.stationBack, (baseSpeed * 0.7f));
        controller.AddLayer(ref layerRenderer.stationMid, (baseSpeed * 0.8f));
        controller.AddLayer(ref layerRenderer.stationFront, (baseSpeed * 0.9f));
        controller.AddLayer(ref layerRenderer.road, baseSpeed);
        controller.AddLayer(ref layerRenderer.fg1, (baseSpeed * 1.1f));
        controller.AddLayer(ref layerRenderer.fg2, (baseSpeed * 1.2f));

        controller.isPlaying = true;
    }
}
