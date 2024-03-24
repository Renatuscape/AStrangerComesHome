using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_PointOfInterest : MonoBehaviour
{
    public List<IdIntPair> requirements;
    public List<IdIntPair> restrictions;
    public List<Sprite> frames;
    public SpriteRenderer spriteRenderer;
    public bool animate = false;

    public int frameIndex;

    public float animationTimer;
    public float animationTick = 0.1f;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            if (RequirementChecker.CheckRequirements(requirements) && RequirementChecker.CheckRestrictions(restrictions))
            {
                animationTimer = 0;
                frameIndex = 0;
                animate = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animate)
        {
            animationTimer += Time.deltaTime;

            if (animationTimer >= animationTick)
            {
                animationTimer = 0;
                AnimationTick();
            }
        }
    }

    void AnimationTick()
    {
        frameIndex++;

        if (frameIndex >= frames.Count)
        {
            frameIndex = 0;
        }

        spriteRenderer.sprite = frames[frameIndex];
    }
}
