using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PageinatedContainer : MonoBehaviour
{
    public GameObject stockContainer;
    public List<GameObject> prefabMasterList = new();
    public List<Item> stock;
    public int itemsPerPage = 28;

    public List<ContainerPage> containerPages;
    public TextMeshProUGUI categoryName;
    public Button btnPageRight;
    public Button btnPageLeft;

    public int pageIndex;

    bool showInventoryCount;

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
    }

    public List<GameObject> Initialise(List<string> stock, bool showInventoryCount, bool skipNotBuyable, bool skipNotSellable)
    {
        List<Item> items = new();

        foreach (string stockID in stock)
        {
            items.Add(Items.FindByID(stockID));
        }

        return Initialise(items, showInventoryCount, skipNotBuyable, skipNotSellable);
    }
    public List<GameObject> Initialise(List<Item> incomingStock, bool showInventoryCount, bool skipNotBuyable, bool skipNotSellable)
    {
        Debug.Log("Attempting to initialise container " + gameObject.name);
        this.showInventoryCount = showInventoryCount;
        ClearPrefabs();
        pageIndex = 0;

        stock = incomingStock;

        if (stock == null || stock.Count == 0)
        {
            stock = Items.all.Where(x => x.notBuyable == false).ToList();
        }
        // CREATE LIST OF PAGES
        containerPages = new List<ContainerPage>();
        SetUpContent(skipNotBuyable, skipNotSellable);

        foreach (var page in  containerPages)
        {
            SpawnPagePrefabs(page);
        }

        OpenPage(0);

        return prefabMasterList;
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
            OpenPage(pageIndex);
    }

    void SetUpContent(bool skipNotBuyable, bool skipNotSellable)
    {
        stock = stock.OrderBy(obj => obj.rarity).ThenBy(obj => obj.name).ToList();

        if (skipNotSellable)
        {
            stock = stock.Where(i => !i.notSellable).ToList();
        }

        if (skipNotBuyable)
        {
            stock = stock.Where(i => !i.notBuyable).ToList();
        }

        var foundCatalysts = stock.Where(i => i.type == ItemType.Catalyst).ToList();
        var foundMaterials = stock.Where(i => i.type == ItemType.Material).ToList();
        var foundSeeds = stock.Where(i => i.type == ItemType.Seed).ToList();
        var foundPlants = stock.Where(i => i.type == ItemType.Plant).ToList();
        var foundTrade = stock.Where(i => i.type == ItemType.Trade).ToList();
        var foundTreasures = stock.Where(i => i.type == ItemType.Treasure).ToList();
        var foundBooks = stock.Where(i => i.type == ItemType.Book).ToList();

        GeneratePages(foundMaterials, "Materials");
        GeneratePages(foundCatalysts, "Catalysts");
        GeneratePages(foundSeeds, "Seeds");
        GeneratePages(foundPlants, "Plants");
        GeneratePages(foundTreasures, "Treasures");
        GeneratePages(foundBooks, "Books");
        GeneratePages(foundTrade, "Trade");
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
            var prefab = BoxFactory.CreateItemIcon(item, showInventoryCount, 64, 18, true);
            prefab.name = item.name;
            prefab.transform.SetParent(stockContainer.transform, false);
            prefabMasterList.Add(prefab);
            page.prefabs.Add(prefab);
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
                    prefab.gameObject.SetActive(true);
                }
            }

            categoryName.text = containerPages[pageIndex].typeName;
        }
    }

    void ClearPrefabs()
    {
        // Debug.Log("Clear prefabs was called on pageinated container.");
        foreach (GameObject pref in prefabMasterList)
        {
            Destroy(pref);
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
