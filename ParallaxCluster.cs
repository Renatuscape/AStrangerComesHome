using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxCluster : MonoBehaviour
{
    public List<GameObject> parallaxLayer = new();
    public GameObject parallaxFacade;
    public float customOffsetMultiplier;
    public float customMaxOffset;
    public bool hasNoMaxOffset;
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
