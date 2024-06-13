using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AlchemyIngredient : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public AlchemyInventoryItem inventoryItem;
    public Image[] images;

    public bool isPickedUp;

    public void Initialise(AlchemyInventoryItem inventoryItem)
    {
        isPickedUp = true;
        this.inventoryItem = inventoryItem;

        images = transform.Find("ImageContainer").GetComponentsInChildren<Image>();

        foreach (var image in images)
        {
            image.raycastTarget = false;

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
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isPickedUp)
        {
            isPickedUp = true;

            foreach (Image image in images)
            {
                image.raycastTarget = false;
            }

            TransientDataScript.DisableFloatText();
        }
    }

    void Drop()
    {
        isPickedUp = false;

        if (AlchemyMenu.lastCollision != null)
        {
            Debug.Log("Last collision was " + AlchemyMenu.lastCollision.name);
        }

        if (AlchemyMenu.lastCollision == inventoryItem.infusionContainer)
        {

            MoveToNewParent(inventoryItem.infusionContainer);
        }
        else if (AlchemyMenu.lastCollision == inventoryItem.materialContainer)
        {
            MoveToNewParent(inventoryItem.materialContainer);
        }
        else
        {
            ReturnToInventory(false);
        }
    }

    public void ReturnToInventory(bool isInstant)
    {
        if (isInstant)
        {
            inventoryItem.ReturnItem(this);
        }
        else
        {
            StartCoroutine(ClearFromTable(0.3f));
        }
    }

    IEnumerator ClearFromTable(float duration)
    {
        transform.SetParent(inventoryItem.dragParent.transform);
        Vector3 targetLocation = inventoryItem.gameObject.transform.position;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            transform.position = Vector3.Lerp(startPosition, targetLocation, t);
            yield return null;
        }

        // Ensure the object is exactly at the target position
        transform.position = targetLocation;
        inventoryItem.ReturnItem(this);
    }

    void MoveToNewParent(GameObject parent)
    {
        foreach (Image image in images)
        {
            image.raycastTarget = true;
        }

        if (transform.parent != parent)
        {
            inventoryItem.SetAllPrefabParents(parent);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPickedUp)
        {
            TransientDataScript.PrintFloatText($"{inventoryItem.itemIconData.item.name}");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPickedUp)
        {
            TransientDataScript.DisableFloatText();
        }
    }

    private void Update()
    {
        if (isPickedUp)
        {
            gameObject.transform.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                Drop();
            }
        }
    }
}