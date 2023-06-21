using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GardenSeedPrefab : MonoBehaviour
{
    public MotherObject itemSource;
    public PlantingManager plantManager;
    public bool isReady = false;
    public TextMeshProUGUI valueText;

    public Image displayImage;
    public Image displayShadow;
    public void EnableObject(Seed seed, PlantingManager script)
    {
        itemSource = seed;
        plantManager = script;

        displayImage.sprite = itemSource.sprite;
        displayShadow.sprite = itemSource.sprite;

        valueText.text = $"{itemSource.dataValue}";

        isReady = true;
    }

    private void Update()
    {
        if (valueText.text != $"{itemSource.dataValue}")
        {
            valueText.text = $"{itemSource.dataValue}";
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
