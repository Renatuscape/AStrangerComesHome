using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float profitMargin;

    public GameObject clearanceNotice;
    public GameObject shopItemPrefab;

    public List<GameObject> buyFromShopPrefabs;
    public List<GameObject> sellFromPlayerPrefabs;

    public PageinatedContainer buyFromShopMenu;
    public PageinatedContainer sellFromPlayerMenu;

    public Button btnBuy;
    public Button btnSell;
    public Button btnLeave;

    public Toggle toggleMultiple;

    public GameObject itemInfoCard;
    public GameObject itemDescriptionCard;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI itemRarity;
    public TextMeshProUGUI itemDescription;

    void Awake()
    {
        profitMargin = 2f;
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();

        btnBuy.onClick.AddListener(() => BtnBuyFromShop());
        btnSell.onClick.AddListener(() => BtnSellFromPlayer());
        btnLeave.onClick.AddListener(() => BtnLeave());

        itemInfoCard.SetActive(false);
        itemDescriptionCard.SetActive(false);
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
            // Set clearance sign
            if (activeShop.saleDay == (int)TransientDataScript.GetWeekDay())
            {
                clearanceNotice.SetActive(true);
            }
            else
            {
                clearanceNotice.SetActive(false);
            }

            // CONTAINERS
            sellFromPlayerPrefabs = sellFromPlayerMenu.Initialise(shop.GetSellList(), true, true, true);
            buyFromShopPrefabs = buyFromShopMenu.Initialise(shop.GetBuyList(), false, true, true);

            if (shop.buysItems)
            {
                foreach (var item in sellFromPlayerPrefabs)
                {
                    var uiData = item.GetComponent<ItemUiData>();
                    uiData.printRarity = true;
                    var shopItemScript = item.AddComponent<ShopItemPrefab>();
                    shopItemScript.Initialise(uiData.item, this, true);
                }
            }

            if (shop.doesNotSell)
            {
                BtnSellFromPlayer();
                buyFromShopMenu.OpenPage(0);
            }
            else
            {
                foreach (var item in buyFromShopPrefabs)
                {
                    var uiData = item.GetComponent<ItemUiData>();
                    uiData.printRarity = true;

                    var shopItemScript = item.AddComponent<ShopItemPrefab>();
                    shopItemScript.Initialise(uiData.item, this, false);
                }

                BtnBuyFromShop();
                sellFromPlayerMenu.OpenPage(0);
            }
        }
    }

    public void BtnSellFromPlayer()
    {
        if (activeShop.buysItems)
        {
            sellFromPlayerMenu.gameObject.SetActive(true);
            buyFromShopMenu.gameObject.SetActive(false);

            btnSell.interactable = false;

            if (!activeShop.doesNotSell)
            {
                btnBuy.interactable = true;
            }

        }

        itemInfoCard.SetActive(false);
        itemDescriptionCard.SetActive(false);
    }

    public void BtnBuyFromShop()
    {
        if (!activeShop.doesNotSell)
        {
            sellFromPlayerMenu.gameObject.SetActive(false);
            buyFromShopMenu.gameObject.SetActive(true);

            btnBuy.interactable = false;

            if (activeShop.buysItems)
            {
                btnSell.interactable = true;
            }
        }

        itemInfoCard.SetActive(false);
        itemDescriptionCard.SetActive(false);
    }

    public void BtnLeave()
    {
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
        gameObject.SetActive(false);
    }

    public void HighlightShopItem(Item item)
    {
        itemName.text = item.name + $" ({Player.GetCount(item.objectID, name)} in inventory)";
        itemPrice.text = activeShop.CalculateBuyFromShopPrice(item).ToString();
        itemDescription.text = item.description;
        itemRarity.text = item.rarity.ToString();

        itemInfoCard.SetActive(true);
        // itemDescriptionCard.SetActive(true);

        //var inventoryAmount = Player.GetCount(item.objectID, name);

        //if (inventoryAmount < item.maxValue)
        //{
        //    var purchase = MoneyExchange.Purchase(itemCost);

        //    if (purchase)
        //    {
        //        Player.Add(item.objectID);
        //        AudioManager.PlayUISound("handleCoins");
        //        //Debug.Log($"{activeShop} You purchased {item.name} for {itemCost}.");
        //        TransientDataScript.PushAlert($"Purchased {item.name}. I now have {Player.GetCount(item.objectID, name)} total.");
        //    }
        //    else
        //    {
        //        Debug.Log("Exchange returned false.");
        //        TransientDataScript.PushAlert("I don't have enough money.");
        //    }

        //}
        //else
        //{
        //    Debug.Log("I already have the maximum amount of this item.");
        //    TransientDataScript.PushAlert("I don't have enough space for more of this.");
        //}

    }
    public void HighlightPlayerItem(Item item)
    {
        itemName.text = item.name;
        itemPrice.text = activeShop.CalculateSellFromInventoryPrice(item).ToString();
        itemDescription.text = item.description;
        itemRarity.text = item.rarity.ToString();

        itemInfoCard.SetActive(true);
        // itemDescriptionCard.SetActive(true);
    }
    void SyncSkills()
    {
        judgement = Player.GetCount("ATT002", "ShopMenu");
    }
}