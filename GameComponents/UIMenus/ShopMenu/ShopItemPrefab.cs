using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemPrefab : MonoBehaviour, IPointerDownHandler, IInitializePotentialDragHandler, IDragHandler
{
    public ShopMenu shopMenu;

    public Item itemSource;
    public bool isReady = false;
    public bool sellFromPlayer = false;
    public bool clicked = false;


    public void Initialise(Item item, ShopMenu script, bool sellFromPlayer)
    {
        this.sellFromPlayer = sellFromPlayer;
        shopMenu = script;
        itemSource = item;
        isReady = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Required for dragging to work and translate properly to the spawned prefab
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

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (shopMenu.buyFromShopMenu.selectedItem == itemSource || shopMenu.sellFromPlayerMenu.selectedItem == itemSource)
        {
            Debug.Log("Spawning draggable " + itemSource.name);
            // Here we instantiate the second object, that we want to drag. 
            var item = itemSource;
            var prefab = BoxFactory.CreateItemIcon(item, false, 96);
            prefab.transform.SetParent(shopMenu.transform, false);
            prefab.name = item.name;

            var draggable = prefab.AddComponent<DraggableShopItem>();
            draggable.shopItem = this;

            if (prefab != null)
            {
                Debug.Log("Drag registered for " + itemSource.name);
                prefab.transform.position = Input.mousePosition;

                eventData.pointerDrag = prefab; // assign instantiated object
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked shop item " + itemSource.name);

        if (isReady)
        {
            if (sellFromPlayer)
            {
                shopMenu.HighlightPlayerItem(itemSource);
            }
            else
            {
                shopMenu.HighlightShopItem(itemSource);
            }
        }
    }
}
