using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemPrefab : MonoBehaviour
{
    public TransientDataScript transientData;
    public ShopMenu shopMenu;

    public Item itemSource;
    public bool isReady = false;
    public TextMeshProUGUI valueText;
    public GameObject frameOval;
    public GameObject frameRound;
    public GameObject itemFrame;
    public GameObject buyButton;
    public Image displayImage;
    public Image displayShadow;
    public float priceIncreasePercent;

    int priceAdjusted;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        frameOval.SetActive(false);
        frameRound.SetActive(false);
        itemFrame.SetActive(false);
        buyButton.SetActive(false);
    }

    public void EnableObject(Item item, ShopMenu script)
    {
        shopMenu = script;
        itemSource = item;
        CalculatePrice();

        displayImage.sprite = itemSource.sprite;
        displayShadow.sprite = itemSource.sprite;
        itemFrame = frameRound;
        isReady = true;
    }

    public void CalculatePrice()
    {
        var newPrice = itemSource.basePrice * (1 + priceIncreasePercent / 100);
        priceAdjusted = (int)Mathf.Ceil(newPrice);
        //Debug.Log($"Price {itemSource.basePrice} adjusted by {priceIncreasePercent}% to {priceAdjusted}");

        valueText.text = $"{priceAdjusted}";
    }
    public void MouseOverItem()
    {
        if (isReady)
        {
            itemFrame.SetActive(true);
            shopMenu.PrintFloatText($"{itemSource.name}");
        }
    }

    public void MouseClickItem()
    {
        if (isReady)
        {
            if (buyButton.activeInHierarchy == true)
            {
                shopMenu.AttemptPurchase(itemSource, priceAdjusted);
                buyButton.SetActive(false);
            }
            else
                buyButton.SetActive(true);

            CalculatePrice();
        }
    }

    public void MouseExitItem()
    {
        if (isReady)
        {
            itemFrame.SetActive(false);
            buyButton.SetActive(false);
            shopMenu.DisableFloatText();
        }
    }
}
