using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemPrefab : MonoBehaviour
{
    public TransientDataScript transientData;
    public InventoryScript inventoryScript;

    public Item itemSource;
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

    public void EnableObject(Item item, InventoryScript script)
    {
        inventoryScript = script;
        itemSource = item;

        if (itemSource.GetInventoryAmount() > 1)
            valueText.text = $"{itemSource.GetInventoryAmount()}";
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
            inventoryScript.PrintFloatText(itemSource.name);

            inventoryScript.DisplayItemInfo(itemSource.name, " ");
            Item item = (Item)itemSource;
            inventoryScript.DisplayItemStats($"Type: {item.type}\nRarity: {itemSource.rarity}\nBase price: {itemSource.basePrice}");

        }
    }

    public void MouseClickItem()
    {
        inventoryScript.DisplayItemInfo(itemSource.name, itemSource.description);

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
