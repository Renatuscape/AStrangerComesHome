using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalStatisticsPage : MonoBehaviour
{
    public FontManager fontManager;
    public DataManagerScript dataManager;
    public PlayerIcon playerIcon;
    public PlayerSprite playerSprite;
    public GameObject personaliaContainer;
    public TextMeshProUGUI namePlate;
    public TextMeshProUGUI licenseInfo;
    public GameObject upgradeContainer;
    public GameObject otherContainer;
    public List<UpgradeIcon> upgrades;
    bool upgradesSetUp = false;

    private void Start()
    {
        if (!upgradesSetUp)
        {
            BuildUpgradePage();
        }
    }
    void OnEnable()
    {
        playerIcon.playerSprite = playerSprite;
        int licenseGrade = Player.GetCount("SCR012", name);
        playerIcon.UpdateImages();

        if (Characters.all.Count > 0)
        {
            var player = Characters.FindByID("ARC000");

            if (player != null)
            {
                namePlate.text = player.ForceTrueNamePlate();
            }
            else
            {
                namePlate.text = dataManager.playerName;
            }
        }
        else
        {
            namePlate.text = dataManager.playerName;
        }

        if (licenseGrade > 0)
        {
            licenseInfo.text = $"Grade {licenseGrade} License\nExperience: {Mathf.FloorToInt((float)dataManager.totalGameDays / 28)} months";

            // namePlate.font = fontManager.header.font;
            // licenseInfo.font = fontManager.script.font;

            personaliaContainer.SetActive(true);
        }
        else
        {
            personaliaContainer.SetActive(false);
        }

        if (upgradesSetUp)
        {
            UpdateUpgradeNumbers();
        }
        else
        {
            BuildUpgradePage();
        }
    }

    void UpdateUpgradeNumbers()
    {
        foreach (var upgrade in upgrades)
        {
            upgrade.level = Player.GetCount(upgrade.upgrade.objectID, "JournalStatistics Update Numbers");
            upgrade.SetLevelText();
        }
    }

    void BuildUpgradePage()
    {
        if (Upgrades.all.Count >= 7)
        {
            int upgradeIndex = 0;

            foreach (Transform slot in upgradeContainer.transform)
            {
                Destroy(slot.gameObject.GetComponent<Image>());
                var upgrade = BoxFactory.CreateUpgradeIcon(Upgrades.all[upgradeIndex], true, false, true);
                upgrade.gameObject.transform.SetParent(slot.transform, false);
                var rect = upgrade.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(150, 150);

                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);

                rect.anchoredPosition = new Vector3(0, 0, 0);

                upgrades.Add(upgrade.gameObject.GetComponent<UpgradeIcon>());
                upgradeIndex++;
            }

            upgradesSetUp = true;
        }
    }
}
