using UnityEngine;
using UnityEngine.EventSystems;

public class GarageUpgradePrefab : MonoBehaviour, IPointerClickHandler
{
    public GarageMenu garageMenu;
    public UpgradeIcon upgradeIcon;

    public void Initialise(GarageMenu garageMenu, UpgradeIcon upgrade)
    {
        this.garageMenu = garageMenu;
        upgradeIcon = upgrade;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        garageMenu.SelectUpgrade(upgradeIcon);
    }
}