using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemPrefab : MonoBehaviour
{
    public TransientDataScript transientData;
    public InventoryScript inventoryScript;

    public MotherObject itemSource;
    public bool isReady = false;
    public TextMeshProUGUI valueText;
    public GameObject itemFrame;
    public Image displayImage;
    public Image displayShadow;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        itemFrame.SetActive(false);
    }

    public void EnableObject(MotherObject motherObject, InventoryScript script)
    {
        inventoryScript = script;
        itemSource = motherObject;

        if (itemSource.dataValue > 1)
            valueText.text = $"{itemSource.dataValue}";
        else
            valueText.text = $"";

        displayImage.sprite = itemSource.sprite;
        displayShadow.sprite = itemSource.sprite;
        isReady = true;
    }
    public void MouseOverItem()
    {
        if (isReady)
        {
            itemFrame.SetActive(true);
            inventoryScript.PrintFloatText(itemSource.printName);

            if (itemSource is Item)
            {
                inventoryScript.DisplayItemInfo(itemSource.printName, " ");
                Item item = (Item)itemSource;
                inventoryScript.DisplayItemStats($"Type: {item.type}\nRarity: {itemSource.rarity}\nBase price: {itemSource.basePrice}");
            }
        }
    }

    public void MouseClickItem()
    {
        inventoryScript.DisplayItemInfo(itemSource.printName, itemSource.longDescription);

    }

    public void MouseExitItem()
    {
        if (isReady)
        {
            itemFrame.SetActive(false);
                inventoryScript.DisableFloatText();

            if (transientData.gameState == GameState.JournalMenu && inventoryScript != null)
            {
                inventoryScript.DisplayItemInfo(" ", " ");
                inventoryScript.DisplayItemStats(" ");
            }
        }
    }
}
