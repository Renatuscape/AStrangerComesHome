using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableLootCrate : MonoBehaviour
{
    public string crateID;
    public string animationID;
    public AnimatedSprite animatedSprite;
    public List<InteractableLootBundle> bundles;
    public InteractableLootBundle loadedBundle;
    public BoxCollider2D col;
    public SpriteRenderer rend;
    public bool neverHideGraphic;
    public bool allowCloseAfterUse;
    public bool looted;
    private void Start()
    {
        crateID = gameObject.name;
        loadedBundle = null;
        col = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();

        if (!string.IsNullOrEmpty(animationID))
        {
            animatedSprite = AnimationLibrary.GetAnimatedObject(animationID);
        }

        if (animatedSprite != null)
        {
            if (animatedSprite.still != null)
            {
                rend.sprite = animatedSprite.still;
            }
        }

        if (bundles != null && bundles.Count > 0)
        {
            CheckAllBundles();
        }
        else
        {
            DisableLootCrate(true);
        }
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.GameState == GameState.Overworld)
        {
            if (!looted)
            {
                loadedBundle.YieldOutput();
                looted = true;
                DisableLootCrate(false);
            }
            else
            {
                if (allowCloseAfterUse)
                {
                    var closingAnimation = animatedSprite.GetAnimationType(AnimationType.close);

                    if (closingAnimation != null)
                    {
                        AnimateAction(closingAnimation, true);
                    }
                    else
                    {
                        Debug.Log("Attempted to close, but no animation was found. Disabling collider.");
                        col.enabled = false;
                    }
                }
            }
        }
    }

    public void CheckAllBundles()
    {
        List<InteractableLootBundle> passingBundles = new();

        for (int i = 0; i < bundles.Count; i++)
        {
            if (bundles[i].SetupAndCheck(crateID + "_" + i))
            {
                if (bundles[i].isPrioritised)
                {
                    loadedBundle = bundles[i];
                    looted = false;
                    break;
                }
                else
                {
                    passingBundles.Add(bundles[i]);
                }
            }
        }

        if (loadedBundle == null)
        {
            if (passingBundles.Count > 0)
            {
                loadedBundle = passingBundles[Random.Range(0, passingBundles.Count)];
                looted = false;
            }
            else
            {
                DisableLootCrate(true);
                Debug.Log("No viable loot was found");
            }
        }
    }

    void DisableLootCrate(bool skipFade)
    {
        if (!allowCloseAfterUse)
        {
            col.enabled = false;
        }

        if (!skipFade)
        {
            if (animatedSprite != null)
            {
                var useAnimation = animatedSprite.GetAnimationType(AnimationType.open, true);

                if (useAnimation == null)
                {
                    useAnimation = animatedSprite.GetAnimationType(AnimationType.use, true);
                }
                if (useAnimation == null)
                {
                    useAnimation = animatedSprite.GetAnimationType(AnimationType.disappear, true);
                }

                if (useAnimation != null)
                {
                    StopAllCoroutines();
                    StartCoroutine(AnimateAction(useAnimation, false));
                }
                else
                {
                    Debug.Log("No apropriate animations were found for opening or disappearing the crate.");

                    if (!neverHideGraphic)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
            else if (!neverHideGraphic)
            {
                Debug.Log("Animated Sprite was null.");
                gameObject.SetActive(false);
            }
        }
        else if (skipFade && !neverHideGraphic)
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator AnimateLoop(AnimationData animationData, bool startFromCustom = false)
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
                StartCoroutine(AnimateLoop(animationData, true));
            }
        }
    }

    IEnumerator AnimateAction(AnimationData animationData, bool resumeIdleAfter)
    {
        if (animationData != null)
        {
            int index = 0;
            int maxIndex = animationData.frames.Count - 1;

            while (index < maxIndex)
            {
                rend.sprite = animationData.frames[index];
                index++;
                yield return new WaitForSeconds(animationData.frameRate);
            }
        }

        if (!neverHideGraphic)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(FadeOutObject());
        }
        else if (resumeIdleAfter)
        {
            var idleAnimation = animatedSprite.animationData.FirstOrDefault(f => f.type == AnimationType.idle);

            if (idleAnimation != null)
            {
                AnimateLoop(idleAnimation);
            }
        }
    }

    IEnumerator FadeOutObject()
    {
        float alpha = rend.color.a;
        float fadeAmount = 0.001f;

        while (rend.color.a > 0)
        {
            alpha -= fadeAmount;
            rend.color = new Color (rend.color.r, rend.color.g, rend.color.b, alpha);
            fadeAmount += 0.001f;
            yield return new WaitForSeconds(0.01f);
        }

        gameObject.SetActive(false);
    }
}
