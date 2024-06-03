using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBackgroundManager : MonoBehaviour
{
    public GameObject backgroundLetterbox;
    public Image backgroundImage;

    private void OnDisable()
    {
        StopAllCoroutines();
        backgroundLetterbox.SetActive(false);
    }

    public void SetUpBackground(string backgroundID)
    {
        if (string.IsNullOrEmpty(backgroundID))
        {
            backgroundLetterbox.SetActive(false);
        }
        else if (backgroundID.ToLower().Contains("removewithoutfade"))
        {
            RemoveBackground();
        }
        else if (backgroundID.ToLower().Contains("remove"))
        {
            RemoveBackgroundWithFade();
        }
        else
        {
            if (backgroundID.Contains("-WithoutFade"))
            {
                var spriteID = backgroundID.Replace("-WithoutFade", "");
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
                var foundSprite = SpriteFactory.GetBackgroundSprite(backgroundID);

                if (foundSprite != null)
                {
                    if (foundSprite != backgroundImage.sprite)
                    {
                        SetBackgroundWithFade(foundSprite);
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

    void SetBackgroundWithFade(Sprite sprite)
    {
        backgroundImage.color = new Color(1, 1, 1, 0);

        backgroundImage.sprite = sprite;

        backgroundLetterbox.SetActive(true);
        StartCoroutine(BackgroundFadeIn());
    }

    void RemoveBackgroundWithFade()
    {
        StartCoroutine(BackgroundFadeOut());
    }

    void RemoveBackground()
    {
        backgroundLetterbox.SetActive(false);
    }

    IEnumerator BackgroundFadeIn()
    {
        float alpha = 0;

        while (alpha < 1)
        {
            alpha += 0.03f;
            backgroundImage.color = new Color(1, 1, 1, alpha);
            yield return new WaitForSeconds(0.01f);
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