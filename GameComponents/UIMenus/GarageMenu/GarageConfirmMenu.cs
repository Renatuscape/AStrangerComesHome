using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
                upgradePrice.text = upgrade.GetPrice().ToString();
                upgradeTitle.text = upgrade.name + $" Lv. {level}";
            }
            else
            {
                TransientDataScript.PushAlert("Not enough money!");
            }
        }
        else
        {
            TransientDataScript.PushAlert("There is no room for further improvement.");
        }
    }

    public void Open(Upgrade upgrade)
    {
        this.upgrade = upgrade;
        level = Player.GetCount(upgrade.objectID, name);
        upgradeTitle.text = upgrade.name + $" Lv. {level}";
        upgradeDescription.text = upgrade.description;
        upgradePrice.text = upgrade.GetPrice().ToString();
        gameObject.SetActive(true);

        foreach (Transform child in upgradeContainer.transform)
        {
            Destroy(child.gameObject);
        }

        var prefab = BoxFactory.CreateUpgradeIcon(upgrade, false, false, false);
        prefab.transform.SetParent(upgradeContainer.transform);
    }
}