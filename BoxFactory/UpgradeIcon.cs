using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Upgrade upgrade;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI levelText;

    public int level;
    public int price;

    bool showPrice = false;
    bool showLevel = false;
    bool showFloatName = false;

    public void Setup(Upgrade upgrade, bool showLevel, bool showPrice, bool showFloatName)
    {
        this.upgrade = upgrade;
        this.showLevel = showLevel;
        this.showPrice = showPrice;
        this.showFloatName = showFloatName;

        var imageComponent = gameObject.GetComponent<Image>();
        imageComponent.sprite = upgrade.sprite;

        if (!showLevel)
        {
            levelText.gameObject.SetActive(false);
        }

        if (!showPrice)
        {
            priceText.gameObject.SetActive(false);
        }

        if (showPrice || showLevel)
        {
            UpdateText();
        }
    }

    public void UpdateText()
    {
        level = Player.GetCount(upgrade.objectID, name);

        if (showLevel)
        {
            SetLevelText();
        }

        if (showPrice)
        {
            SetPriceTag();
        }

        Canvas.ForceUpdateCanvases();
    }

    public void SetLevelText()
    {
        levelText.text = "Lv. " + level.ToString();
    }
    public void SetPriceTag()
    {
        price = upgrade.GetPrice();

        priceText.text = "Price: " + price.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (showFloatName)
        {
            TransientDataScript.PrintFloatText(upgrade.name);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (showFloatName)
        {
            TransientDataScript.DisableFloatText();
        }
    }
}