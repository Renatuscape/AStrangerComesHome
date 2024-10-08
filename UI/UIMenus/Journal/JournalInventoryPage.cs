using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class JournalInventoryPage : MonoBehaviour
{
    public PageinatedContainer inventoryContainer;
    public List<GameObject> itemPrefabs;
    public Item selectedItem;
    public GameObject itemInfo;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemRarity;
    public TextMeshProUGUI itemDescription;

    private void OnEnable()
    {
        itemName.text = "";
        itemRarity.text = "Choose an item";
        itemDescription.text = "";

        itemPrefabs = inventoryContainer.Initialise(Player.GetInventory().Where(i => i.type != ItemType.Book).ToList(), true, true, true);
        SetUpInventoryItems();
        inventoryContainer.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        inventoryContainer.gameObject.SetActive(false);
        inventoryContainer.ClearPrefabs();
    }

    private void Update()
    {
        if (inventoryContainer.selectedItem == null && selectedItem != null)
        {
            CheckSelectItem();
        }
    }

    void SetUpInventoryItems()
    {
        foreach (var prefab in itemPrefabs)
        {
            var inventoryHelper = prefab.AddComponent<PageinatedInventoryHelper>();
            inventoryHelper.inventory = this;
        }
    }
    public void CheckSelectItem()
    {
        selectedItem = inventoryContainer.selectedItem;

        if (selectedItem != null)
        {
            itemName.text = Items.GetEmbellishedItemText(selectedItem, false, false, false, false);
            itemRarity.text = selectedItem.rarity.ToString();
            itemDescription.text =  DialogueTagParser.ParseText(selectedItem.description) + (selectedItem.type == ItemType.Seed ? $"\n{Items.GetItemSeedData(selectedItem)}" : "");
        }
        else
        {
            itemName.text = "";
            itemRarity.text = "Choose an item";
            itemDescription.text = "";
        }

    }
}

public class PageinatedInventoryHelper : MonoBehaviour, IPointerClickHandler
{
    public JournalInventoryPage inventory;
    public void OnPointerClick(PointerEventData eventData)
    {
        inventory.CheckSelectItem();
    }
}