using System.Collections;
using UnityEngine;

public class StarPrefab : MonoBehaviour
{
    public SpriteRenderer rend;
    public bool flicker = false;
    public float maxAlpha = 1;
    public float minAlpha = 0.2f;
    public float flickerForce = 0.4f;
    public bool setUpComplete = false;
    public bool isPuttingOut = false;
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        maxAlpha = Random.Range(minAlpha, 1);

        if (Random.Range(0, 100) > 50)
        {
            flicker = true;

            if (maxAlpha < flickerForce)
            {
                maxAlpha = flickerForce + 0.2f;
            }
        }

        if (Random.Range(0, 100) > 95)
        {
            rend.color = Color.cyan;
        }
        else if (Random.Range(0, 100) > 98)
        {
            rend.color = Color.magenta;
        }

        StartCoroutine(FadeInOnStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (setUpComplete && flicker && !isPuttingOut)
        {
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, Random.Range(maxAlpha - flickerForce, maxAlpha));
        }
    }

    internal void PutOut()
    {
        isPuttingOut = true;
        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FadeInOnStart()
    {
        float alpha = 0;
        float fadeValue = 0.01f;

        while (alpha < maxAlpha)
        {
            alpha += fadeValue;
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);
            fadeValue += 0.001f;

            yield return new WaitForSeconds(0.05f);
        }

        setUpComplete = true;
    }
    IEnumerator FadeAndDestroy()
    {
        float alpha = rend.color.a;
        float fadeValue = 0.01f;

        while (alpha > 0)
        {
            alpha -= fadeValue;
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);
            fadeValue += 0.001f;

            yield return new WaitForSeconds(0.05f);
        }
        Destroy(gameObject);
    }
}
