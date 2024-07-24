using System;
using UnityEngine;

[Serializable]
public class ParallaxRenderPackage
{
    public SpriteRenderer rend;
    public float passiveSpeed;
    public float parallaxMultiplier;
    public Vector3 spriteSize;

    public float lastMoveAmount;
}
