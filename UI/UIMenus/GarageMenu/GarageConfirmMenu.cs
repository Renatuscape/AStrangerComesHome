using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageConfirmMenu : MonoBehaviour
{
    public GarageMenu garageMenu;
    public UpgradeIcon upgradeIcon;
    public Button btnUpgrade;
    public Button btnRepair;
    public TextMeshProUGUI upgradeTitle;
    public TextMeshProUGUI upgradeDescription;
    public TextMeshProUGUI upgradePrice;
    public TextMeshProUGUI repairPrice;
    public GameObject repairInfo;
    public GameObject upgradeContainer;
    public bool isSetUp = false;
    int level; // track level here to avoid delay issues when upgrading
    int calculatedRepairPrice;

    public void SetUp(GarageMenu garageMenu)
    {
        if (!isSetUp)
        {
            this.garageMenu = garageMenu;
            btnUpgrade.onClick.AddListener(() => UpgradeButton());
            btnRepair.onClick.AddListener(() => RepairButton());
            gameObject.SetActive(false);
            isSetUp = true;
        }
    }

    public void UpgradeButton()
    {

        if (level < 10)
        {
            if (garageMenu.AttemptUpgrade(upgradeIcon.upgrade))
            {
                level++;
                upgradePrice.text = upgradeIcon.upgrade.GetPrice().ToString();
                upgradeTitle.text = upgradeIcon.upgrade.name + $" Lv. {level}";
                AudioManager.PlaySoundEffect("bellCopperHigh");
            }
            else
            {
                LogAlert.QueueTextAlert("Not enough money!");
                AudioManager.PlaySoundEffect("knockSmall");
            }
        }
        else
        {
            LogAlert.QueueTextAlert("There is no room for further improvement.");
            AudioManager.PlaySoundEffect("pingGlassy");
        }
    }

    public void RepairButton()
    {
        if (UpgradeWearTracker.RepairUpgrade(upgradeIcon.upgrade.objectID, calculatedRepairPrice))
        {
            LogAlert.QueueTextAlert(upgradeIcon.upgrade.name + " has been repaired.");
            repairInfo.gameObject.SetActive(false);
            btnRepair.gameObject.SetActive(false);
            upgradeIcon.UpdateSlider();  //upgradeIcon.wearSlider.value = 0;
            AudioManager.PlaySoundEffect("pingGlassy");
        }
        else
        {
            LogAlert.QueueTextAlert("I don't have enough money to repair this.");
            AudioManager.PlaySoundEffect("knockSmall");
        }
    }

    public void Open(Upgrade upgrade)
    {
        foreach (Transform child in upgradeContainer.transform)
        {
            Destroy(child.gameObject);
        }

        var prefab = BoxFactory.CreateUpgradeIcon(upgrade, false, false, false, true);
        prefab.transform.SetParent(upgradeContainer.transform);
        var script = prefab.GetComponent<UpgradeIcon>();
        upgradeIcon = script;

        level = Player.GetCount(upgradeIcon.upgrade.objectID, name);
        upgradeIcon.level = level;
        upgradeIcon.UpdateSlider();

        upgradeTitle.text = upgradeIcon.upgrade.name + $" Lv. {level}";
        upgradeDescription.text = upgradeIcon.upgrade.description;
        upgradeIcon.UpdateSlider();

        if (level < 10)
        {
            upgradePrice.text = upgrade.GetPrice().ToString();
        }
        else
        {
            upgradePrice.text = "Fully Upgraded";
        }

        calculatedRepairPrice = UpgradeWearTracker.CalculateRepairPrice(upgrade.objectID);

        if (calculatedRepairPrice >= 10)
        {
            repairPrice.text = calculatedRepairPrice.ToString();
            repairInfo.gameObject.SetActive(true);
            btnRepair.gameObject.SetActive(true);
        }
        else
        {
            repairInfo.gameObject.SetActive(false);
            btnRepair.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
    }
}