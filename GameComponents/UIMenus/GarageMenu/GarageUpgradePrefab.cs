using UnityEngine;
using UnityEngine.EventSystems;

public class GarageUpgradePrefab : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    public GarageMenu garageMenu;
    public UpgradeIcon upgradeIcon;
    public bool isClicked;

    public void Initialise(GarageMenu garageMenu, UpgradeIcon upgrade)
    {
        this.garageMenu = garageMenu;
        upgradeIcon = upgrade;
        isClicked = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClicked)
        {
            garageMenu.SelectUpgrade(upgradeIcon);
        }
        else
        {
            isClicked = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isClicked = false;
    }
}