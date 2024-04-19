using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShelvedBook : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CollectionsBooks collectionsPage;
    public Item bookItem;
    public Book bookContent;
    public bool isSelected;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected)
        {
            if (bookContent != null)
            {
                collectionsPage.reader.Initialise(bookContent);
            }
            else
            {
                TransientDataScript.PushAlert("Nothing interesting in here.");
            }
        }
        else
        {
            collectionsPage.SelectBook(this);
            isSelected = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TransientDataScript.PrintFloatText($"{bookItem.name}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TransientDataScript.DisableFloatText();
        isSelected = false;
    }

    internal void Initialise(CollectionsBooks collectionsBooks, Item item)
    {
        bookItem = item;
        collectionsPage = collectionsBooks;
        bookContent = Books.FindByItemID(item.objectID);

        if (bookContent == null)
        {
            Image[] images = transform.Find("ImageContainer").GetComponentsInChildren<Image>();

            foreach (Image image in images)
            {
                if (image.gameObject.name != "Shadow")
                {
                    image.color = new Color(0.7f, 0.7f, 0.7f);
                }
            }
        }
    }
}