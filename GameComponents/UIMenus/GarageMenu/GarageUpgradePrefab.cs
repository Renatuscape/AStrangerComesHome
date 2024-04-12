using UnityEngine;
using UnityEngine.EventSystems;

public class GarageUpgradePrefab : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    public GarageMenu garageMenu;
    public Upgrade upgrade;
    public bool isClicked;

    public void Initialise(GarageMenu garageMenu, Upgrade upgrade)
    {
        this.garageMenu = garageMenu;
        this.upgrade = upgrade;
        isClicked = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClicked)
        {
            garageMenu.SelectUpgrade(upgrade);
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