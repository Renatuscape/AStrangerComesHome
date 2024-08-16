using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemPrefab : MonoBehaviour, IPointerDownHandler
{
    public ShopMenu shopMenu;

    public Item itemSource;
    public bool isReady = false;
    public bool sellFromPlayer = false;

    public void Initialise(Item item, ShopMenu script, bool sellFromPlayer)
    {
        this.sellFromPlayer = sellFromPlayer;
        shopMenu = script;
        itemSource = item;
        isReady = true;
    }

    public void UpdateInventoryCount()
    {
        var inventoryCount = Player.GetCount(itemSource.objectID, "shop prefab " + itemSource.name);
        // Debug.Log("Updating inventory. Count was" + inventoryCount);

        if (sellFromPlayer)
        {
            if (inventoryCount > 0)
            {
                foreach (Transform child in gameObject.transform)
                {
                    var textMesh = child.GetComponentInChildren<TextMeshProUGUI>();
                    if (textMesh != null)
                    {
                        textMesh.text = inventoryCount.ToString();
                        break;
                    }
                }
            }
            else
            {
                shopMenu.SetUpSellFromPlayerMenu();
            }
        }
        else
        {
            // Debug.Log("Found item to buy. Inventory count was " + inventoryCount);
            if (inventoryCount == 1)
            {
                // Debug.Log("ATTEMPTING TO REFRESH SELL FROM PLAYER MENU");
                shopMenu.SetUpSellFromPlayerMenu();
            }
            else
            {
                foreach (var prefab in shopMenu.sellFromPlayerMenu.prefabMasterList)
                {
                    var script = prefab.GetComponent<ShopItemPrefab>();

                    if (script != null && script.itemSource == itemSource)
                    {
                        script.UpdateInventoryCount();
                        break;
                    }
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (shopMenu.selectedItem == null)
        {
            if (sellFromPlayer)
            {
                shopMenu.HighlightPlayerItem(itemSource);
            }
            else
            {
                shopMenu.HighlightShopItem(itemSource);
            }

            shopMenu.SelectItemToDrag(this);
        }
        else
        {
            shopMenu.ClearSelections();
        }
    }
}
