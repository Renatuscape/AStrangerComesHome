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
    public float priceMultiplier;

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

        /*if (itemSource is Upgrade)
        {
            displayImage.rectTransform.sizeDelta = new Vector2(64, 32);
            displayShadow.rectTransform.sizeDelta = new Vector2(64, 32);
            displayShadow.transform.localPosition = new Vector3(1, 1, 0);
            itemFrame = frameOval;
        }*/
        isReady = true;
    }

    public void CalculatePrice()
    {
        float price = itemSource.basePrice * priceMultiplier;

        //if (itemSource.type == ItemType.Upgrade)
          //  price = itemSource.basePrice * (1 + (itemSource.GetInventoryAmount() * itemSource.GetInventoryAmount())) * priceMultiplier;//price = itemSource.basePrice * shopMenu.priceMultiplier * (1 + (itemSource.dataValue * 3));

        priceAdjusted = (int)Mathf.Floor(price);
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
