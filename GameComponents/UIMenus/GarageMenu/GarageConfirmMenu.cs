using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class GarageConfirmMenu : MonoBehaviour
{
    public GarageMenu garageMenu;
    public Upgrade upgrade;
    public Button btnUpgrade;
    public TextMeshProUGUI upgradeTitle;
    public TextMeshProUGUI upgradeDescription;
    public TextMeshProUGUI upgradePrice;
    public GameObject upgradeContainer;
    public bool isSetUp = false;
    int level; // track level here to avoid delay issues when upgrading

    public void SetUp(GarageMenu garageMenu)
    {
        if (!isSetUp)
        {
            this.garageMenu = garageMenu;
            btnUpgrade.onClick.AddListener(() => UpgradeButton());
            gameObject.SetActive(false);
            isSetUp = true;
        }
    }

    public void UpgradeButton()
    {

        if (level < 10)
        {
            if (garageMenu.AttemptUpgrade(upgrade))
            {
                level++;
                upgradePrice.text = "Price: " + upgrade.GetPrice();
                upgradeTitle.text = upgrade.name + $" Lv. {level}";
            }
            else
            {
                TransientDataCalls.PushAlert("Not enough money!");
            }
        }
        else
        {
            TransientDataCalls.PushAlert("There is no room for further improvement.");
        }
    }

    public void Open(Upgrade upgrade)
    {
        this.upgrade = upgrade;
        level = Player.GetCount(upgrade.objectID, name);
        upgradeTitle.text = upgrade.name + $" Lv. {level}";
        upgradeDescription.text = upgrade.description;
        upgradePrice.text = "Price: " + upgrade.GetPrice();
        gameObject.SetActive(true);

        foreach (Transform child in upgradeContainer.transform)
        {
            Destroy(child.gameObject);
        }

        var prefab = BoxFactory.CreateUpgradeIcon(upgrade, false, false, false);
        prefab.transform.SetParent(upgradeContainer.transform);
    }
}