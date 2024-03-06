using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexibleBackgroundAdjuster : MonoBehaviour
{
    public RectTransform targetRect;
    public RectTransform thisTransform;
    public bool adjustHeight;
    public float addedHeight;
    public float maxHeight;
    public bool adjustWidth;
    public float addedWidth;
    public float maxWidth;

    void Start()
    {
        thisTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (adjustHeight)
        {
            if (maxHeight > 0 && targetRect.sizeDelta.y + addedHeight > maxHeight)
            {
                thisTransform.sizeDelta = new Vector2(thisTransform.sizeDelta.x, maxHeight);
            }
            else if (maxHeight == 0 || targetRect.sizeDelta.y + addedHeight < maxHeight) //if no max, always adjust
            {
                thisTransform.sizeDelta = new Vector2(thisTransform.sizeDelta.x, targetRect.sizeDelta.y + addedHeight);
            }
        }
        if (adjustWidth)
        {
            if (maxWidth > 0 && targetRect.sizeDelta.x + addedWidth > maxWidth)
            {
                thisTransform.sizeDelta = new Vector2(maxWidth, thisTransform.sizeDelta.y);
            }
            else if (maxWidth == 0 || targetRect.sizeDelta.x + addedWidth < maxWidth) //if no max, always adjust
            {
                thisTransform.sizeDelta = new Vector2(targetRect.sizeDelta.x + addedWidth, thisTransform.sizeDelta.y);
            }

        }
    }
}
