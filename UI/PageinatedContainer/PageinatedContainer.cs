using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageinatedContainer : MonoBehaviour
{
    public GameObject stockContainer;
    public GameObject selectorFrame;
    public Item selectedItem;
    public List<GameObject> prefabMasterList = new();
    public List<Item> stock;
    public int itemsPerPage = 28;
    public string noItemsMessage = "No Suitable Items";

    public List<ContainerPage> containerPages;
    public TextMeshProUGUI categoryName;
    public Button btnPageRight;
    public Button btnPageLeft;

    public int pageIndex;

    bool showInventoryCount;
    bool useDefaultFloat;
    bool useSelectorFrame;

    private void Awake()
    {
        // Clear out all dummy items
        foreach (Transform child in stockContainer.transform)
        {
            if (child.name.ToLower().Contains("placeholder"))
            Destroy(child.gameObject);
        }

        btnPageLeft.onClick.AddListener(() => ChangePage(true));
        btnPageRight.onClick.AddListener(() => ChangePage(false));

        if (string.IsNullOrEmpty(noItemsMessage))
        {
            noItemsMessage = "No Suitable Items";
        }
    }

    public List<GameObject> Initialise(List<string> stock, bool showInventoryCount, bool useDefaultFloat, bool useSelectorFrame)
    {
        List<Item> items = new();

        foreach (string stockID in stock)
        {
            items.Add(Items.FindByID(stockID));
        }

        return Initialise(items, showInventoryCount, useDefaultFloat, useSelectorFrame);
    }

    public List<GameObject> Initialise(List<Item> incomingStock, bool showInventoryCount, bool useDefaultFloat, bool useSelectorFrame, bool printAllIfStockIsEmpty = false)
    {
        // Debug.Log("Attempting to initialise container " + gameObject.name);
        this.showInventoryCount = showInventoryCount;
        this.useDefaultFloat = useDefaultFloat;
        this.useSelectorFrame = useSelectorFrame;

        ClearPrefabs();
        ClearSeletion();

        pageIndex = 0;

        stock = incomingStock;

        if (stock == null || stock.Count == 0)
        {
            if (printAllIfStockIsEmpty)
            {
                Debug.Log("Adding all items to stock for " + gameObject.name);
                stock = Items.all.Where(x => x.notBuyable == false).ToList();
                btnPageRight.gameObject.SetActive(true);
                btnPageLeft.gameObject.SetActive(true);
            }
            else
            {
                categoryName.text = noItemsMessage;
                btnPageRight.gameObject.SetActive(false);
                btnPageLeft.gameObject.SetActive(false);
            }
        }
        else
        {
            btnPageRight.gameObject.SetActive(true);
            btnPageLeft.gameObject.SetActive(true);
        }

        // CREATE LIST OF PAGES
        containerPages = new List<ContainerPage>();
        SetUpContent();

        foreach (var page in  containerPages)
        {
            SpawnPagePrefabs(page);
        }

        OpenPage(0);

        return prefabMasterList;
    }

    public void SelectItem(ItemIconData itemData)
    {
        selectedItem = itemData.item;
        selectorFrame.transform.position = itemData.gameObject.transform.position;
        selectorFrame.gameObject.SetActive(true);
    }

    public void ChangePage(bool pageBack)
    {
        var oldIndex = pageIndex;

        if (pageBack)
        {
            if (pageIndex > 0)
                pageIndex--;
            else
                pageIndex = containerPages.Count - 1;
        }
        else
        {
            if (pageIndex < containerPages.Count - 1)
                pageIndex++;
            else
                pageIndex = 0;
        }

        if (oldIndex != pageIndex)
        {
            OpenPage(pageIndex);
        }

        ClearSeletion();
    }

    void SetUpContent()
    {
        stock = stock.OrderBy(obj => obj.rarity).ThenBy(obj => obj.name).ToList();

        var foundCatalysts = stock.Where(i => i.type == ItemType.Catalyst).ToList();
        var foundMaterials = stock.Where(i => i.type == ItemType.Material).ToList();
        var foundSeeds = stock.Where(i => i.type == ItemType.Seed).ToList();
        var foundPlants = stock.Where(i => i.type == ItemType.Plant).ToList();
        var foundTrade = stock.Where(i => i.type == ItemType.Trade).ToList();
        var foundTreasures = stock.Where(i => i.type == ItemType.Treasure).ToList();
        var foundBooks = stock.Where(i => i.type == ItemType.Book).ToList();
        var foundMisc = stock.Where(i => i.type == ItemType.Misc).OrderBy(obj => obj.objectID).ToList();

        GeneratePages(foundMaterials, "Materials");
        GeneratePages(foundCatalysts, "Catalysts");
        GeneratePages(foundSeeds, "Seeds");
        GeneratePages(foundPlants, "Plants");
        GeneratePages(foundTreasures, "Treasures");
        GeneratePages(foundBooks, "Books");
        GeneratePages(foundTrade, "Trade");
        GeneratePages(foundMisc, "Miscellaneous");
    }

    void GeneratePages(List<Item> foundList, string pageName)
    {
        // Debug.Log("Attempting to generate page for list with " + foundList.Count + " entries named " + pageName);
        if (foundList.Count == 0)
            return; // Skip generating empty pages

        var newPage = new ContainerPage();
        newPage.pageContent = new List<Item>();
        newPage.typeName = pageName;
        containerPages.Add(newPage);

        var pageIndex = 1;

        foreach (Item item in foundList)
        {
            var currentPage = containerPages[containerPages.Count - 1];

            if (currentPage.pageContent.Count == itemsPerPage)
            {
                pageIndex++;

                var nextPage = new ContainerPage();
                nextPage.pageContent = new List<Item>();
                nextPage.typeName = pageName + " " + pageIndex;
                containerPages.Add(nextPage);

                currentPage = nextPage;
            }

            if (currentPage.pageContent.Count < itemsPerPage)
            {
                currentPage.pageContent.Add(item);
            }
        }
    }

    void SpawnPagePrefabs(ContainerPage page)
    {
        page.prefabs = new();

        foreach (Item item in page.pageContent)
        {
            var uiScript = BoxFactory.CreateItemIcon(item, showInventoryCount, 64, 18);
            var prefab = uiScript.gameObject;
            prefab.name = item.name;
            prefab.transform.SetParent(stockContainer.transform, false);
            prefabMasterList.Add(prefab);
            page.prefabs.Add(prefab);

            if (useSelectorFrame)
            {
                var selector = prefab.AddComponent<PageinatedSelectorFrame>();
                selector.parentClass = this;
                selector.itemUiData = uiScript;
            }

            if (!useDefaultFloat)
            {
                uiScript.disableFloatText = true;
            }
        }
        // Debug.Log("Spawned " + page.prefabs.Count + " prefabs for " + page.typeName);
    }

    public void OpenPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < containerPages.Count)
        {
            foreach (Transform child in stockContainer.transform)
            {
                child.gameObject.SetActive(false);
            }

            if (containerPages[pageIndex].prefabs != null && containerPages[pageIndex].prefabs.Count > 0)
            {
                foreach (var prefab in containerPages[pageIndex].prefabs)
                {
                    if (prefab != null)
                    {
                        prefab.gameObject.SetActive(true);
                    }
                    else
                    {
                        containerPages[pageIndex].prefabs.Remove(prefab);
                        prefabMasterList.Remove(prefab);
                    }
                }
            }

            categoryName.text = containerPages[pageIndex].typeName;
        }
    }

    public void ClearSeletion()
    {
        if (selectorFrame != null)
        {
            selectedItem = null;
            selectorFrame.gameObject.SetActive(false);
        }
        else
        {
            if (useSelectorFrame)
            {
                Debug.LogWarning("Use Selector Frame was enabled, but selectorFrame object was null. Disabling useSelectorFrame.");
                useSelectorFrame = false;
            }
        }
    }

    public void ClearPrefabs()
    {
        // Debug.Log("Clear prefabs was called on pageinated container.");
        foreach (GameObject pref in prefabMasterList)
        {
            Destroy(pref.GetComponent<PageinatedSelectorFrame>());
            pref.GetComponent<ItemIconData>().Return("PageinatedContainer on ClearPrefabs.");
        }

        prefabMasterList.Clear();
        containerPages.Clear();
    }
}

[System.Serializable]
public class ContainerPage
{
    public string typeName;
    public List<Item> pageContent;
    public List<GameObject> prefabs;
}

public class PageinatedSelectorFrame : MonoBehaviour, IPointerDownHandler
{
    public PageinatedContainer parentClass;
    public ItemIconData itemUiData;

    public void OnPointerDown(PointerEventData eventData)
    {
        parentClass.SelectItem(itemUiData);
    }
}