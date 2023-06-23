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
    public TransientDataScript transientData;
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
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        infoCardName.text = "???";
        infoCardDescription.text = "???";
    }

    public void UpdateWindowSize(int x, int y)
    {
        windowSize.sizeDelta = new Vector2(x, y);
    }

    public void UpdateWindowPosition(float x, float y)
    {
        transform.localPosition = new Vector3(x, y, 0);
    }

    private void OnEnable()
    {
        PopulateInventoryButtons();
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

        foreach (MotherObject x in transientData.objectIndex)
        {
            if (x.dataValue > 0)
            {
                if (inventoryPage == DynamicInventoryPage.Catalysts)
                {
                    if (x is Catalyst)
                        InstantiateObject((Item)x);
                }
                else if (inventoryPage == DynamicInventoryPage.Materials)
                {

                    if (x is Material)
                        InstantiateObject((Item)x);
                }
                else if (inventoryPage == DynamicInventoryPage.Misc)
                {
                    if (x is Misc)
                        InstantiateObject((Item)x);
                }
                else if (inventoryPage == DynamicInventoryPage.Plants)
                {
                    if (x is Plant)
                        InstantiateObject((Item)x);
                }
                else if (inventoryPage == DynamicInventoryPage.Seeds)
                {
                    if (x is Seed)
                        InstantiateObject((Item)x);
                }
                else if (inventoryPage == DynamicInventoryPage.Trade)
                {
                    if (x is Trade)
                        InstantiateObject((Item)x);
                }
                else if (inventoryPage == DynamicInventoryPage.Treasures)
                {
                    if (x is Treasure)
                        InstantiateObject((Item)x);
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
        prefab.name = x.printName;
        prefab.transform.SetParent(itemContainer.transform, false);
        prefab.GetComponent<DynamicItemPrefab>().EnableObject(x, this);
    }

    public void SetActiveItem(Item x)
    {
        activeItem = x;
        infoCardName.text = x.printName;
        infoCardDescription.text = x.longDescription;
        Canvas.ForceUpdateCanvases();  // *
        infoCardDescription.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false; // **
        infoCardDescription.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
    }
}
