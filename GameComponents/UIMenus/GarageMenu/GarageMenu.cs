using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GarageMenu : MonoBehaviour
{
    public PortraitRenderer portraitRenderer;
    public Character character;
    public GameObject mechUpContainer;
    public GameObject magicUpContainer;
    public GarageConfirmMenu confirmMenu;
    public List<UpgradeIcon> upgradeList;
    public Upgrade selectedUpgrade;

    bool upgradesLoaded = false;

    void Start()
    {
        if (TransientDataScript.GameState != GameState.ShopMenu)
        {
            gameObject.SetActive(false);
        }

        confirmMenu.SetUp(this);
    }

    public void Initialise(Character character)
    {
        this.character = character;
        TransientDataCalls.SetGameState(GameState.ShopMenu, name, gameObject);
        portraitRenderer.EnableForGarage(character.objectID);

        confirmMenu.gameObject.SetActive(false);
        gameObject.SetActive(true);

        Debug.Log(gameObject);

        if (!upgradesLoaded)
        {
            foreach (Upgrade upgrade in Upgrades.all)
            {
                var prefab = BoxFactory.CreateUpgradeIcon(upgrade, true, true, true);

                if (upgrade.type == UpgradeType.Magical)
                {
                    prefab.transform.SetParent(magicUpContainer.transform, false);
                }
                else
                {
                    prefab.transform.SetParent(mechUpContainer.transform, false);
                }

                prefab.AddComponent<GarageUpgradePrefab>();
                var script = prefab.GetComponent<GarageUpgradePrefab>();
                script.Initialise(this, upgrade);

                upgradeList.Add(prefab.GetComponent<UpgradeIcon>());
            }

            upgradesLoaded = true;
        }
        else
        {
            foreach (var upgrade in upgradeList)
            {
                upgrade.UpdateText();
            }
        }
    }

    public void SelectUpgrade(Upgrade upgrade)
    {
        selectedUpgrade = upgrade;
        confirmMenu.Open(upgrade);
    }

    public bool AttemptUpgrade(Upgrade upgrade)
    {
        if (MoneyExchange.Purchase(upgrade.GetPrice()))
        {
            Player.Add(upgrade.objectID);

            var upg = upgradeList.FirstOrDefault(u => u.upgrade.objectID == upgrade.objectID);
            upg.UpdateText();

            return true;
        }
        else
        {
            return false;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        TransientDataCalls.SetGameState(GameState.Overworld, name, gameObject);
    }
}
