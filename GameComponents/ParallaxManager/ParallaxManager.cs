
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    ParallaxSetupHelper setupHelper;
    public ParallaxController controller;
    public ParallaxBackgroundRenderer layerRenderer;
    public List<ParallaxBackgroundPackage> layerPackages;
    public SpriteRenderer fadeCurtain;
    public bool isReady = false;

    void Start()
    {
        if (!isReady)
        {
            setupHelper = new();
            setupHelper.parallaxManager = this;
            setupHelper.Initialise();
        }
    }

    public void LoadRegion(string regionID)
    {
        Debug.Log("Loading region " + regionID);
        var package = layerPackages.FirstOrDefault(p => p.regionID == regionID);
        if (package == null)
        {
            package = layerPackages[0];
        }
        StopAllCoroutines();
        StartCoroutine(LoadBackgroundPackage(package));
    }

    IEnumerator LoadBackgroundPackage(ParallaxBackgroundPackage package)
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
}
