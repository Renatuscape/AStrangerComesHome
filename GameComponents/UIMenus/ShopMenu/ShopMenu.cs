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
    public Skill merchantile;
    public TransientDataScript transientData;
    public DataManagerScript dataManager;
    public float priceMultiplier;

    public GameObject clearanceNotice;
    public GameObject shopItemPrefab;
    public TopicManager topicManager;

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
        public List<MotherObject> pageContent;
    }

    void Awake()
    {
        priceMultiplier = 2f;
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
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

            foreach (MotherObject x in shopPages[pageIndex].pageContent)
            {
                var shelf = this.shelf;

                var prefab = Instantiate(shopItemPrefab);
                prefab.name = x.printName;
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
                priceMultiplier = 1.5f - (merchantile.dataValue * 0.1f);
            }
            else
            {
                clearanceNotice.SetActive(false);
                priceMultiplier = 2f - (merchantile.dataValue * 0.1f);
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
                    var foundCatalysts = new List<MotherObject>();
                    var foundMaterials = new List<MotherObject>();
                    var foundSeeds = new List<MotherObject>();
                    var foundPlants = new List<MotherObject>();
                    var foundTrade = new List<MotherObject>();
                    var foundTreasures = new List<MotherObject>();
                    var foundUpgrades = new List<MotherObject>();

                    foreach (MotherObject x in objectList)
                    {
                        if (x is Catalyst)
                        {
                            foundCatalysts.Add(x);
                        }
                        else if (x is Material)
                        {
                            foundMaterials.Add(x);
                        }
                        else if (x is Seed)
                        {
                            foundSeeds.Add(x);
                        }
                        else if (x is Plant)
                        {
                            foundPlants.Add(x);
                        }
                        else if (x is Trade)
                        {
                            foundTrade.Add(x);
                        }
                        else if (x is Treasure)
                        {
                            foundTreasures.Add(x);
                        }
                        else if (x is Upgrade)
                        {
                            foundUpgrades.Add(x);
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

                void GeneratePages(List<MotherObject> foundList, string pageName)
                {
                    if (foundList.Count == 0)
                        return; // Skip generating empty pages

                    var newPage = new ShopPage();
                    newPage.pageContent = new List<MotherObject>();
                    newPage.typeName = pageName;
                    shopPages.Add(newPage);

                    var pageNumber = 1;

                    foreach (MotherObject x in foundList)
                    {
                        var currentPage = shopPages[shopPages.Count - 1];

                        if (currentPage.pageContent.Count == 9)
                        {
                            pageNumber++;

                            var nextPage = new ShopPage();
                            nextPage.pageContent = new List<MotherObject>();
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

            //SPAWN A SPECIAL ITEMS
            if (shopObject.specialItemA != null)
            {
                priceMultiplier = 3f - (merchantile.dataValue * 0.2f);
                var objPrefab = Instantiate(shopItemPrefab);
                objPrefab.name = shopObject.specialItemA.printName;
                objPrefab.transform.SetParent(specialShelfA.transform, false);
                objPrefab.GetComponent<ShopItemPrefab>().priceMultiplier = priceMultiplier;
                objPrefab.GetComponent<ShopItemPrefab>().EnableObject(shopObject.specialItemA, this);
            }
            if (shopObject.specialItemB != null)
            {
                priceMultiplier = 3f - (merchantile.dataValue * 0.2f);
                var objPrefab = Instantiate(shopItemPrefab);
                objPrefab.name = shopObject.specialItemB.printName;
                objPrefab.transform.SetParent(specialShelfB.transform, false);
                objPrefab.GetComponent<ShopItemPrefab>().priceMultiplier = priceMultiplier;
                objPrefab.GetComponent<ShopItemPrefab>().EnableObject(shopObject.specialItemB, this);
            }
            if (shopObject.specialItemC != null)
            {
                priceMultiplier = 3f - (merchantile.dataValue * 0.2f);
                var objPrefab = Instantiate(shopItemPrefab);
                objPrefab.name = shopObject.specialItemC.printName;
                objPrefab.transform.SetParent(specialShelfC.transform, false);
                objPrefab.GetComponent<ShopItemPrefab>().priceMultiplier = priceMultiplier;
                objPrefab.GetComponent<ShopItemPrefab>().EnableObject(shopObject.specialItemC, this);
            }
            if (shopObject.specialItemD != null)
            {
                priceMultiplier = 3f - (merchantile.dataValue * 0.2f);
                var objPrefab = Instantiate(shopItemPrefab);
                objPrefab.name = shopObject.specialItemD.printName;
                objPrefab.transform.SetParent(specialShelfD.transform, false);
                objPrefab.GetComponent<ShopItemPrefab>().priceMultiplier = priceMultiplier;
                objPrefab.GetComponent<ShopItemPrefab>().EnableObject(shopObject.specialItemD, this);
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
        //Debug.Log($"{shopObject.farewellText}");
    }

    public void AttemptPurchase(MotherObject buyItem, int itemCost)
    {
        if (buyItem.dataValue < buyItem.maxValue)
        {
            if (dataManager.playerGold >= itemCost)
            {
                //Add confirm menu
                dataManager.playerGold -= itemCost;
                buyItem.dataValue++;
                Debug.Log($"{shopObject.sucessfulPurchaseText} You purchased {buyItem.printName} for {itemCost}. You now have {buyItem.dataValue} and your remaining gold is {dataManager.playerGold}");
            }
            else
                Debug.Log(shopObject.notEnoguhMoneyText);
        }
        else
            Debug.Log(shopObject.maxedValueText);
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
        topicManager.OpenTopicManager(shopObject.shopKeeper);
    }
}
