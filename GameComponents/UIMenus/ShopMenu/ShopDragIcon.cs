using UnityEngine;
using UnityEngine.UI;

public class ShopDragIcon: MonoBehaviour
{
    public Image draggableImage;

    public void DragThis(Sprite sprite)
    {
        draggableImage.sprite = sprite;
        transform.position = Input.mousePosition;
        gameObject.SetActive(true);
    }

    public void DisableIcon()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }
}
