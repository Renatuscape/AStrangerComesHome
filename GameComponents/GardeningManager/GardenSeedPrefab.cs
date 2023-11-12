using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GardenSeedPrefab : MonoBehaviour
{
    public Item itemSource;
    public PlantingManager plantManager;
    public bool isReady = false;
    public TextMeshProUGUI valueText;

    public Image displayImage;
    public Image displayShadow;
    public void EnableObject(Item seed, PlantingManager script)
    {
        itemSource = seed;
        plantManager = script;

        displayImage.sprite = itemSource.sprite;
        displayShadow.sprite = itemSource.sprite;

        valueText.text = $"{itemSource.GetInventoryAmount()}";

        isReady = true;
    }

    private void Update()
    {
        if (valueText.text != $"{itemSource.GetInventoryAmount()}")
        {
            valueText.text = $"{itemSource.GetInventoryAmount()}";
        }
    }

    public void MouseDownItem()
    {
        if (isReady)
        {
            plantManager.SelectSeed(gameObject);
        }
    }
    public void MouseOverItem()
    {
        if (isReady)
        {
            //some kind of text that displays the object's name ($"{itemSource.printName}");
        }
    }

    public void MouseExitItem()
    {
        if (isReady)
        {
            //clear whatever text you figured out;
        }
    }
}
