using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalInventoryPage : MonoBehaviour
{
    public GameObject dynamicInventory;
    public DynamicInventory inventory;

    private void OnEnable()
    {
        inventory.displayCatalysts = true;
        inventory.displaySeeds = true;
        inventory.displayPlants = true;
        inventory.displayMaterials = true;
        inventory.displayTreasures = true;
        inventory.displayTrade = true;
        inventory.displayMisc = true;

        dynamicInventory.SetActive(true);
        inventory.gameObject.SetActive(true);
        inventory.UpdateWindowSize(450, 200);
        inventory.UpdateWindowPosition(0, -45);
        inventory.UpdateButtonContainerWidth(480);
        inventory.PopulateItemContainer(DynamicInventoryPage.Catalysts);
    }

    private void OnDisable()
    {
        if (dynamicInventory != null)
        {
            dynamicInventory.SetActive(false);
        }
    }
}
