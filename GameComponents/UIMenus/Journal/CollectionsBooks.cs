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
    void Start()
    {
        reader.gameObject.SetActive(false);
        bookDetails.SetActive(false);
    }

    private void OnEnable()
    {
        if (prefabs.Count > 0)
        {
            foreach (var prefab in prefabs)
            {
                Destroy(prefab);
            }
            prefabs.Clear();

            reader.gameObject.SetActive(false);
            bookDetails.SetActive(false);
        }

        var bookList = Books.GetBookItems();

        if (bookList != null)
        {
            bookList = bookList.OrderBy(i => i.name).ToList();

            foreach (Item item in bookList)
            {
                if (item.type == ItemType.Book) // filter by inventory later
                {
                    var newBook = BoxFactory.CreateItemIcon(item, true, 64, 18);
                    prefabs.Add(newBook);
                    newBook.transform.SetParent(bookContainer.transform, false);

                    newBook.AddComponent<ShelvedBook>();
                    var script = newBook.GetComponent<ShelvedBook>();
                    script.Initialise(this, item);
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
}