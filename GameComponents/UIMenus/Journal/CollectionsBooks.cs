using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectionsBooks : MonoBehaviour
{
    public GameObject reader;
    public GameObject bookContainer;
    public GameObject bookDetails;
    public TextMeshProUGUI detailTitle;
    public TextMeshProUGUI detailDescription;

    public List<GameObject> prefabs;
    public Item selectedBook;
    void Start()
    {
        reader.SetActive(false);
        bookDetails.SetActive(false);
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        foreach (Item item in Items.all)
        {
            if (item.type == ItemType.Book) // filter by inventory later
            {
                var newBook = BoxFactory.CreateItemIcon(item, true, 64, 18);
                prefabs.Add(newBook);
                newBook.transform.SetParent(bookContainer.transform, false);

                newBook.AddComponent<ShelvedBook>();
                var script = newBook.GetComponent<ShelvedBook>();
                script.bookItem = item;
                script.bookContent = Books.FindByItemID(item.objectID);
                script.collectionsPage = this;
            }
        }
    }

    public void SelectBook(ShelvedBook shelvedBook)
    {
        detailTitle.text = shelvedBook.bookItem.name;
        detailDescription.text = shelvedBook.bookItem.description;
    }

    private void OnDisable()
    {
        foreach (var prefab in prefabs)
        {
            Destroy(prefab);
        }
        prefabs.Clear();

        reader.SetActive(false);
    }
}

public class ShelvedBook : MonoBehaviour
{
    public CollectionsBooks collectionsPage;
    public Item bookItem;
    public Book bookContent;

    public void SelectBook()
    {
        collectionsPage.SelectBook(this);
    }
}