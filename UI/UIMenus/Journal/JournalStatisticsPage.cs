using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalStatisticsPage : MonoBehaviour
{
    public FontManager fontManager;
    public DataManagerScript dataManager;
    public PlayerSprite playerSprite;
    public GameObject personaliaContainer;
    public TextMeshProUGUI namePlate;
    public TextMeshProUGUI namePlateShadow;
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
        int licenseGrade = Player.GetCount("SCR012", name);

        if (Characters.all.Count > 0)
        {
            var player = Characters.FindByID("ARC000");

            if (player != null)
            {
                namePlate.text = player.ForceTrueNamePlate();
                namePlateShadow.text = player.trueName;
            }
            else
            {
                namePlate.text = dataManager.playerName;
                namePlateShadow.text = dataManager.playerName;
            }
        }
        else
        {
            namePlate.text = dataManager.playerName;
        }

        if (licenseGrade > 0)
        {
            string serviceTime;

            if (dataManager.totalGameDays > 336)
            {
                var years = Mathf.FloorToInt((float)dataManager.totalGameDays / 336);
                serviceTime = $"{years} year{(years != 1 ? "s" : "")} in service";
            }
            else
            {
                var months = Mathf.FloorToInt((float)dataManager.totalGameDays / 28);
                serviceTime = $"{months} month{(months != 1 ? "s" : "")} in service";
            }

            licenseInfo.text = $"Grade {licenseGrade} License\n{serviceTime}";

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
            upgrade.UpdateSlider();
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
                var upgrade = BoxFactory.CreateUpgradeIcon(Upgrades.all[upgradeIndex], true, false, true, true);
                upgrade.gameObject.transform.SetParent(slot.transform, false);
                var rect = upgrade.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(100, 100);

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
