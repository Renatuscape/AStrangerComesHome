using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicItemPrefab : MonoBehaviour
{
    public Item itemSource;
    public DynamicInventory dynamicInventory;
    public bool isReady = false;
    public TextMeshProUGUI itemCount;
    public GameObject itemFrame;

    public Image displayImage;
    public Image displayShadow;
    public void EnableObject(Item item, DynamicInventory script)
    {
        itemSource = item;
        dynamicInventory = script;

        displayImage.sprite = itemSource.sprite;
        displayShadow.sprite = itemSource.sprite;

        itemCount.text = $"{Player.GetCount(itemSource.objectID, name)}";

        isReady = true;
    }

    private void Update()
    {
        if (dynamicInventory.activeItem != itemSource)
        {
            itemFrame.SetActive(false);
        }
    }
    public void MouseDownItem()
    {
        if (isReady)
        {
            dynamicInventory.SetActiveItem(itemSource);
            itemFrame.SetActive(true);
        }
    }

    public void MouseOverItem()
    {
        if (isReady)
        {
            TransientDataScript.PrintFloatEmbellishedItem(itemSource, true, true);
        }
    }

    public void MouseExitItem()
    {
        if (isReady)
        {
            TransientDataScript.DisableFloatText();
        }
    }
}
