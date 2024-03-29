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
    int rhetorics;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        rhetorics = Player.GetCount("MAG002", "ShopMenu");
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
        float rhetoricsDiscount = Mathf.Clamp(rhetorics * 0.02f, 0f, 0.20f); // 0.02f represents 2%, 0.20f represents 20%
        float finalPrice = newPrice - (newPrice * rhetoricsDiscount);

        priceAdjusted = (int)Mathf.Ceil(finalPrice);
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
