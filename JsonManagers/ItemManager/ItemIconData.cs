using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIconData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI priceText;
    public Image[] images;
    public Image itemSprite;
    public Image itemShadow;
    public bool disableFloatText;
    public bool printPrice;
    public bool printRarity;
    public bool printType;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!disableFloatText)
        {
            Items.PrintFloatEmbellishedItem(item, printPrice, printRarity, printType);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!disableFloatText)
        {
            TransientDataScript.DisableFloatText();
        }
    }

    public void EnableCount(bool enable, int value)
    {
        if (enable)
        {
            countText.text = value.ToString();
            priceText.transform.parent.gameObject.SetActive(false);
            countText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            countText.transform.parent.gameObject.SetActive(false);
        }

    }

    public void UpdateCount(int value)
    {
        countText.text = value.ToString();
    }

    public void EnablePrice(bool enable, int value)
    {
        if (enable)
        {
            priceText.text = value.ToString();
            priceText.transform.parent.gameObject.SetActive(true);
            countText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            priceText.transform.parent.gameObject.SetActive(false);
        }
    }

    public void UpdatePrice(int value)
    {
        priceText.text = value.ToString();
    }

    public void Return(string caller)
    {
        //Debug.Log("Attempting to return ItemIconData with gameobject name " + gameObject.name);

        var alchemyDraggable = gameObject.GetComponent<AlchemyDraggableItem>();
        if (alchemyDraggable != null)
        {
            Destroy(alchemyDraggable);
        }

        var shopDraggable = gameObject.GetComponent<DraggableShopItem>();
        if (shopDraggable != null)
        {
            Destroy(shopDraggable);
        }

        var shopItem = gameObject.GetComponent<ShopItemPrefab>();
        if (shopItem != null)
        {
            Destroy(shopItem);
        }

        var giftItem = gameObject.GetComponent<GiftItem>();
        if (giftItem != null)
        {
            Destroy(giftItem);
        }

        priceText.transform.parent.gameObject.SetActive(false);
        BoxFactory.ReturnItemIconToPool(this, caller);
    }
    private void OnDestroy()
    {
        Debug.LogWarning("Item icon named " + gameObject.name + " was destroyed! Fix old code, please :(");
    }
}
