using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxCluster : MonoBehaviour
{
    public List<SpriteRenderer> frontLayerSprites;
    public List<SpriteRenderer> midLayerSprites;
    public List<SpriteRenderer> backLayerSprites;
    public List<GameObject> parallaxLayer = new();
    public List<CharacterNode> characterNodes;
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

            script.offsetMultiplier = 1.2f + layerAdjustment;
            script.parallaxFacade = parallaxFacade;
            script.hasNoMaxOffset = hasNoMaxOffset;

            if (customOffsetMultiplier != 0)
            {
                script.offsetMultiplier = customOffsetMultiplier + layerAdjustment;
            }

            layerAdjustment += 0.1f;
        }
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
            character.RefreshNode();
        }
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
                frontLayerSprites.Add(renderer);
            }
            else if (renderer.sortingLayerName.ToLower().Contains("mid"))
            {
                midLayerSprites.Add(renderer);
            }
            else
            {
                backLayerSprites.Add(renderer);
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

    float tick = StaticGameValues.parallaxFrameRate;
    float timer = 0;

    private void LateUpdate()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            timer += Time.deltaTime;
            if (timer >= tick)
            {
                timer = 0;
                Tick();
            }            
        }
    }

    void Tick()
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
