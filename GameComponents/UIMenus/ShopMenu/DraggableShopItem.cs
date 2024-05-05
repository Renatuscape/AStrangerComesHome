using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableShopItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public ShopItemPrefab shopItem;
    public Transform parentContainer;
    public GameObject lastCollision;
    public Image[] images;
    public bool isDragging;
    public float shadowTransparency;

    private void Awake()
    {
        images = transform.Find("ImageContainer").GetComponentsInChildren<Image>();

        foreach (var image in images)
        {
            if (image.gameObject.name == "Shadow")
            {
                shadowTransparency = image.color.a;
            }

            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }

        Invoke("CheckDrag", 0.1f);
    }

    void CheckDrag()
    {
        if (!isDragging)
        {
            Destroy(gameObject);
        }

        foreach (var image in images)
        {
            if (image.gameObject.name == "Shadow")
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, shadowTransparency);
            }
            else
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log($"Beginning drag {shopItem}");
        parentContainer = transform.parent;
        transform.SetAsLastSibling();

        foreach (Image image in images)
        {
            image.raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        // Debug.Log($"Dragging {shopItem}");

        transform.position = Input.mousePosition; // MouseTracker.GetMouseWorldPosition();

        if (eventData.pointerEnter != null)
        {
            // Check if the pointer is over a container GameObject
            //Debug.Log($"Currently colliding with: {eventData.pointerEnter.gameObject}");
            lastCollision = eventData.pointerEnter.gameObject;
        }
        else
        {
            lastCollision = null;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log($"Ending drag {shopItem.itemSource}");

        foreach (Image image in images)
        {
            image.raycastTarget = true;
        }


        if (lastCollision != null && lastCollision.name.ToLower().Contains("register"))
        {
            shopItem.shopMenu.HandleTransaction(shopItem);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}