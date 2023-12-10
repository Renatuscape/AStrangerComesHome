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
    public Shop shopObject;
    public string shopkeeperID;
    public int merchantile;
    public TransientDataScript transientData;
    public DataManagerScript dataManager;
    public float priceMultiplier;

    public GameObject clearanceNotice;
    public GameObject shopItemPrefab;
    public DialogueSystem dialogueSystem;

    public GameObject shelf;
    public GameObject specialShelfA; //special item
    public GameObject specialShelfB;
    public GameObject specialShelfC;
    public GameObject specialShelfD;

    public List<ShopPage> shopPages;
    public TextMeshProUGUI categoryName;
    public int pageIndex;

    [System.Serializable]
    public struct ShopPage
    {
        public string typeName;
        public List<Item> pageContent;
    }

    void Awake()
    {
        priceMultiplier = 2f;
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
    }

    void SyncSkills()
    {
        merchantile = Player.GetCount("ATT002", "ShopMenu");
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
                prefab.GetComponent<ShopItemPrefab>().priceMultiplier = priceMultiplier;
                prefab.GetComponent<ShopItemPrefab>().EnableObject(x, this);
            }

            shelf.GetComponent<GridLayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();

            categoryName.text = shopPages[pageIndex].typeName;
        }

    }

    private void OnEnable()
    {
        transientData.DisableFloatText();
        pageIndex = 0;

        shelf.GetComponent<GridLayoutGroup>().enabled = false;
        shelf.GetComponent<GridLayoutGroup>().enabled = true;

        if (transientData.currentShop != null)
        {
            shopObject = transientData.currentShop;
        }

        if (shopObject != null)
        {
            //CALCULATE SHOP RATE
            if (shopObject.saleDay == transientData.weekDay)
            {
                clearanceNotice.SetActive(true);
                priceMultiplier = 1.5f - (merchantile * 0.1f);
            }
            else
            {
                clearanceNotice.SetActive(false);
                priceMultiplier = 2f - (merchantile * 0.1f);
            }

            //SET UP SHOP WITH PAGES
            shopObject.SetupShop();

            if (shopObject.shopInventory != null)
            {
                //Set cell width
                shelf.GetComponent<GridLayoutGroup>().cellSize = new Vector2(shopObject.cellWidth, 32);

                //Sort items by rarity
                var objectList = shopObject.shopInventory.OrderBy(obj => obj.rarity).ToList();


                //CREATE LIST OF PAGES
                shopPages = new List<ShopPage>();
                SortShopObjects();


                void SortShopObjects()
                {
                    var foundCatalysts = new List<Item>();
                    var foundMaterials = new List<Item>();
                    var foundSeeds = new List<Item>();
                    var foundPlants = new List<Item>();
                    var foundTrade = new List<Item>();
                    var foundTreasures = new List<Item>();
                    var foundUpgrades = new List<Item>();

                    foreach (Item item in objectList)
                    {
                        if (item.type == ItemType.Catalyst)
                        {
                            foundCatalysts.Add(item);
                        }
                        else if (item.type == ItemType.Material)
                        {
                            foundMaterials.Add(item);
                        }
                        else if (item.type == ItemType.Seed)
                        {
                            foundSeeds.Add(item);
                        }
                        else if (item.type == ItemType.Plant)
                        {
                            foundPlants.Add(item);
                        }
                        else if (item.type == ItemType.Trade)
                        {
                            foundTrade.Add(item);
                        }
                        else if (item.type == ItemType.Treasure)
                        {
                            foundTreasures.Add(item);
                        }
                    }

                    GeneratePages(foundMaterials, "Materials");
                    GeneratePages(foundCatalysts, "Catalysts");
                    GeneratePages(foundSeeds, "Seeds");
                    GeneratePages(foundPlants, "Plants");
                    GeneratePages(foundTreasures, "Treasures");
                    GeneratePages(foundUpgrades, "Upgrades");
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

    private void OnDisable()
    {
        foreach (Transform child in shelf.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in specialShelfA.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in specialShelfB.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in specialShelfC.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in specialShelfD.transform)
        {
            Destroy(child.gameObject);
        }

        transientData.DisableFloatText();
    }

    public void AttemptPurchase(Item item, int itemCost)
    {
        if (dataManager.playerGold >= itemCost)
        {
            //Add confirm menu
            dataManager.playerGold -= itemCost;
            item.AddToPlayer();
            Debug.Log($"{shopObject.sucessfulPurchaseText} You purchased {item.name} for {itemCost}. You now have {item.GetCountPlayer()} and your remaining gold is {dataManager.playerGold}");
        }
        else
            Debug.Log(shopObject.notEnoguhMoneyText);
    }

    public void PrintFloatText(string text)
    {
        transientData.PrintFloatText(text);
    }

    public void DisableFloatText()
    {
        transientData.DisableFloatText();
    }

    public void ChatButton()
    {
        dialogueSystem.OpenTopicMenu(shopkeeperID);
    }
}
