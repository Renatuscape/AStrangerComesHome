using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;
public class BookManager : MonoBehaviour
{
    public List<Book> debugItemList = Books.all;
    public bool allObjecctsLoaded = false;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad;
    public List<Item> bookItems;
    void Start()
    {
        if (Items.all == null || Items.all.Count == 0)
        {
            Debug.Log("No items loaded. Aborting book load.");
        }
        else
        {
            var info = new DirectoryInfo(Application.streamingAssetsPath + "/JsonData/Items/Books/");
            var fileInfo = info.GetFiles();
            numberOfFilesToLoad = fileInfo.Count();

            foreach (var file in fileInfo)
            {
                if (file.Extension != ".json")
                {
                    numberOfFilesToLoad -= 1;
                }
                else
                {
                    LoadFromJson(Path.GetFileName(file.FullName)); // Pass only the file name
                }
            }
        }
    }

    [System.Serializable]
    public class BookWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Book[] books;
    }

    public void LoadFromJson(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Items/Books/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = File.ReadAllText(jsonPath);
            BookWrapper dataWrapper = JsonUtility.FromJson<BookWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.books != null)
                {
                    foreach (Book book in dataWrapper.books)
                    {
                        InitialiseBook(book, Books.all);
                    }
                    filesLoaded++;
                    if (filesLoaded == numberOfFilesToLoad)
                    {
                        allObjecctsLoaded = true;
                        Debug.Log("All BOOKS successfully loaded from Json.");
                    }
                }
                else
                {
                    Debug.LogError("Books array is null in JSON data. Check that the list has a wrapper with the \'books\' tag and that the object class is serializable.");
                }
            }
            else
            {
                Debug.LogError("JSON data is malformed. No wrapper found?");
                Debug.Log(jsonData); // Log the JSON data for inspection
            }
        }
        else
        {
            Debug.LogError("JSON file not found: " + jsonPath);
        }
    }

    void InitialiseBook(Book book, List<Book> bookList)
    {
        IDReader(ref book);

        bookList.Add(book);
    }

    void IDReader(ref Book book)
    {
        if (bookItems == null || bookItems.Count == 0)
        {
            bookItems = Books.GetBookItems();
            Debug.Log("Set book items list for BookManager ID reader.");
        }

        string bookID = book.objectID.Split("-")[0];
        Debug.Log($"Looking for book item with ID {bookID}");

        book.inventoryItem = bookItems.FirstOrDefault(i => i.objectID.Contains(bookID));
        if (book.inventoryItem != null)
        {
            Debug.Log($"Found {book.inventoryItem.name}");
        }
        else
        {
            Debug.Log($"{bookID} yielded no results.");
        }
    }
}
