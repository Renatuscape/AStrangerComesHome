using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum AnimationAction
{
    idle,
    idleAction,
    appear,
    disappear,
    walk,
    run,
    alert,
    chat
}

public class AnimationLibrary : MonoBehaviour
{
    public List<AnimatedSprite> animatedObject;

    public static AnimationLibrary animationLibrary;

    private void Start()
    {
        animationLibrary = GetComponent<AnimationLibrary>();
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
    public AnimationFrames idle;
    public List<AnimationFrames> frames;
}

[Serializable]
public class AnimationFrames
{
    public string name;
    public AnimationAction type;
    public bool disallowLooping;
    public float frameRate = 0.1f;
    public List<Sprite> frames;
}