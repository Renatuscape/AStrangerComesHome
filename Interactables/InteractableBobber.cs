using System.Collections;
using UnityEngine;

public class InteractableBobber : MonoBehaviour
{
    public GameObject bobbingObject;
    public SpriteRenderer rend;
    public string animationID;
    public bool animateAndBob;
    public AnimatedSprite animatedSprite;
    public float bobHeight;
    public float bobIncrement;
    public void EnableBobber()
    {
        if (!string.IsNullOrEmpty(animationID))
        {
            animatedSprite = AnimationLibrary.GetAnimatedObject(animationID);
            var idleAnimation = animatedSprite.GetAnimationType(AnimationType.idle);
            StartCoroutine(Animate(idleAnimation));

            if (animateAndBob)
            {
                StartCoroutine(Bob(idleAnimation.frameRate));
            }
        }
        else
        {
            StartCoroutine(Bob(0.15f));
        }
    }

    IEnumerator Animate(AnimationData animationData, bool startFromCustom = false)
    {
        if (animationData != null)
        {
            int index = 0;

            if (startFromCustom)
            {
                index = animationData.customLoopStart;
            }

            int maxIndex = animationData.frames.Count - 1;

            while (index < maxIndex)
            {
                rend.sprite = animationData.frames[index];
                index++;
                yield return new WaitForSeconds(animationData.frameRate);
            }

            if (!animationData.disallowLooping)
            {
                StartCoroutine(Animate(animationData, true));
            }
        }
    }

    IEnumerator Bob(float frameRate)
    {
        float bobAmount = bobIncrement;
        while (true)
        {
            bobAmount += bobIncrement;
            yield return new WaitForSeconds(frameRate);
        }
    }
}
