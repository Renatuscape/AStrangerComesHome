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

    public List<Item> shopInventory;
    public List<GameObject> spawnedPrefabs;

    void Awake()
    {
        profitMargin = 2f;
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
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
        SyncSkills();
        TransientDataScript.SetGameState(GameState.ShopMenu, name, gameObject);
        transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);

        this.shopkeeper = shopkeeper;
        activeShop = shop;

        EnablePortraits();

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
}


[System.Serializable]
public struct ContainerPage
{
    public string typeName;
    public List<Item> pageContent;
}