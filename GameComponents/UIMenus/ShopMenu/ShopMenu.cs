using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopMenu : MonoBehaviour
{
    public Character shopkeeper;
    public Shop activeShop;
    public int judgement;
    public DataManagerScript dataManager;
    public PortraitRenderer portraitRenderer; //remember to use .gameObject for the object

    public GameObject clearanceNotice;
    public GameObject priceTagPrefab;

    public List<GameObject> buyFromShopPrefabs;
    public List<GameObject> sellFromPlayerPrefabs;

    public PageinatedContainer buyFromShopMenu;
    public PageinatedContainer sellFromPlayerMenu;

    public Button btnBuy;
    public Button btnSell;
    public Button btnLeave;

    public Toggle toggleMultiple;

    public GameObject itemInfoCard;
    public TextMeshProUGUI itemNamePlate;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI itemValue;
    public TextMeshProUGUI itemNumberInInventory;

    void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();

        btnBuy.onClick.AddListener(() => BtnBuyFromShop());
        btnSell.onClick.AddListener(() => BtnSellFromPlayer());
        btnLeave.onClick.AddListener(() => BtnLeave());

        itemInfoCard.SetActive(false);
        if (TransientDataScript.GameState != GameState.ShopMenu)
        {
            gameObject.SetActive(false);
        }
    }
    public void SetUpShop(Character shopkeeper, Shop shop)
    {
        SyncSkills();
        TransientDataScript.SetGameState(GameState.ShopMenu, name, gameObject);
        transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);

        this.shopkeeper = shopkeeper;
        activeShop = shop;

        portraitRenderer.EnableForShop(shopkeeper.objectID);

        if (activeShop != null)
        {
            // SET CLEARANCE SIGN
            if (activeShop.saleDay == (int)TransientDataScript.GetWeekDay())
            {
                clearanceNotice.SetActive(true);
            }
            else
            {
                clearanceNotice.SetActive(false);
            }

            // SET UP CONTAINERS WITH PREFABS
            buyFromShopPrefabs = buyFromShopMenu.Initialise(shop.GetSellFromShopList(), false, true, true);

            if (!shop.buysItems)
            {
                btnSell.interactable = false;
            }
            else
            {
                SetUpSellFromPlayerMenu();
            }

            if (shop.doesNotSell)
            {
                BtnSellFromPlayer();
                buyFromShopMenu.OpenPage(0);
                btnBuy.interactable = false;
            }
            else
            {
                foreach (GameObject itemPrefab in buyFromShopPrefabs)
                {
                    var uiData = itemPrefab.GetComponent<ItemUiData>();
                    uiData.printRarity = true;

                    var shopItemScript = itemPrefab.AddComponent<ShopItemPrefab>();
                    shopItemScript.Initialise(uiData.item, this, false);

                    GameObject priceTag = Instantiate(priceTagPrefab, itemPrefab.transform);
                    priceTag.name = "PriceTag";

                    var tagText = priceTag.GetComponentInChildren<TextMeshProUGUI>();
                    tagText.text = activeShop.CalculateBuyFromShopPrice(uiData.item).ToString();
                }

                BtnBuyFromShop();
                sellFromPlayerMenu.OpenPage(0);
            }
        }
    }

    public void SetUpSellFromPlayerMenu()
    {
        sellFromPlayerPrefabs = sellFromPlayerMenu.Initialise(activeShop.GetBuyFromPlayerList(true), true, true, true);

        foreach (var item in sellFromPlayerPrefabs)
        {
            var uiData = item.GetComponent<ItemUiData>();
            uiData.printRarity = true;
            var shopItemScript = item.AddComponent<ShopItemPrefab>();
            shopItemScript.Initialise(uiData.item, this, true);
        }
    }

    public void BtnSellFromPlayer()
    {
        if (activeShop.buysItems)
        {
            sellFromPlayerMenu.gameObject.SetActive(true);
            buyFromShopMenu.gameObject.SetActive(false);
            buyFromShopMenu.ClearSeletion();

            btnSell.interactable = false;

            if (!activeShop.doesNotSell)
            {
                btnBuy.interactable = true;
            }

        }

        itemInfoCard.SetActive(false);
    }

    public void BtnBuyFromShop()
    {
        if (!activeShop.doesNotSell)
        {
            sellFromPlayerMenu.gameObject.SetActive(false);
            buyFromShopMenu.gameObject.SetActive(true);
            sellFromPlayerMenu.ClearSeletion();

            btnBuy.interactable = false;

            if (activeShop.buysItems)
            {
                btnSell.interactable = true;
            }
        }

        itemInfoCard.SetActive(false);
    }

    public void BtnLeave()
    {
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
        gameObject.SetActive(false);
    }

    public void HighlightShopItem(Item item)
    {
        itemNamePlate.text = "<b><size=32>" + Items.GetEmbellishedItemText(item, false, false, false) + "</size></b>\n" + item.rarity;
        itemPrice.text = $"Buy price: " + activeShop.CalculateBuyFromShopPrice(item).ToString();
        itemValue.text = $"Value: {item.basePrice}";
        itemNumberInInventory.text = $"({Player.GetCount(item.objectID, name)} in inventory)";

        itemInfoCard.SetActive(true);

    }
    public void HighlightPlayerItem(Item item)
    {
        itemNamePlate.text = "<b><size=32>" + Items.GetEmbellishedItemText(item, false, false, false) + "</size></b>\n" + item.rarity;
        itemPrice.text = "Sell price: " + activeShop.CalculateSellFromInventoryPrice(item).ToString();
        itemValue.text = $"Value: {item.basePrice}";
        itemNumberInInventory.text = "";

        itemInfoCard.SetActive(true);
        // itemDescriptionCard.SetActive(true);
    }
    void SyncSkills()
    {
        judgement = Player.GetCount("ATT002", "ShopMenu");
    }

    public void HandleTransaction(ShopItemPrefab shopItem)
    {
        Debug.Log("TRANSACTION DETECTED " + shopItem.itemSource.name);
        var item = shopItem.itemSource;
        var inventoryAmount = Player.GetCount(item.objectID, name);

        if (shopItem.sellFromPlayer)
        {
            if (inventoryAmount > 0)
            {
                var itemCost = activeShop.CalculateSellFromInventoryPrice(item);

                Player.Remove(item.objectID);
                AudioManager.PlayUISound("handleCoins");
                MoneyExchange.AddHighestDenomination(itemCost);
                LogAlert.QueueTextAlert("Sold for " + itemCost + " shillings.");
                //TransientDataScript.PushAlert($"Sold {item.name} for {itemCost} shillings.\nI now have {Player.GetCount(item.objectID, name)} total.");

                shopItem.UpdateInventoryCount();
            }
            else
            {
                Debug.LogWarning("Item count was 0. Ensure that item count is properly updated.");
            }
        }
        else
        {
            var itemCost = activeShop.CalculateBuyFromShopPrice(item);

            if (inventoryAmount < item.maxValue)
            {
                var purchase = MoneyExchange.Purchase(itemCost);

                if (purchase)
                {
                    Player.Add(item.objectID);
                    AudioManager.PlayUISound("handleCoins");
                    //Debug.Log($"{activeShop} You purchased {item.name} for {itemCost}.");
                    LogAlert.QueueTextAlert("Paid " + itemCost + " shillings.");
                    //TransientDataScript.PushAlert($"Purchased {item.name}. I now have {Player.GetCount(item.objectID, name)} total.");

                    shopItem.UpdateInventoryCount();
                }
                else
                {
                    AudioManager.PlayUISound("drumDoubleTap");
                    // Debug.Log("Exchange returned false.");
                    LogAlert.QueueTextAlert("I don't have enough money.");
                }

            }
            else
            {
                AudioManager.PlayUISound("drumDoubleTap");
                // Debug.Log("I already have the maximum amount of this item.");
                LogAlert.QueueTextAlert("I don't have enough space for more of this.");
            }
        }
    }
}