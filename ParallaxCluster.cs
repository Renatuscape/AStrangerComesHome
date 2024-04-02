using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxCluster : MonoBehaviour
{
    public List<GameObject> parallaxLayer = new();
    public GameObject parallaxFacade;
    void Start()
    {
        float layerAdjustment = 0;

        foreach (GameObject layer in parallaxLayer)
        {
            layer.AddComponent<ParallaxLayer>();
            var script = layer.GetComponent<ParallaxLayer>();

            script.originalY = layer.transform.position.y;
            script.originalX = layer.transform.localPosition.x;
            script.maxOffsetX = 0.5f;
            script.offsetMultiplier = 1.05f + layerAdjustment;
            script.parallaxFacade = parallaxFacade;

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


    private void Update()
    {
        var newX = (parallaxFacade.transform.position.x * offsetMultiplier) + originalX;

        if (parallaxFacade.transform.localPosition.x > 0 && newX > maxOffsetX)
        {
            Debug.Log($"New position was {newX}. Changing to {maxOffsetX}, the max offset.");
            newX = maxOffsetX;
        }
        else if (parallaxFacade.transform.localPosition.x < 0 && newX < maxOffsetX)
        {
            Debug.Log($"New position was {newX}. Changing to {0 - maxOffsetX}, the max offset.");
            newX = 0 - maxOffsetX;
        }

        transform.position = new Vector3(newX + originalX, originalY, 0);
    }
}
