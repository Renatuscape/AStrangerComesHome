using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

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
    public GameObject shelf3; //special item
    public MenuFloatTextScript floatText;

    void Awake()
    {
        priceMultiplier = 2f;
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
    }

    private void OnEnable()
    {
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

            //READY TO SET UP SHOP AND SPAWN ITEMS
            shopObject.SetupShop();

            if (shopObject.shopInventory != null)
            {
                if (shopObject.sellsUpgrades)
                {
                    shelf.GetComponent<GridLayoutGroup>().cellSize = new Vector2(64, 32);
                }
                else
                {
                    shelf.GetComponent<GridLayoutGroup>().cellSize = new Vector2(32, 32);
                }

                var objectList = shopObject.shopInventory.OrderBy(obj => obj.rarity).ToList();

                foreach (MotherObject x in shopObject.shopInventory)
                {
                    var shelf = this.shelf;

                    var prefab = Instantiate(shopItemPrefab);
                    prefab.name = x.printName;
                    prefab.transform.SetParent(shelf.transform, false);
                    prefab.GetComponent<ShopItemPrefab>().EnableObject(x, this);
                }

                //Debug.Log($"{shopObject.welcomeText}");
            }

            //SPAWN A SPECIAL ITEM ON A SPECIAL DAY IF IT EXISTS ON THE SHOP OBJECT. Consolidate into one method?
            if (shopObject.rareItemA != null && shopObject.rareItemDayA == transientData.weekDay)
            {
                priceMultiplier = 3f - (merchantile.dataValue * 0.2f);
                var objPrefab = Instantiate(shopItemPrefab);
                objPrefab.name = shopObject.rareItemA.printName;
                objPrefab.transform.SetParent(shelf3.transform, false);
                objPrefab.GetComponent<ShopItemPrefab>().EnableObject(shopObject.rareItemA, this);
            }
            else if (shopObject.rareItemB != null && shopObject.rareItemDayB == transientData.weekDay)
            {
                priceMultiplier = 3f - (merchantile.dataValue * 0.2f);
                var objPrefab = Instantiate(shopItemPrefab);
                objPrefab.name = shopObject.rareItemB.printName;
                objPrefab.transform.SetParent(shelf3.transform, false);
                objPrefab.GetComponent<ShopItemPrefab>().EnableObject(shopObject.rareItemB, this);
            }
            else if (shopObject.rareItemC != null && shopObject.rareItemDayC == transientData.weekDay)
            {
                priceMultiplier = 3f - (merchantile.dataValue * 0.2f);
                var objPrefab = Instantiate(shopItemPrefab);
                objPrefab.name = shopObject.rareItemC.printName;
                objPrefab.transform.SetParent(shelf3.transform, false);
                objPrefab.GetComponent<ShopItemPrefab>().EnableObject(shopObject.rareItemC, this);
            }
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in shelf.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in shelf3.transform)
        {
            Destroy(child.gameObject);
        }

        floatText.DisableFloatText();
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
        floatText.PrintFloatText(text);
    }

    public void DisableFloatText()
    {
        floatText.DisableFloatText();
    }

    public void ChatButton()
    {
        topicManager.OpenTopicManager(shopObject.shopKeeper);
    }
}
