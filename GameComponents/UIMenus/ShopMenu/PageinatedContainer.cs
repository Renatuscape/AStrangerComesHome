using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageinatedContainer : MonoBehaviour
{
    public GameObject stockContainer;
    List<GameObject> prefabs;
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
            Destroy(child.gameObject);
        }

        btnPageLeft.onClick.AddListener(() => ChangePage(true));
        btnPageRight.onClick.AddListener(() => ChangePage(false));
    }

    public List<GameObject> Initialise(List<string> stock, bool showInventoryCount)
    {
        List<Item> items = new();

        foreach (string stockID in stock)
        {
            items.Add(Items.FindByID(stockID));
        }

        return Initialise(items, showInventoryCount);
    }
    public List<GameObject> Initialise(List<Item> stock, bool showInventoryCount)
    {
        this.showInventoryCount = showInventoryCount;
        ClearPrefabs();
        pageIndex = 0;

        if (stock != null)
        {
            if (stock.Count == 0)
            {
                stock = Items.all.Where(x => x.notBuyable == false).ToList();
            }

            // CREATE LIST OF PAGES
            containerPages = new List<ContainerPage>();
            SetUpContent();
        }

        return prefabs;
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
            SpawnPageContent(pageIndex);
    }

    void SetUpContent()
    {
        stock = stock.OrderBy(obj => obj.rarity).ToList();

        var foundCatalysts = stock.Where(x => x.type == ItemType.Catalyst).ToList();
        var foundMaterials = stock.Where(x => x.type == ItemType.Material).ToList();
        var foundSeeds = stock.Where(x => x.type == ItemType.Seed).ToList();
        var foundPlants = stock.Where(x => x.type == ItemType.Plant).ToList();
        var foundTrade = stock.Where(x => x.type == ItemType.Trade).ToList();
        var foundTreasures = stock.Where(x => x.type == ItemType.Treasure).ToList();
        var foundBooks = stock.Where(x => x.type == ItemType.Book).ToList();

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
        if (foundList.Count == 0)
            return; // Skip generating empty pages

        var newPage = new ContainerPage();
        newPage.pageContent = new List<Item>();
        newPage.typeName = pageName;
        containerPages.Add(newPage);

        var pageNumber = 1;

        foreach (Item item in foundList)
        {
            var currentPage = containerPages[containerPages.Count - 1];

            if (currentPage.pageContent.Count == itemsPerPage)
            {
                pageNumber++;

                var nextPage = new ContainerPage();
                nextPage.pageContent = new List<Item>();
                nextPage.typeName = pageName + " " + pageNumber;
                containerPages.Add(nextPage);

                currentPage = nextPage;
            }

            if (currentPage.pageContent.Count < itemsPerPage)
            {
                currentPage.pageContent.Add(item);
            }
        }
    }

    void SpawnPageContent(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < containerPages.Count)
        {
            stockContainer.GetComponent<GridLayoutGroup>().enabled = false;
            foreach (Transform child in stockContainer.transform)
            {
                Destroy(child.gameObject);
            }
            stockContainer.GetComponent<GridLayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();
            stockContainer.GetComponent<GridLayoutGroup>().enabled = false;

            foreach (Item item in containerPages[pageIndex].pageContent)
            {
                var prefab = BoxFactory.CreateItemIcon(item, showInventoryCount, 64);
                prefab.name = item.name;
                prefab.transform.SetParent(stockContainer.transform, false);
                prefabs.Add(prefab);
            }

            stockContainer.GetComponent<GridLayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();

            categoryName.text = containerPages[pageIndex].typeName;
        }
    }

    void ClearPrefabs()
    {
        foreach (GameObject pref in prefabs)
        {
            Destroy(pref);
        }
        prefabs.Clear();
    }
}
