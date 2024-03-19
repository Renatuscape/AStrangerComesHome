using UnityEngine;
using UnityEngine.EventSystems;

public class AlchemyInventoryItem : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public AlchemyMenu alchemyMenu;
    public Item item;
    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (alchemyMenu.synthData != null && !alchemyMenu.synthData.isSynthActive)
        {
            // Here we instantiate the second object, that we want to drag. 
            GameObject go = alchemyMenu.DragItemFromInventory(item);

            if (go != null)
            {
                go.transform.position = Input.mousePosition;

                eventData.pointerDrag = go; // assign instantiated object
            }
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        // Required for dragging to work and translate properly to the spawned prefab
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TransientDataCalls.PrintFloatText($"{item.name}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TransientDataCalls.DisableFloatText();
    }
}
