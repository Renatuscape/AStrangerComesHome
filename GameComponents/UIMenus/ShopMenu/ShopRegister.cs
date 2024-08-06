using UnityEngine;
using UnityEngine.EventSystems;

public class ShopRegister : MonoBehaviour, IPointerDownHandler
{
    public ShopMenu shopMenu;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (shopMenu.selectedItem != null)
        {
            shopMenu.HandleTransaction();
        }
    }
}
