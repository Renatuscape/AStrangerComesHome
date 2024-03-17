using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyInventory : MonoBehaviour
{
    public AlchemyMenu alchemyMenu;
    public Button btnPlants;
    public Button btnCatalyst;
    public Button btnTreasure;
    public Button btnMaterial;
    public Button btnTrade;
    public Button btnAll;
    public GameObject prefabContainer;
    public List<GameObject> inventoryPrefabs = new();

    void Start()
    {
        SetUpButtons();
    }

    public void SetUpButtons()
    {
        btnPlants.onClick.AddListener(() => PrintItemCategory(ItemType.Plant));
        btnCatalyst.onClick.AddListener(() => PrintItemCategory(ItemType.Catalyst));
        btnTreasure.onClick.AddListener(() => PrintItemCategory(ItemType.Treasure));
        btnTrade.onClick.AddListener(() => PrintItemCategory(ItemType.Trade));
        btnMaterial.onClick.AddListener(() => PrintItemCategory(ItemType.Material));
        btnAll.onClick.AddListener(() => PrintAllItems());
    }

    public void PrintAllItems()
    {
        RenderInventory(ItemType.Catalyst, true);
    }

    public void PrintItemCategory(ItemType category)
    {
        RenderInventory(category);
    }

    public List<ItemIntPair> GetIngredientsInInventory(bool isDebugging = false)  //call only once at Initialisation or on Clear, and edit availableIngredients when manipulating materials.
    {
        List<ItemIntPair> availableIngredients = new();

        foreach (var item in Items.all) // exclude seeds, misc, scripts and books, and any unique item
        {
            if (item.type == ItemType.Treasure
            || item.type == ItemType.Plant
            || item.type == ItemType.Trade
            || item.type == ItemType.Catalyst
            || item.type == ItemType.Material)
            {
                if (item.rarity != ItemRarity.Unique)
                {

                    if (isDebugging)
                    {
                        availableIngredients.Add(new() { item = item, amount = 5 });
                    }
                    else
                    {
                        int amount = item.GetCountPlayer();

                        if (amount > 0)
                        {
                            availableIngredients.Add(new() { item = item, amount = amount });
                        }
                    }
                }
            }
        }

        return availableIngredients;
    }

    public void RenderInventory(ItemType pageType, bool printAll = false, bool isDebugging = false)
    {
        ClearInventory();

        foreach (var entry in alchemyMenu.availableIngredients)
        {
            if (entry.amount > 0)
            {
                if (entry.item.type == pageType || printAll || isDebugging)
                {
                    var prefab = BoxFactory.CreateItemIcon(entry.item, true, 64);
                    prefab.name = entry.item.name;
                    prefab.transform.SetParent(prefabContainer.transform, false);
                    prefab.AddComponent<AlchemyInventoryItem>();
                    var script = prefab.GetComponent<AlchemyInventoryItem>();
                    script.alchemyMenu = alchemyMenu;
                    script.item = entry.item;

                    var tag = prefab.transform.Find("Tag").gameObject;

                    TextMeshProUGUI text = tag.transform.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = $"{entry.amount}";

                    inventoryPrefabs.Add(prefab);
                }
            }
        }
    }

    void ClearInventory()
    {
        foreach (var prefab in inventoryPrefabs)
        {
            Destroy(prefab);
        }
        inventoryPrefabs = new();
    }
}
