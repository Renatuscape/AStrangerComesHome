using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleAnimationController : MonoBehaviour
{
    public SpriteRenderer rend;
    public Image img;
    public AnimatedSprite animatedSprite;

    public void SetUpController(AnimatedSprite animatedSprite)
    {
        this.animatedSprite = animatedSprite;
    }

    public void SetAnimationType(AnimationType animationType, bool loop, bool startFromCustom = false)
    {
        if (animatedSprite == null)
        {
            Debug.LogWarning("Attempted to set animation type while animatedSprite was null.");
        }
        else
        {
            var animation = animatedSprite.GetAnimationType(animationType);

            if (animation != null)
            {
                PlayAnimation(animation, loop, startFromCustom);
            }
            else
            {
                Debug.LogWarning("Could not find animation of type " + animationType.ToString());
            }
        }
    }

    public void PlayAnimation(AnimationData animationData, bool loop, bool startFromCustom = false)
    {
        if (rend != null || img != null)
        {
            StopAllCoroutines();
            StartCoroutine(Animate(animationData, loop, startFromCustom));
        }
    }

    public void FadeAlpha()
    {
        if (rend != null || img != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator Animate(AnimationData animationData, bool loop, bool startFromCustom = false)
    {
        if (animationData != null)
        {

            int index = 0;

            if (startFromCustom)
            {
                index = animationData.customLoopStart;
            }

            int maxIndex = animationData.frames.Count;

            while (index < maxIndex)
            {
                SetSprite(animationData.frames[index]);
                index++;
                yield return new WaitForSeconds(animationData.frameRate);
            }

            if (loop && !animationData.disallowLooping)
            {
                if (loop)
                {
                    SetSprite(animationData.frames[0]);
                    yield return new WaitForSeconds(animationData.delayBeforeLoop - animationData.frameRate);
                }
                StartCoroutine(Animate(animationData, true));
            }
        }
    }

    void SetSprite(Sprite sprite)
    {
        if (rend != null)
        {
            rend.sprite = sprite;
        }

        if (img != null)
        {
            img.sprite = sprite;
        }
    }

    void SetColour(Color color)
    {
        if (rend != null)
        {
            rend.color = color;
        }

        if (img != null)
        {
            img.color = color;
        }
    }

    IEnumerator FadeOut()
    {
        float alpha = 1;

        if (rend != null)
        {
            alpha = rend.color.a;
        }

        if (img != null)
        {
            alpha = img.color.a;
        }

        float fadeAmount = 0.001f;

        while (alpha >= 0)
        {
            alpha -= fadeAmount;
            SetColour(new Color(rend.color.r, rend.color.g, rend.color.b, alpha));
            fadeAmount += 0.001f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
