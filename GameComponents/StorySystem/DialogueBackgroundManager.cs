using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBackgroundManager : MonoBehaviour
{
    public GameObject backgroundLetterbox;
    public Image backgroundImage;
    public Image backdrop;

    float fastFade = 0.01f;
    float slowFade = 0.02f;
    float extraSlowFade = 0.03f;

    private void OnDisable()
    {
        StopAllCoroutines();
        backgroundLetterbox.SetActive(false);
    }

    public void SetUpBackground(string backgroundID)
    {
        var spriteID = backgroundID;

        if (string.IsNullOrEmpty(spriteID))
        {
            backgroundLetterbox.SetActive(false);
        }
        else if (backgroundID.Contains("RemoveWithoutFade"))
        {
            RemoveBackground();
        }
        else if (backgroundID.Contains("Remove"))
        {
            RemoveBackgroundWithFade();
        }
        else
        {
            // CHECK BACKDROP COLOUR
            if (spriteID.Contains("-OnWhite"))
            {
                backdrop.color = Color.white;
                spriteID = spriteID.Replace("-OnWhite", "");
            }
            else if (backgroundID.Contains("-#"))
            {
                var splitString = spriteID.Split("-#");
                var hexColour = splitString[1];
                spriteID = splitString[0];

                if (ColorUtility.TryParseHtmlString(hexColour, out Color colour))
                {
                    backdrop.color = colour;
                }
                else
                {
                    Debug.Log("Could not parse hex colour for background. String was " + hexColour + ". Sprite ID after split was " + spriteID);
                }
            }
            else if (backdrop.color != Color.black)
            {
                backdrop.color = Color.black;
            }

            // CHECK FOR FADE
            if (spriteID.Contains("-WithoutFade"))
            {
                spriteID = spriteID.Replace("-WithoutFade", "");
                var foundSprite = SpriteFactory.GetBackgroundSprite(spriteID);

                if (foundSprite != null)
                {
                    SetBackground(foundSprite);
                }
                else
                {
                    backgroundLetterbox.SetActive(false);
                    Debug.Log("Background was not found with ID " + spriteID + ". Check that it exists in SpriteFactory background list.");
                }
            }
            else
            {
                float fadeSpeed = fastFade;

                if (spriteID.Contains("-ExSlowFade"))
                {
                    fadeSpeed = extraSlowFade;
                    spriteID = spriteID.Replace("-ExSlowFade", "");
                }
                else if (spriteID.Contains("-SlowFade"))
                {
                    fadeSpeed = slowFade;
                    spriteID = spriteID.Replace("-SlowFade", "");
                }

                var foundSprite = SpriteFactory.GetBackgroundSprite(spriteID);

                if (foundSprite != null)
                {
                    if (foundSprite != backgroundImage.sprite)
                    {
                        SetBackgroundWithFade(foundSprite, fadeSpeed);
                    }
                }
                else
                {
                    backgroundLetterbox.SetActive(false);
                    Debug.Log("Background was not found with ID " + backgroundID + ". Check that it exists in SpriteFactory background list.");
                }
            }
        }
    }

    void SetBackground(Sprite sprite)
    {
        backgroundImage.sprite = sprite;
        backgroundImage.color = new Color(1, 1, 1, 1);
    }

    void SetBackgroundWithFade(Sprite sprite, float fadeSpeed)
    {
        backgroundImage.color = new Color(1, 1, 1, 0);

        backgroundImage.sprite = sprite;

        backgroundLetterbox.SetActive(true);
        StartCoroutine(BackgroundFadeIn(fadeSpeed));
    }

    void RemoveBackgroundWithFade()
    {
        StartCoroutine(BackgroundFadeOut());
    }

    void RemoveBackground()
    {
        backgroundLetterbox.SetActive(false);
    }

    IEnumerator BackgroundFadeIn(float fadeSpeed)
    {
        float alpha = 0;

        while (alpha < 1)
        {
            alpha += 0.03f;
            backgroundImage.color = new Color(1, 1, 1, alpha);
            yield return new WaitForSeconds(fadeSpeed);
        }
    }

    IEnumerator BackgroundFadeOut()
    {
        float alpha = backgroundImage.color.a;

        while (alpha > 0)
        {
            alpha += 0.05f;
            backgroundImage.color = new Color(1, 1, 1, alpha);
            yield return new WaitForSeconds(0.05f);
        }

        RemoveBackground();
    }
}