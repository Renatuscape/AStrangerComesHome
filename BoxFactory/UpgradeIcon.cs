﻿using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Upgrade upgrade;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI levelText;
    public Slider wearSlider;
    public GameObject brokenIcon;

    public int level;
    public int price;

    bool showPrice = false;
    bool showLevel = false;
    bool showFloatName = false;
    bool showWearSlider = false;

    public void Setup(Upgrade upgrade, bool showLevel, bool showPrice, bool showFloatName, bool showWearSlider)
    {
        this.upgrade = upgrade;
        this.showLevel = showLevel;
        this.showPrice = showPrice;
        this.showFloatName = showFloatName;
        this.showWearSlider = showWearSlider;

        var imageComponent = gameObject.GetComponent<Image>();
        imageComponent.sprite = upgrade.sprite;
        brokenIcon.SetActive(false);

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

        if (showWearSlider)
        {
            if (Player.upgradeWear == null || Player.upgradeWear.Count < 1)
            {
                foreach (var up in Upgrades.all)
                {
                    TransientDataScript.gameManager.dataManager.upgradeWear.Add(new IdIntPair() { objectID = up.objectID, amount = 0 });
                    Player.upgradeWear = TransientDataScript.gameManager.dataManager.upgradeWear;
                }
            }

            wearSlider.gameObject.SetActive(true);
            UpdateSlider();
        }
        else
        {
            wearSlider.gameObject.SetActive(false);
        }
    }

    public void UpdateBrokenIcon()
    {
        if (upgrade.isBroken)
        {
            brokenIcon.SetActive(true);
        }
        else
        {
            brokenIcon.SetActive(false);
        }
    }
    public void UpdateSlider()
    {
        if (wearSlider != null && showWearSlider)
        {
            float maxValue = UpgradeWearTracker.CalculateMaxWear(level);

            int wear = Player.upgradeWear.FirstOrDefault(e => e.objectID == upgrade.objectID).amount;

            wearSlider.maxValue = maxValue;

            if (wear >= maxValue / 100 * 3)
            {
                wearSlider.value = maxValue - wear;
            }
            else
            {
                wearSlider.value = maxValue;
            }

            UpdateBrokenIcon();

        }
        else
        {
            Debug.Log("Could not find slider.");
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