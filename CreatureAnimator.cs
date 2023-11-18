using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAnimator : MonoBehaviour
{
    public TransientDataScript transientData;
    public SpriteRenderer spriteRenderer;
    public List<Sprite> idleSprites = new();
    public List<Sprite> walkSprites = new();
    public List<Sprite> runSprites = new();

    float timePerFrame = 0.1f;
    float timeLeft = 0.2f;
    int frame = 0;

    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            //RUN
            if (transientData.currentSpeed > 6)
            {
                timePerFrame = 0.05f;
                Animator(runSprites);
            }
            else if (transientData.currentSpeed > 4.5f)
            {
                timePerFrame = 0.07f;
                Animator(runSprites);
            }
            else if (transientData.currentSpeed > 3)
            {
                timePerFrame = 0.1f;
                Animator(runSprites);
            }
            //WALK
            else if (transientData.currentSpeed > 2)
            {
                timePerFrame = 0.07f;
                Animator(walkSprites);
            }
            else if (transientData.currentSpeed > 0)
            {
                timePerFrame = 0.1f;
                Animator(walkSprites);
            }
            //REVERSE
            else if (transientData.currentSpeed < 0)
            {
                timePerFrame = 0.15f;
                Animator(walkSprites, true);
            }
            //IDLE
            else if (transientData.currentSpeed < 0.01f)
            {
                timePerFrame = 0.1f;
                Animator(idleSprites);
            }
            timeLeft = timePerFrame;
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
