using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationParallax : MonoBehaviour
{
    public List<GameObject> frontParallaxLayers;
    public List<GameObject> midParallaxLayers;
    public List<GameObject> backParallaxLayers;
    public List<GameObject> back2ParallaxLayers;
    public List<SpriteRenderer> spritesFront;
    public List<SpriteRenderer> spritesMid;
    public List<SpriteRenderer> spritesBack;
    public List<GameObject> parallaxLayer = new();
    public List<CharacterNode> characterNodes;
    public StationPrefab parentStation;
    public StationParallaxData dataPackage;

    private void Awake()
    {
        GetAllSpriteRenderers();
        StartCoroutine(FadeIn(false));

        parentStation = GetComponent<StationPrefab>();
    }
    void Start()
    {
        PackageParallaxData();
    }

    void PackageParallaxData()
    {
        dataPackage = new();
        dataPackage.parentStation = parentStation;

        float maxOffsetFacade = 0;
        float offsetIncrement = 1;
        float maxOffsetCentre = 30;

        foreach (var layer in frontParallaxLayers)
        {
            SetUpParallaxData("Station Front", dataPackage.frontParallaxLayers, layer, maxOffsetFacade, maxOffsetCentre);
        }

        maxOffsetFacade += offsetIncrement;
        foreach (var layer in midParallaxLayers)
        {
            SetUpParallaxData("Station Mid", dataPackage.midParallaxLayers, layer, maxOffsetFacade, maxOffsetCentre);
        }

        maxOffsetFacade += offsetIncrement;
        foreach (var layer in backParallaxLayers)
        {
            SetUpParallaxData("Station Back", dataPackage.backParallaxLayers, layer, maxOffsetFacade, maxOffsetCentre);
        }

        maxOffsetFacade += offsetIncrement;
        foreach (var layer in back2ParallaxLayers)
        {
            SetUpParallaxData("BG1", dataPackage.back2ParallaxLayers, layer, maxOffsetFacade, maxOffsetCentre);
        }

        ParallaxController.ParallaxThisStation(dataPackage);
    }

    StationParallaxDataLayer SetUpParallaxData( string layerName, List<StationParallaxDataLayer> list, GameObject layer, float maxOffsetFacade, float maxOffsetCentre)
    {
        var data = layer.AddComponent<StationParallaxDataLayer>();
        data.layerName = layerName;
        data.maxOffsetFromCentre = maxOffsetCentre;
        data.maxOffsetFromFacade = maxOffsetFacade;
        list.Add(data);
        return data;
    }

    public void StartFadeOut(bool setComplete)
    {
        StartCoroutine(FadeOut(setComplete));

        foreach (var character in characterNodes)
        {
            character.TemporarilyHide();
        }
    }

    public void StartFadeIn(bool setComplete)
    {
        StartCoroutine(FadeIn(setComplete));

        foreach (var character in characterNodes)
        {
            character.FadeInAfterTemporaryDisable();
        }
    }

    IEnumerator FadeOut(bool setReady)
    {
        float alpha = 1;

        while (alpha > -0.5)
        {
            alpha -= 0.02f;

            foreach (SpriteRenderer rend in spritesFront)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha + 0.3f);
            }

            foreach (SpriteRenderer rend in spritesMid)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha + 0.18f);
            }

            foreach (SpriteRenderer rend in spritesBack)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);
            }

            yield return new WaitForSeconds(0.01f);
        }

        if (setReady)
        {
            parentStation.clustersReady++;
        }
    }
    IEnumerator FadeIn(bool setReady)
    {
        bool fadeComplete = false;
        float alphaBack = 0;
        float alphaMid = 0;
        float alphaFront = 0;

        float fadeValueBack = 0.01f;
        float fadeValueMid = 0.01f;
        float fadeValueFront = 0.01f;

        while (!fadeComplete)
        {

            foreach (SpriteRenderer rend in spritesFront)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alphaFront);
            }

            foreach (SpriteRenderer rend in spritesMid)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alphaMid);
            }

            foreach (SpriteRenderer rend in spritesBack)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alphaBack);
            }

            if (alphaBack > 1 && alphaMid > 1 && alphaFront > 1)
            {
                fadeComplete = true;
            }

            fadeValueBack += 0.005f;
            fadeValueMid += 0.01f;
            fadeValueFront += 0.03f;

            alphaBack += fadeValueBack;
            alphaMid += fadeValueMid;
            alphaFront += fadeValueFront;

            yield return new WaitForSeconds(0.05f);
        }

        if (setReady)
        {
            parentStation.clustersReady++;
        }
    }

    void GetAllSpriteRenderers()
    {
        List<SpriteRenderer> allRenderersFound = new();

        // Traverse all SpriteRenderer components in the current GameObject and its children
        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>(true))
        {
            // Check if the current GameObject has a CharacterNode component
            CharacterNode characterNode = renderer.gameObject.GetComponent<CharacterNode>();

            if (characterNode == null)
            {
                // If no CharacterNode component is found, add the renderer to the list
                allRenderersFound.Add(renderer);
            }
            else
            {
                // If a CharacterNode component is found and it's not already in the list, add it
                if (!characterNodes.Contains(characterNode))
                {
                    characterNodes.Add(characterNode);
                }
            }
        }

        // Process the found renderers based on their sorting layer
        foreach (SpriteRenderer renderer in allRenderersFound)
        {
            if (renderer.sortingLayerName.ToLower().Contains("front"))
            {
                spritesFront.Add(renderer);
            }
            else if (renderer.sortingLayerName.ToLower().Contains("mid"))
            {
                spritesMid.Add(renderer);
            }
            else
            {
                spritesBack.Add(renderer);
            }
        }
    }
}

[Serializable]
public class StationParallaxData
{
    public StationPrefab parentStation;

    public List<StationParallaxDataLayer> frontParallaxLayers;
    public List<StationParallaxDataLayer> midParallaxLayers;
    public List<StationParallaxDataLayer> backParallaxLayers;
    public List<StationParallaxDataLayer> back2ParallaxLayers;
}

public class StationParallaxDataLayer : MonoBehaviour
{
    public GameObject layer;
    public string layerName;
    public float maxOffsetFromFacade; // Stop moving when this limit is reached
    public float maxOffsetFromCentre; // Ready to swap sides when this limit is reached
}
