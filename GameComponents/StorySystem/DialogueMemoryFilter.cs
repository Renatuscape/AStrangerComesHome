using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMemoryFilter : MonoBehaviour
{
    public GameObject canvas;
    public Image colourWash;
    float maxAlpha;
    bool ready;

    private void Setup()
    {
        maxAlpha = colourWash.color.a;
        ready = true;
    }

    private void OnDisable()
    {
        CloseMemoryFilter();
    }

    public void CloseMemoryFilter()
    {
        StopAllCoroutines();
        canvas.SetActive(false);
    }
    public void StartMemoryFilter()
    {
        if (!ready)
        {
            Setup();
        }

        canvas.SetActive(true);
        StartCoroutine(FadeInColourWash());
    }

    IEnumerator FadeInColourWash()
    {
        float alphaIncrement = 0.001f;
        colourWash.color = new Color(colourWash.color.r, colourWash.color.g, colourWash.color.b, 0);

        while (colourWash.color.a < maxAlpha)
        {
            alphaIncrement += 0.001f;
            yield return new WaitForSeconds(0.1f);
            colourWash.color = new Color(colourWash.color.r, colourWash.color.g, colourWash.color.b, colourWash.color.a + alphaIncrement);
        }
    }
}
