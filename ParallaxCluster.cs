using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ParallaxCluster : MonoBehaviour
{
    public List<SpriteRenderer> frontLayerSprites;
    public List<SpriteRenderer> midLayerSprites;
    public List<SpriteRenderer> backLayerSprites;
    public List<GameObject> parallaxLayer = new();
    public StationParallax parentStation;
    public GameObject parallaxFacade;
    public float customOffsetMultiplier;
    public float customMaxOffset;
    public bool hasNoMaxOffset;

    private void Awake()
    {
        GetAllSpriteRenderers();
        StartCoroutine(FadeIn(false));
    }
    void Start()
    {
        float layerAdjustment = 0;

        foreach (GameObject layer in parallaxLayer)
        {
            layer.AddComponent<ParallaxLayer>();
            var script = layer.GetComponent<ParallaxLayer>();

            script.originalY = layer.transform.position.y;
            script.originalX = layer.transform.localPosition.x;

            if (customMaxOffset != 0)
            {
                script.maxOffsetX = 0.5f;
            }
            else
            {
                script.maxOffsetX = customMaxOffset;
            }

            script.offsetMultiplier = 1.05f + layerAdjustment;
            script.parallaxFacade = parallaxFacade;
            script.hasNoMaxOffset = hasNoMaxOffset;

            if (customOffsetMultiplier != 0)
            {
                script.offsetMultiplier = customOffsetMultiplier + layerAdjustment;
            }

            layerAdjustment += 0.05f;
        }
    }

    public void StartFadeOut(bool setComplete)
    {
        StartCoroutine(FadeOut(setComplete));
    }

    public void StartFadeIn(bool setComplete)
    {
        StartCoroutine(FadeIn(setComplete));
    }

    IEnumerator FadeOut(bool setReady)
    {
        float alpha = 1;

        while (alpha > -0.5)
        {
            alpha -= 0.02f;

            foreach (SpriteRenderer rend in frontLayerSprites)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha + 0.3f);
            }

            foreach (SpriteRenderer rend in midLayerSprites)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha + 0.18f);
            }

            foreach (SpriteRenderer rend in backLayerSprites)
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

            foreach (SpriteRenderer rend in frontLayerSprites)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alphaFront);
            }

            foreach (SpriteRenderer rend in midLayerSprites)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alphaMid);
            }

            foreach (SpriteRenderer rend in backLayerSprites)
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

        foreach (SpriteRenderer child in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if (child.gameObject.GetComponent<CharacterNode>() ==  null)
            {
                allRenderersFound.Add(child);
            }

            foreach (SpriteRenderer grandChild in child.gameObject.transform.GetComponentsInChildren<SpriteRenderer>())
            {
                if (grandChild.gameObject.GetComponent<CharacterNode>() == null)
                {
                    allRenderersFound.Add(grandChild);
                }

                foreach (SpriteRenderer greatGrandChild in grandChild.gameObject.transform.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (greatGrandChild.gameObject.GetComponent<CharacterNode>() == null)
                    {
                        allRenderersFound.Add(greatGrandChild);
                    }
                }
            }
        }

        foreach (SpriteRenderer rend in allRenderersFound)
        {
            if (rend.sortingLayerName.ToLower().Contains("front"))
            {
                frontLayerSprites.Add(rend);
            }
            else if (rend.sortingLayerName.ToLower().Contains("mid"))
            {
                midLayerSprites.Add(rend);
            }
            else
            {
                backLayerSprites.Add(rend);
            }
        }
    }
}

public class ParallaxLayer : MonoBehaviour
{
    public GameObject layer;
    public float originalX;
    public float originalY;

    public float maxOffsetX;
    public float offsetMultiplier;

    public GameObject parallaxFacade;

    public bool hasNoMaxOffset;

    private void Update()
    {
        var newX = (parallaxFacade.transform.position.x * offsetMultiplier) + originalX;

        if (!hasNoMaxOffset)
        {
            if (transform.position.x >= 0)
            {
                if (newX < parallaxFacade.transform.position.x + maxOffsetX)
                {
                    //transform.position = new Vector3(newX + originalX, originalY, 0);
                }
                else
                {
                    newX = parallaxFacade.transform.position.x + maxOffsetX;
                    //transform.position = new Vector3(newX + originalX, originalY, 0);
                }
            }
            else if (transform.position.x < 0)
            {
                if (newX > parallaxFacade.transform.position.x - maxOffsetX)
                {
                    //transform.position = new Vector3(newX + originalX, originalY, 0);
                }
                else
                {
                    newX = parallaxFacade.transform.position.x - maxOffsetX;
                }
            }
        }

        transform.position = new Vector3(newX + originalX, originalY, 0);
    }
}
