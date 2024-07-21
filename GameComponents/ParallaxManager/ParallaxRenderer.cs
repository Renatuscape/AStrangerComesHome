using System;
using System.Reflection;
using UnityEngine;
[Serializable]
public class ParallaxRenderer
{
    public SpriteRenderer sky;
    public SpriteRenderer bg6;
    public SpriteRenderer bg5;
    public SpriteRenderer bg4;
    public SpriteRenderer bg3;
    public SpriteRenderer bg2;
    public SpriteRenderer bg1;
    public SpriteRenderer stationBack;
    public SpriteRenderer stationMid;
    public SpriteRenderer stationFront;
    public SpriteRenderer road;
    public SpriteRenderer fg1;
    public SpriteRenderer fg2;

    public void LoadStation()
    {

    }

    public void ApplyPackage(ParallaxLayerPackage package)
    {
        sky.sprite = package.sky;
        bg6.sprite = package.bg6;
        bg5.sprite = package.bg5;
        bg4.sprite = package.bg4;
        bg3.sprite = package.bg3;
        bg2.sprite = package.bg2;
        bg1.sprite = package.bg1;
        road.sprite = package.road;
        fg1.sprite = package.fg1;
        fg2.sprite = package.fg2;

        if (sky.sprite == null)
        {
            Debug.LogWarning("Missing sky sprite from package. Sky is obligatory.");
        }

        if (bg6.sprite == null) { bg6.gameObject.SetActive(false); } else { bg6.gameObject.SetActive(true); }
        if (bg5.sprite == null) { bg5.gameObject.SetActive(false); } else { bg5.gameObject.SetActive(true); }
        if (bg4.sprite == null) { bg4.gameObject.SetActive(false); } else { bg4.gameObject.SetActive(true); }
        if (bg3.sprite == null) { bg3.gameObject.SetActive(false); } else { bg3.gameObject.SetActive(true); }
        if (bg2.sprite == null) { bg2.gameObject.SetActive(false); } else { bg2.gameObject.SetActive(true); }
        if (bg1.sprite == null) { bg1.gameObject.SetActive(false); } else { bg1.gameObject.SetActive(true); }

        if (road.sprite == null)
        {
            Debug.LogWarning("Missing road sprite from package. Road is obligatory.");
        }

        if (fg1.sprite == null) { fg1.gameObject.SetActive(false); } else { fg1.gameObject.SetActive(true); }
        if (fg2.sprite == null) { fg2.gameObject.SetActive(false); } else { fg2.gameObject.SetActive(true); }

        ResetAllLayerPositions();
    }

    public void ResetAllLayerPositions()
    {
        Debug.Log("Reset all layer positions was called.");
        Type type = GetType();
        FieldInfo[] fields = type.GetFields();

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(Sprite))
            {
                SpriteRenderer rend = (SpriteRenderer)field.GetValue(this);
                Debug.Log($"{field.Name}: {rend?.name}");
                ResetLayerPosition(rend);
            }
        }
    }

    public void ResetLayerPosition(SpriteRenderer layer)
    {
        layer.gameObject.transform.localPosition = Vector3.zero;
    }
}
