using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CollectionsBooks : MonoBehaviour
{
    public BookReader reader;
    public GameObject bookContainer;
    public GameObject bookDetails;
    public TextMeshProUGUI detailTitle;
    public TextMeshProUGUI detailDescription;

    public List<GameObject> prefabs = new();
    public Item selectedBook;

    public void Initialise(bool forLetters)
    {
        reader.gameObject.SetActive(false);
        bookDetails.SetActive(true);
        gameObject.SetActive(true);

        detailTitle.text = "";
        detailDescription.text = "Double-click a book to see its content, if it has anything of interest.";

        if (prefabs.Count > 0)
        {
            foreach (var prefab in prefabs)
            {
                prefab.GetComponent<ItemIconData>().Return("CollectionsBook in Initialise");
            }
            prefabs.Clear();

            reader.gameObject.SetActive(false);
        }

        var bookList = Books.GetBookItems();

        if (bookList != null)
        {
            bookList = bookList.OrderBy(i => i.name).ToList();

            foreach (Item item in bookList)
            {
                if (item.type == ItemType.Book)
                {
                    if (Player.GetCount(item.objectID, name) > 0)
                    {
                        if (forLetters && item.objectID.Contains("LET")
                        || !forLetters && item.objectID.Contains("BOO"))
                        {
                            var newBook = BoxFactory.CreateItemIcon(item, true, 64, 18).gameObject;
                            prefabs.Add(newBook);
                            newBook.transform.SetParent(bookContainer.transform, false);

                            newBook.AddComponent<ShelvedBook>();
                            var script = newBook.GetComponent<ShelvedBook>();
                            script.Initialise(this, item);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("Could not print books as no items were found.");
        }
    }

    public void SelectBook(ShelvedBook shelvedBook)
    {
        detailTitle.text = shelvedBook.bookItem.name;
        detailDescription.text = shelvedBook.bookItem.description;
        bookDetails.SetActive(true);
    }

    private void OnDisable()
    {
        reader.gameObject.SetActive(false);
    }
}