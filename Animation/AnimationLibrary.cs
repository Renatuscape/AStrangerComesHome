using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum AnimationType
{
    idle,
    idleAction,
    appear,
    disappear,
    open,
    close,
    walk,
    run,
    alert,
    chat,
    use
}

public class AnimationLibrary : MonoBehaviour
{
    public List<AnimatedSprite> animatedObject;
    public static AnimationLibrary animationLibrary;
    float defaultFrameRate = 0.1f;
    private void Start()
    {
        animationLibrary = GetComponent<AnimationLibrary>();

        foreach (var animatedSprite in animatedObject)
        {
            foreach (var anim in animatedSprite.animationData)
            {
                if (anim.frameRate == 0)
                {
                    anim.frameRate = defaultFrameRate;
                }
            }
        }
    }

    public static AnimatedSprite GetAnimatedObject(string searchTerm)
    {
        var animatedObject = animationLibrary.animatedObject.Where(a => a.name == searchTerm).FirstOrDefault();
        if (animatedObject == null)
        {
            Debug.Log($"No animated object found in library by the name of {searchTerm}");
        }
        return animatedObject;
    }
}

[Serializable]
public class AnimatedSprite
{
    public string name;
    public Sprite still;
    public List<AnimationData> animationData;

    public AnimationData GetAnimationType(AnimationType type, bool checkForRandom = false)
    {
        if (checkForRandom)
        {
            List<AnimationData> animations = animationData.FindAll(f => f.type == type).ToList();

            if (animations.Count > 0)
            {
                return animations[UnityEngine.Random.Range(0, animations.Count)];
            }
            else
            {
                return null;
            }
        }
        else
        {
            return animationData.FirstOrDefault(f => f.type == type);
        }
    }
}

[Serializable]
public class AnimationData
{
    public AnimationType type;
    public int customLoopStart;
    public bool disallowLooping;
    public float frameRate = 0.1f;
    public List<Sprite> frames;
}