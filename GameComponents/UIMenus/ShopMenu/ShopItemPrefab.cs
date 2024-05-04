using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemPrefab : MonoBehaviour, IPointerClickHandler
{
    public ShopMenu shopMenu;

    public Item itemSource;
    public bool isReady = false;
    public bool sellFromPlayer = false;


    public void Initialise(Item item, ShopMenu script, bool sellFromPlayer)
    {
        this.sellFromPlayer = sellFromPlayer;
        shopMenu = script;
        itemSource = item;
        isReady = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked shop item " + itemSource.name);
        if (isReady)
        {
            if (sellFromPlayer)
            {
                shopMenu.HighlightPlayerItem(itemSource);
            }
            else
            {
                shopMenu.HighlightShopItem(itemSource);
            }
        }
    }
}
