using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUiData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    public TextMeshProUGUI numberMesh;
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
}
