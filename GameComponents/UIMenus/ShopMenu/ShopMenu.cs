using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static ShopMenu;
using System;
using TMPro;

public class ShopMenu : MonoBehaviour
{
    public Character shopkeeper;
    public Shop activeShop;
    public int judgement;
    public DataManagerScript dataManager;
    public PortraitRenderer portraitRenderer; //remember to use .gameObject for the object
    public float profitMargin;

    public GameObject clearanceNotice;
    public GameObject shopItemPrefab;
    public StorySystem dialogueSystem;

    public GameObject shelf;
    public GameObject specialShelfA; //special item
    public GameObject specialShelfB;
    public GameObject specialShelfC;
    public GameObject specialShelfD;

    public List<ShopPage> shopPages;
    public TextMeshProUGUI categoryName;
    public int pageIndex;
    public List<Item> shopInventory;
    public List<GameObject> spawnedPrefabs;

    void Awake()
    {
        profitMargin = 2f;
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
    }

    private void OnEnable()
    {
        if (shopkeeper != null && activeShop != null && !string.IsNullOrEmpty(shopkeeper.objectID) && !string.IsNullOrEmpty(activeShop.objectID))
        {
            SetUpShop(shopkeeper, activeShop);
        }
    }
    private void OnDisable()
    {
        foreach (var item in spawnedPrefabs)
        {
            Destroy(item);
        }

        TransientDataScript.DisableFloatText();
    }

    public void SetUpShop(Character shopkeeper, Shop shop)
    {
        TransientDataScript.SetGameState(GameState.ShopMenu, name, gameObject);
        transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);

        this.shopkeeper = shopkeeper;
        activeShop = shop;

        EnablePortraits();
        TransientDataScript.DisableFloatText();
        pageIndex = 0;

        shelf.GetComponent<GridLayoutGroup>().enabled = false;
        shelf.GetComponent<GridLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();

        if (activeShop != null)
        {
            //CALCULATE SHOP RATE
            if (activeShop.saleDay == TransientDataScript.GetWeekDay())
            {
                clearanceNotice.SetActive(true);
                profitMargin = activeShop.clearanceMargin - (judgement * 0.5f);
            }
            else
            {
                clearanceNotice.SetActive(false);
                profitMargin = activeShop.profitMargin - judgement;
            }

            if (profitMargin < 0)
            {
                profitMargin = 0;
            }


            //SET UP SHOP WITH PAGES
            shopInventory = activeShop.GetInventory();

            if (shopInventory != null)
            {
                //Set cell width
                //shelf.GetComponent<GridLayoutGroup>().cellSize = new Vector2(shopObject.cellWidth, 32);

                //Sort items by rarity
                shopInventory = shopInventory.OrderBy(obj => obj.rarity).ToList();


                //CREATE LIST OF PAGES
                shopPages = new List<ShopPage>();
                SortShopObjects();


                void SortShopObjects()
                {
                    if (shopInventory.Count == 0)
                    {
                        shopInventory = Items.all.Where(x=> x.notBuyable == false).ToList();
                    }
                    var foundCatalysts = shopInventory.Where(x => x.type == ItemType.Catalyst).ToList();
                    var foundMaterials = shopInventory.Where(x => x.type == ItemType.Material).ToList();
                    var foundSeeds = shopInventory.Where(x => x.type == ItemType.Seed).ToList();
                    var foundPlants = shopInventory.Where(x => x.type == ItemType.Plant).ToList();
                    var foundTrade = shopInventory.Where(x => x.type == ItemType.Trade).ToList();
                    var foundTreasures = shopInventory.Where(x => x.type == ItemType.Treasure).ToList();
                    var foundBooks = shopInventory.Where(x => x.type == ItemType.Book).ToList();


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

                    var newPage = new ShopPage();
                    newPage.pageContent = new List<Item>();
                    newPage.typeName = pageName;
                    shopPages.Add(newPage);

                    var pageNumber = 1;

                    foreach (Item x in foundList)
                    {
                        var currentPage = shopPages[shopPages.Count - 1];

                        if (currentPage.pageContent.Count == 9)
                        {
                            pageNumber++;

                            var nextPage = new ShopPage();
                            nextPage.pageContent = new List<Item>();
                            nextPage.typeName = pageName + " " + pageNumber;
                            shopPages.Add(nextPage);

                            currentPage = nextPage;
                        }

                        if (currentPage.pageContent.Count < 9)
                        {
                            currentPage.pageContent.Add(x);
                        }
                    }
                }

                SpawnShopItems(pageIndex);
            }
        }
    }

    void SpawnShopItems(int pageIndex)
    {
        SyncSkills();

        if (pageIndex >= 0 && pageIndex < shopPages.Count)
        {
            shelf.GetComponent<GridLayoutGroup>().enabled = false;
            foreach (Transform child in shelf.transform)
            {
                Destroy(child.gameObject);
            }
            shelf.GetComponent<GridLayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();
            shelf.GetComponent<GridLayoutGroup>().enabled = false;

            foreach (Item x in shopPages[pageIndex].pageContent)
            {
                var shelf = this.shelf;

                var prefab = Instantiate(shopItemPrefab);
                prefab.name = x.name;
                prefab.transform.SetParent(shelf.transform, false);
                prefab.GetComponent<ShopItemPrefab>().priceIncreasePercent = profitMargin;
                prefab.GetComponent<ShopItemPrefab>().EnableObject(x, this);
                spawnedPrefabs.Add(prefab);
            }

            shelf.GetComponent<GridLayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();

            categoryName.text = shopPages[pageIndex].typeName;
        }

    }

    void EnablePortraits()
    {
        portraitRenderer.gameObject.SetActive(true);
        portraitRenderer.EnableForShop(shopkeeper.objectID);
    }

    public void AttemptPurchase(Item item, int itemCost)
    {
        var inventoryAmount = Player.GetCount(item.objectID, name);

        if (inventoryAmount < item.maxValue)
        {
            var purchase = MoneyExchange.Purchase(itemCost);

            if (purchase)
            {
                Player.Add(item.objectID);
                AudioManager.PlayUISound("handleCoins");
                Debug.Log($"{activeShop.sucessfulPurchaseText} You purchased {item.name} for {itemCost}.");
                TransientDataScript.PushAlert($"Purchased {item.name}. I now have {Player.GetCount(item.objectID, name)} total.");
            }
            else
            {
                Debug.Log("Exchange returned false.");
                TransientDataScript.PushAlert("I don't have enough money.");
            }

        }
        else
        {
            Debug.Log("I already have the maximum amount of this item.");
            TransientDataScript.PushAlert("I don't have enough space for more of this.");
        }

    }

    void SyncSkills()
    {
        judgement = Player.GetCount("ATT002", "ShopMenu");
    }
    public void ChangePage(bool pageBack)
    {
        var oldIndex = pageIndex;

        if (pageBack)
        {
            if (pageIndex > 0)
                pageIndex--;
            else
                pageIndex = shopPages.Count - 1;
        }
        else
        {
            if (pageIndex < shopPages.Count - 1)
                pageIndex++;
            else
                pageIndex = 0;
        }

        if (oldIndex != pageIndex)
            SpawnShopItems(pageIndex);
    }

    public void PrintFloatText(string text)
    {
        TransientDataScript.PrintFloatText(text);
    }

    public void DisableFloatText()
    {
        TransientDataScript.DisableFloatText();
    }

    public void ChatButton()
    {
        dialogueSystem.OpenTopicMenu(shopkeeper.objectID);
    }
}


[System.Serializable]
public struct ShopPage
{
    public string typeName;
    public List<Item> pageContent;
}