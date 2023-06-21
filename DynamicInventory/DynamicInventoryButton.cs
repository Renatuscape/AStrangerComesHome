using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicInventoryButton : MonoBehaviour
{
    public DynamicInventory dynamicInventory;
    public DynamicInventoryPage thisPage;

public void ButtonDown()
    {
        dynamicInventory.PopulateItemContainer(thisPage);
    }
}
