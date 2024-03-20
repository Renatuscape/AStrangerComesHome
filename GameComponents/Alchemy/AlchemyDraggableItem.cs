using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AlchemyDraggableItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public AlchemyObject alchemyObject;
    public Transform parentContainer;
    public GameObject dragParent;

    public Item item;
    public Image[] images;
    public GameObject lastCollision;
    //public bool isInfusion;

    private void Awake()
    {
        images = transform.Find("ImageContainer").GetComponentsInChildren<Image>();

        foreach (var image in images)
        {
            if (image.gameObject.name == "Shadow")
            {
                // Get the RectTransform component of the image
                RectTransform rectTransform = image.GetComponent<RectTransform>();

                // Set the left property to 0
                rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);

                // Set the top property to 40
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -40);
            }
        }
    }

    private void Start()
    {
        lastCollision = alchemyObject.materialContainer;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log($"Beginning drag {item}");
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
        //Debug.Log($"Ending drag {item}");
        transform.SetParent(parentContainer);
        transform.SetAsLastSibling();

        foreach (Image image in images)
        {
            image.raycastTarget = true;
        }

        if (lastCollision != alchemyObject.materialContainer
            && lastCollision != alchemyObject.infusionContainer
            && lastCollision.name != "Image")
        {
            alchemyObject.ReturnToInventory(this);
        }
        else if (transform.parent.gameObject == dragParent)
        {
            alchemyObject.ReturnToInventory(this);
        }
        else if (lastCollision == alchemyObject.infusionContainer)
        {
            alchemyObject.SetAsInfusion();
            //alchemyObject.alchemyMenu.selectedIngredients.UpdateItemContainer(this, gameObject);
        }
        else if (lastCollision == alchemyObject.materialContainer)
        {
            alchemyObject.SetAsMaterial();
            //alchemyObject.alchemyMenu.selectedIngredients.UpdateItemContainer(this, gameObject);
        }
        else if (lastCollision.name == "Image")
        {
            var draggableItemScript = lastCollision.transform.parent.transform.parent.GetComponent<AlchemyDraggableItem>();

            if (draggableItemScript != null)
            {
                if (alchemyObject.isInfusion)
                {
                    alchemyObject.SetAsInfusion();
                }
                else
                {
                    alchemyObject.SetAsMaterial();
                }
                //transform.SetParent(lastCollision.transform.parent.transform.parent.transform.parent, false);
            }
            else
            {
                var inventoryItemScript = lastCollision.transform.parent.transform.parent.GetComponent<AlchemyInventoryItem>();

                if (inventoryItemScript != null)
                {
                    alchemyObject.ReturnToInventory(this);
                }
                else
                {
                    Debug.Log("Last collision was named Image, but could not find AlchemyDraggableItem or AlchemyInventoryItem script. Defaulting to table.");
                    alchemyObject.SetAsMaterial();
                    //transform.SetParent(alchemyObject.materialContainer.transform, false);
                }
            }
            //alchemyObject.alchemyMenu.selectedIngredients.UpdateItemContainer(this, gameObject);
        }
    }
}
