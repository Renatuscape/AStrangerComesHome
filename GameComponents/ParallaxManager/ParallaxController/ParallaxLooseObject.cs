using System;
using UnityEngine;

[Serializable]
public class ParallaxLooseObject
{
    public GameObject parallaxObject;
    public ParallaxRenderPackage layerPackage;
    public float yAxis;
    public float minX;
    public float maxX;
    public float passiveSpeed;

    public float lastMoveAmount;
}
