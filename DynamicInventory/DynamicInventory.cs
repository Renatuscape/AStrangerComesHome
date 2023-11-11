using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DynamicInventoryPage
{
    Catalysts,
    Seeds,
    Plants,
    Materials,
    Treasures,
    Trade,
    Misc
}
public class DynamicInventory : MonoBehaviour
{
    public RectTransform windowSize;
    public GameObject dynamicItemPrefab;
    public GameObject itemContainer;
    public TextMeshProUGUI infoCardName;
    public TextMeshProUGUI infoCardDescription;

    public bool displayCatalysts;
    public bool displaySeeds;
    public bool displayPlants;
    public bool displayMaterials;
    public bool displayTreasures;
    public bool displayTrade;
    public bool displayMisc;

    public GameObject btnCatalysts;
    public GameObject btnSeeds;
    public GameObject btnPlants;
    public GameObject btnMaterials;
    public GameObject btnTreasures;
    public GameObject btnTrade;
    public GameObject btnMisc;

    public Item activeItem;
    private void Awake()
    {
        infoCardName.text = "???";
        infoCardDescription.text = "???";
    }

    private void OnEnable()
    {
        PopulateInventoryButtons();
    }

    private void OnDisable()
    {
        UpdateWindowPosition(0, 0);
    }

    public void UpdateWindowSize(int x, int y)
    {
        windowSize.sizeDelta = new Vector2(x, y);
    }

    public void UpdateWindowPosition(float x, float y)
    {
        transform.localPosition = new Vector3(x, y, 0);
    }

    public void PopulateInventoryButtons()
    {
        SetButton(displayCatalysts, btnCatalysts);
        SetButton(displaySeeds, btnSeeds);
        SetButton(displayPlants, btnPlants);
        SetButton(displayMaterials, btnMaterials);
        SetButton(displayTreasures, btnTreasures);
        SetButton(displayTrade, btnTrade);
        SetButton(displayMisc, btnMisc);
    }
    void SetButton(bool display, GameObject obj)
    {
        if (display)
            obj.SetActive(true);
        else
            obj.SetActive(false);
    }

    public void PopulateItemContainer(DynamicInventoryPage inventoryPage)
    {
        ClearItemContainer();

        foreach (Item x in Items.allItems)
        {
            if (Player.GetItemCount(x.objectID) > 0)
            {
                if (inventoryPage == DynamicInventoryPage.Catalysts)
                {
                    if (x.type == ItemType.Catalyst)
                        InstantiateObject(x);
                }
                else if (inventoryPage == DynamicInventoryPage.Materials)
                {

                    if (x.type == ItemType.Material)
                        InstantiateObject(x);
                }
                else if (inventoryPage == DynamicInventoryPage.Misc)
                {
                    if (x.type == ItemType.Misc)
                        InstantiateObject(x);
                }
                else if (inventoryPage == DynamicInventoryPage.Plants)
                {
                    if (x.type == ItemType.Plant)
                        InstantiateObject(x);
                }
                else if (inventoryPage == DynamicInventoryPage.Seeds)
                {
                    if (x.type == ItemType.Seed)
                        InstantiateObject(x);
                }
                else if (inventoryPage == DynamicInventoryPage.Trade)
                {
                    if (x.type == ItemType.Trade)
                        InstantiateObject(x);
                }
                else if (inventoryPage == DynamicInventoryPage.Treasures)
                {
                    if (x.type == ItemType.Treasure)
                        InstantiateObject(x);
                }
            }
        }
    }

    void ClearItemContainer()
    {
        foreach (Transform child in itemContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void InstantiateObject(Item x)
    {
        var prefab = Instantiate(dynamicItemPrefab);
        prefab.name = x.name;
        prefab.transform.SetParent(itemContainer.transform, false);
        prefab.GetComponent<DynamicItemPrefab>().EnableObject(x, this);
    }

    public void SetActiveItem(Item x)
    {
        activeItem = x;
        infoCardName.text = x.name;
        infoCardDescription.text = x.description;
        Canvas.ForceUpdateCanvases();  // *
        infoCardDescription.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false; // **
        infoCardDescription.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
    }
}
