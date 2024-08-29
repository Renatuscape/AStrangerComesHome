using UnityEngine;
using UnityEngine.EventSystems;

public class GardenSeedFloat : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemIconData itemData;
    public void OnPointerEnter(PointerEventData eventData)
    {
        TransientDataScript.PrintFloatText(Items.GetEmbellishedItemText(itemData.item, false, true, false) + $"\nGrow time: {100 * (itemData.item.health + itemData.item.yield)}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TransientDataScript.DisableFloatText();
    }
}