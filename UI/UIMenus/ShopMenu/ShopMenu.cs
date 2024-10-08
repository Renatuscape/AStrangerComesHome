using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopMenu : MonoBehaviour
{
    public Character shopkeeper;
    public Shop activeShop;
    public int judgement;
    public DataManagerScript dataManager;
    public PortraitRenderer portraitRenderer; //remember to use .gameObject for the object

    public GameObject clearanceNotice;
    public Image backgroundSprite;
    public ShopDragIcon dragIcon;

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

    public ShopItemPrefab selectedItem;

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
        ClearSelections();
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

            // SET BACKGROUND

            if (string.IsNullOrEmpty(shop.backgroundGraphic))
            {
                backgroundSprite.gameObject.SetActive(false);
            }
            else
            {
                var foundSprite = SpriteFactory.GetBackgroundSprite(shop.backgroundGraphic, "shop");

                if (foundSprite != null)
                {
                    backgroundSprite.sprite = foundSprite;
                    backgroundSprite.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("Shop sprite was not null, but could not be found. Check tag " + shop.backgroundGraphic);
                    backgroundSprite.gameObject.SetActive(false);
                }
            }

            // SET UP CONTAINERS WITH PREFABS
            buyFromShopPrefabs = buyFromShopMenu.Initialise(shop.GetSellFromShopList(), false, true, false);

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
                    var uiData = itemPrefab.GetComponent<ItemIconData>();
                    uiData.printRarity = true;
                    uiData.printSeedData = true;

                    var shopItemScript = itemPrefab.AddComponent<ShopItemPrefab>();
                    shopItemScript.Initialise(uiData.item, this, false);

                    uiData.EnablePrice(true, activeShop.CalculateBuyFromShopPrice(uiData.item));
                }

                BtnBuyFromShop();
                sellFromPlayerMenu.OpenPage(0);
            }
        }
    }

    public void SetUpSellFromPlayerMenu()
    {
        sellFromPlayerPrefabs = sellFromPlayerMenu.Initialise(activeShop.GetBuyFromPlayerList(true), true, true, false);

        foreach (var item in sellFromPlayerPrefabs)
        {
            var uiData = item.GetComponent<ItemIconData>();
            uiData.printRarity = true;
            uiData.printSeedData = true;
            var shopItemScript = item.AddComponent<ShopItemPrefab>();
            shopItemScript.Initialise(uiData.item, this, true);
        }
    }

    public void BtnSellFromPlayer()
    {
        selectedItem = null;
        dragIcon.DisableIcon();

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
        selectedItem = null;
        dragIcon.DisableIcon();

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
        itemNamePlate.text = "<b><size=32>" + Items.GetEmbellishedItemText(item, false, false, false, false) + "</size></b>\n" + item.rarity;
        itemPrice.text = $"Buy price: " + activeShop.CalculateBuyFromShopPrice(item).ToString();
        itemValue.text = $"Value: {item.basePrice}";
        itemNumberInInventory.text = $"({Player.GetCount(item.objectID, name)} in inventory)";

        itemInfoCard.SetActive(true);

    }
    public void HighlightPlayerItem(Item item)
    {
        itemNamePlate.text = "<b><size=32>" + Items.GetEmbellishedItemText(item, false, false, false, false) + "</size></b>\n" + item.rarity;
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

    public void SelectItemToDrag(ShopItemPrefab itemPrefab)
    {
        selectedItem = itemPrefab;
        dragIcon.DragThis(itemPrefab.itemSource.sprite);
    }

    public void HandleTransaction()
    {
        dragIcon.DisableIcon();

        Debug.Log("TRANSACTION DETECTED " + selectedItem.itemSource.name);
        var item = selectedItem.itemSource;
        var inventoryAmount = Player.GetCount(item.objectID, name);

        if (selectedItem.sellFromPlayer)
        {
            if (inventoryAmount > 0)
            {
                var itemCost = activeShop.CalculateSellFromInventoryPrice(item);

                if (MoneyExchange.AddHighestDenomination(itemCost, false, out var totalAdded))
                {
                    Player.Remove(item.objectID);
                    AudioManager.PlaySoundEffect("cloth3", +0.3f);
                    // LogAlert.QueueTextAlert($"Sold for {itemCost} shillings.\nI now have {Player.GetCount(item.objectID, name)} total.");
                    //TransientDataScript.PushAlert($"Sold {item.name} for {itemCost} shillings.\nI now have {Player.GetCount(item.objectID, name)} total.");

                    selectedItem.UpdateInventoryCount();
                }
                else
                {
                    LogAlert.QueueTextAlert("Not enough space in wallet.");
                    AudioManager.PlaySoundEffect("drumDoubleTap");
                }
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
                    AudioManager.PlaySoundEffect("cloth3", +0.3f);
                    //Debug.Log($"{activeShop} You purchased {item.name} for {itemCost}.");
                    //LogAlert.QueueTextAlert($"Paid { itemCost} shillings.\nI now have {Player.GetCount(item.objectID, name)} total.");
                    //TransientDataScript.PushAlert($"Purchased {item.name}. I now have {Player.GetCount(item.objectID, name)} total.");

                    selectedItem.UpdateInventoryCount();
                }
                else
                {
                    AudioManager.PlaySoundEffect("drumDoubleTap");
                    LogAlert.QueueTextAlert("I don't have enough money.");
                }

            }
            else
            {
                AudioManager.PlaySoundEffect("drumDoubleTap");
                // Debug.Log("I already have the maximum amount of this item.");
                LogAlert.QueueTextAlert("I don't have enough space for more of this.");
            }
        }

        ClearSelections();
    }

    public void ClearSelections()
    {
        itemInfoCard.SetActive(false);
        selectedItem = null;
        dragIcon.DisableIcon();
    }

    private void OnDisable()
    {
        buyFromShopMenu.ClearPrefabs();
        sellFromPlayerMenu.ClearPrefabs();
    }
}