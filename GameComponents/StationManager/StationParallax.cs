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
    public float maxOffsetCentre;
    float offsetIncrement;
    int offsetMultiplier;

    private void Awake()
    {
        GetAllSpriteRenderers();
        StartCoroutine(FadeIn(false));

        parentStation = GetComponent<StationPrefab>();

        offsetIncrement = 3f;
        offsetMultiplier = (midParallaxLayers.Count > 0 ? 1: 0) + (backParallaxLayers.Count > 0 ? 3 : 0) + (back2ParallaxLayers.Count > 0 ? 3 : 0);
        maxOffsetCentre = 32 + (offsetIncrement * offsetMultiplier);

        PackageParallaxData();
    }

    void PackageParallaxData()
    {
        dataPackage = new();
        dataPackage.layers = new();
        dataPackage.maxOffsetCentre = maxOffsetCentre;

        dataPackage.parentStation = parentStation;
        dataPackage.facade = frontParallaxLayers[0];

        if (dataPackage.facade == null)
        {
            dataPackage.facade = midParallaxLayers[0];
        }

        if (dataPackage.facade == null)
        {
            dataPackage.facade = backParallaxLayers[0];
        }

        float maxOffsetFacade = 0;

        foreach (var layer in frontParallaxLayers)
        {
            var data = SetUpParallaxData("Station Front", layer, maxOffsetFacade, maxOffsetCentre);
            if (data != null)
            {
                dataPackage.layers.Add(data);
            }
            else
            {
                Debug.Log($"Data for {layer} was null.");
            }
        }

        maxOffsetFacade += offsetIncrement;
        foreach (var layer in midParallaxLayers)
        {
            var data = SetUpParallaxData("Station Mid", layer, maxOffsetFacade, maxOffsetCentre);
            dataPackage.layers.Add(data);
        }

        maxOffsetFacade += offsetIncrement;
        foreach (var layer in backParallaxLayers)
        {
            var data = SetUpParallaxData("Station Back", layer, maxOffsetFacade, maxOffsetCentre);
            dataPackage.layers.Add(data);
        }

        maxOffsetFacade += offsetIncrement;
        foreach (var layer in back2ParallaxLayers)
        {
            var data = SetUpParallaxData("BG1", layer, maxOffsetFacade, maxOffsetCentre);
            dataPackage.layers.Add(data);
        }

        ParallaxControllerHelper.AddStationToParallax(dataPackage);
    }

    StationParallaxDataLayer SetUpParallaxData(string layerName, GameObject layer, float maxOffsetFacade, float maxOffsetCentre)
    {
        var layerData = layer.AddComponent<StationParallaxDataLayer>();
        layerData.layerName = layerName;
        layerData.maxOffsetFromCentre = maxOffsetCentre;
        layerData.maxOffsetFromFacade = maxOffsetFacade;
        layerData.layer = layer;
        return layerData;
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
    public float maxOffsetCentre;

    public GameObject facade;
    public List<StationParallaxDataLayer> layers;
}

public class StationParallaxDataLayer : MonoBehaviour
{
    public GameObject layer;
    public string layerName;
    public float maxOffsetFromFacade; // Stop moving when this limit is reached
    public float maxOffsetFromCentre; // Ready to swap sides when this limit is reached
    public float speedMultiplier; // Set by controller using layerName
    public bool readyToMoveLeft;
    public bool readyToMoveRight;

    public float lastMoveAmount;
}
