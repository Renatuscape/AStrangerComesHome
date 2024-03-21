using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_SimpleLoop : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public List<Sprite> spriteList = new();
    public float secondsPerFrame = 0.15f;
    int frame = 0;
    float timeLeft;
    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft < 0)
        {
            Animator(spriteList);
            timeLeft = secondsPerFrame;
        }
    }

    void Animator(List<Sprite> frames, bool isReversed = false)
    {
        if (isReversed)
        {
            frame--;

            if (frame < 0)
            {
                frame = frames.Count - 1;
            }
        }
        else
        {
            frame++;

            if (frame >= frames.Count - 1)
            {
                frame = 0;
            }
        }

        spriteRenderer.sprite = frames[frame];
    }
}
