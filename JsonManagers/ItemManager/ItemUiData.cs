using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUiData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
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
}
