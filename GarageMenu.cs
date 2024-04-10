using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageMenu : MonoBehaviour
{
    public GameObject prefabContainer;
    public GameObject upgradePrefab;
    public List<GameObject> prefabList;
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Initialise(Character character)
    {
        foreach (Upgrade upgrade in Upgrades.all)
        {
            var prefab = Instantiate(upgradePrefab);
            prefab.transform.SetParent(prefabContainer.transform, false);
            prefabList.Add(prefab);

            prefab.AddComponent<UpgradePrefab>();
            var script = prefab.GetComponent<UpgradePrefab>();

            script.Setup(upgrade, true, true);
        }
    }
}

public class UpgradePrefab : MonoBehaviour
{
    public Upgrade upgrade;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI levelText;

    public int level;
    public int price;

    bool showPrice = false;
    bool showLevel = false;

    public void Setup(Upgrade upgrade, bool showLevel, bool showPrice)
    {
        this.upgrade = upgrade;
        this.showLevel = showLevel;
        this.showPrice = showPrice;

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
    }

    public void SetLevelText()
    {
        levelText.text = level.ToString();
    }
    public void SetPriceTag()
    {
        price = upgrade.basePrice + (level * upgrade.basePrice * 5);

        priceText.text = price.ToString();
    }
}