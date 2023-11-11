using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GardenSeedPrefab : MonoBehaviour
{
    public Item itemSource;
    public int itemAmount;
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

        valueText.text = $"{Player.GetItemCount(itemSource.objectID)}";

        isReady = true;

        SyncItemCount();
    }

    private void Update()
    {
        if (valueText.text != $"{itemAmount}")
        {
            valueText.text = $"{itemAmount}";
        }
    }

    int SyncItemCount()
    {
        return Player.GetItemCount(itemSource.objectID);
    }
    public void MouseDownItem()
    {
        if (isReady)
        {
            plantManager.SelectSeed(gameObject);
        }
        SyncItemCount();
    }
    public void MouseOverItem()
    {
        SyncItemCount();

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
