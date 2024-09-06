using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBackgroundManager : MonoBehaviour
{
    readonly Dictionary<string, Action> behaviour = new();
    public Image bgImage;
    public Image bgSolid;

    float fastFade = 0.07f;
    float normalFade = 0.03f;
    float slowFade = 0.005f;

    float fadeSpeed;

    private void Start()
    {
        fadeSpeed = normalFade;
        InitialiseTags();
    }
    private void InitialiseTags()
    {
        behaviour["SolidBlack"] = () => SolidBlack();
        behaviour["SolidWhite"] = () => SolidWhite();
        behaviour["FadeSolidIn"] = () => StartCoroutine(FadeIn(bgSolid));
        behaviour["FadeSolidOut"] = () => StartCoroutine(FadeOut(bgSolid));
        behaviour["FadeImageIn"] = () => StartCoroutine(FadeIn(bgImage));
        behaviour["FadeImageOut"] = () => StartCoroutine(FadeOut(bgImage));
        behaviour["OnWhite"] = () => { bgSolid.color = Color.white; };
    }

    void SolidBlack()
    {
        Debug.Log("BGM: Solid back was called. Image set to transparent.");
        bgSolid.color = new Color(0, 0, 0, 1);
        bgImage.color = new Color(1, 1, 1, 0);
    }

    void SolidWhite()
    {
        Debug.Log("BGM: Solid white was called. Image set to transparent.");
        bgSolid.color = new Color(1, 1, 1, 1);
        bgImage.color = new Color(1, 1, 1, 0);
    }

    private void OnDisable()
    {
        ClearBackground();
    }

    public void ClearBackground()
    {
        Debug.Log("BGM: Clear background was called. Image and solid set to transparent.");
        StopAllCoroutines();
        bgSolid.color = new Color(0, 0, 0, 0);
        bgImage.color = new Color(1, 1, 1, 0);
        bgImage.sprite = null;
    }
    public void SetUpBackground(string backgroundID)
    {
        Debug.Log("Attempting to set up background with ID " + backgroundID);

        if (string.IsNullOrEmpty(backgroundID))
        {

        }
        else if (backgroundID.Contains('-'))
        {
            BuildBgFromTags(backgroundID.Split('-'));
        }
        else if (behaviour.TryGetValue(backgroundID, out var value))
        {
            value.Invoke();
        }
        else
        {
            Sprite background = SpriteFactory.GetBackgroundSprite(backgroundID);

            if (background != null && bgImage.sprite != background)
            {
                fadeSpeed = normalFade;
                SetBackground(background);
                StartCoroutine(FadeIn(bgImage));
            }

            // Do nothing if the background is null. Leave current background as is
            // Also do nothing if backgorund is the same with no behaviour commands
        }
    }

    void BuildBgFromTags(string[] tags)
    {
        Sprite background = SpriteFactory.GetBackgroundSprite(tags[0]);

        if (background != null)
        {
            SetBackground(background);
        }

        bool speedFound = false;

        foreach (string tag in tags)
        {
            if (!speedFound)
            {
                if (tag == "Fast")
                {
                    fadeSpeed = fastFade;
                    speedFound = true;
                    break;
                }
                else if (tag == "Slow")
                {
                    fadeSpeed = slowFade;
                    speedFound = true;
                    break;
                }
            }
        }

        if (!speedFound) { fadeSpeed = normalFade; }

        foreach (string tag in tags)
        {
            if (behaviour.TryGetValue(tag, out var value))
            {
                value.Invoke();
            }
            else if (tag.Contains("Image_HEX#"))
            {
                bgImage.color = TransientDataScript.GetColourFromHex(tag.Replace("Image_HEX#", ""));
            }
            else if (tag.Contains("Solid_HEX#"))
            {
                bgSolid.color = TransientDataScript.GetColourFromHex(tag.Replace("Solid_HEX#", ""));
            }
        }
    }

    void SetBackground(Sprite sprite)
    {
        bgImage.sprite = sprite;
        bgSolid.color = Color.black;
        bgImage.color = new Color(1, 1, 1, 1);
    }

    IEnumerator FadeIn(Image target)
    {
        Debug.Log("BGM: Attempting to fade in " + target);
        Debug.Log("BGM: Fade speed is " + fadeSpeed);

        float alpha = 0;

        while (alpha < 1)
        {
            target.color = new Color(target.color.r, target.color.g, target.color.b, alpha);
            alpha += fadeSpeed;
            yield return new WaitForSeconds(fadeSpeed);
        }

        Debug.Log("BGM: Completed fade in of " + target);
    }

    IEnumerator FadeOut(Image target)
    {
        Debug.Log("BGM: Attempting to fade out " + target);
        Debug.Log("BGM: Fade speed is " + fadeSpeed);
        float alpha = target.color.a;

        while (alpha > 0)
        {
            target.color = new Color(target.color.r, target.color.g, target.color.b, alpha);
            alpha -= fadeSpeed;
            yield return new WaitForSeconds(0.05f);
        }

        Debug.Log("BGM: Completed fade out of " + target);
    }
}