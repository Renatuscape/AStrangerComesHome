using System.Collections;
using UnityEngine;

public class InteractableNode : MonoBehaviour
{
    public string nodeID;
    public string animationID;
    public bool hideOnLoot;
    public bool hideBobber;
    public InteractableBobber interactBobber;
    public SpriteRenderer rend;
    public BoxCollider2D col;
    public AnimatedSprite animatedSprite;

    internal IEnumerator FadeOutObject()
    {
        float alpha = rend.color.a;
        float fadeAmount = 0.001f;

        while (rend.color.a > 0)
        {
            alpha -= fadeAmount;
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);
            fadeAmount += 0.001f;
            yield return new WaitForSeconds(0.01f);
        }

        gameObject.SetActive(false);
    }

    internal IEnumerator AnimateAndFadeOut(AnimationData animationData, bool fadeAlpha)
    {
        StartCoroutine(Animate(animationData, false));
        float animationTime = animationData.frameRate * animationData.frames.Count;

        if (fadeAlpha)
        {
            float fadeBegin = animationTime - 0.44f;
            yield return new WaitForSeconds(fadeBegin);

            StartCoroutine(FadeOutObject());
        }
        else
        {
            yield return new WaitForSeconds(animationTime);
            gameObject.SetActive(false);
        }
    }

    internal IEnumerator Animate(AnimationData animationData, bool loop, bool startFromCustom = false)
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

            if (loop && !animationData.disallowLooping)
            {
                StartCoroutine(Animate(animationData, true));
            }
        }
    }
}
