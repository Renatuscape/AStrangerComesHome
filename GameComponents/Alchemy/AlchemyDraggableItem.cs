using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AlchemyDraggableItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public AlchemyMenu alchemyMenu;
    public Transform parentContainer;
    public GameObject dragParent;

    public Item item;
    public Image[] images;
    public GameObject lastCollision;

    private void Awake()
    {
        images = transform.Find("ImageContainer").GetComponentsInChildren<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"Beginning drag {item}");
        parentContainer = transform.parent;
        transform.SetAsLastSibling();

        foreach (Image image in images)
        {
            image.raycastTarget = false;
        }
        //image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.SetParent(dragParent.transform);
        transform.position = Input.mousePosition; // MouseTracker.GetMouseWorldPosition();

        if (eventData.pointerEnter != null)
        {
            // Check if the pointer is over a container GameObject
            //Debug.Log($"Currently colliding with: {eventData.pointerEnter.gameObject}");
            lastCollision = eventData.pointerEnter.gameObject;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"Ending drag {item}");
        transform.SetParent(parentContainer);
        transform.SetAsLastSibling();

        foreach (Image image in images)
        {
            image.raycastTarget = true;
        }

        if (lastCollision != alchemyMenu.tableContainer
            && lastCollision != alchemyMenu.infusionContainer
            && lastCollision.name != "Image")
        {
            alchemyMenu.ReturnIngredientToInventory(gameObject);
        }
        else if (transform.parent.gameObject == dragParent)
        {
            alchemyMenu.ReturnIngredientToInventory(gameObject);
        }
    }
}
