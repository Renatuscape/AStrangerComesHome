using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_ScaleOnEnable : MonoBehaviour
{
    public Vector3 startScale;
    public Vector3 endScale;
    public float duration = 0.5f;
    public bool destroyOnEnd;

    private void OnEnable()
    {
        StartCoroutine(ScaleOverTime());
    }

    IEnumerator ScaleOverTime()
    {
        float startTime = Time.time;
        float endTime = startTime + duration;
        while (Time.time < endTime)
        {
            float timeRatio = (Time.time - startTime) / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, timeRatio);
            yield return null;
        }
        transform.localScale = endScale;

        if (destroyOnEnd)
        {
            Destroy(gameObject);
        }
    }
    private void OnDisable()
    {
        if (!destroyOnEnd)
        {
            transform.localScale = startScale;
        }
    }
}
