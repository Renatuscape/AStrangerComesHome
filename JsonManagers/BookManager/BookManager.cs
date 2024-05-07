using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Ports;
public class BookManager : MonoBehaviour
{
    public List<Book> debugItemList = Books.all;
    public List<Item> bookItems;

    public bool allObjecctsLoaded = false;
    public int filesLoaded;
    public int numberOfFilesToLoad;
    public Task StartLoading()
    {
        List<Task> loadingTasks = new List<Task>();

        gameObject.SetActive(true);
        filesLoaded = 0;
        numberOfFilesToLoad = 0;

        if (Items.all == null || Items.all.Count == 0)
        {
            Debug.Log("No items loaded. Aborting book load.");
        }
        else
        {
            var info = new DirectoryInfo(Application.streamingAssetsPath + "/JsonData/Items/Books/");
            var fileInfo = info.GetFiles();

            foreach (var file in fileInfo)
            {
                if (file.Extension == ".json")
                {
                    numberOfFilesToLoad++;
                    Task loadingTask = LoadFromJsonAsync(Path.GetFileName(file.FullName)); // Pass only the file name
                    loadingTasks.Add(loadingTask);
                }
            }
        }

        return Task.WhenAll(loadingTasks);
    }

    [System.Serializable]
    public class BookWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Book[] books;
    }

    public async Task LoadFromJsonAsync(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Items/Books/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
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
        if (book.objectID.Contains("LET"))
        {
            LetterSetup(ref book);
        }
        else
        {
            IDReader(ref book);
        }

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
        // Debug.Log($"Looking for book item with ID {bookID}");

        book.inventoryItem = bookItems.FirstOrDefault(i => i.objectID.Contains(bookID));
        book.name = book.inventoryItem.name;
    }

    void LetterSetup(ref Book letter)
    {
        letter.isLetter = true;
        var letterID = letter.objectID.Split("-")[0];
        var letterSprite = SpriteFactory.GetItemSprite(letterID);

        if (letterSprite == null)
        {
            letterSprite = SpriteFactory.GetItemSprite("LET000");
        }

        letter.inventoryItem = new()
        {
            objectID = letterID,
            name = letter.name,
            sprite = letterSprite,
            notBuyable = true,
            notSellable = true,
            rarity = ItemRarity.Common,
            type = ItemType.Book,
            maxValue = 9,
        };

        Items.all.Add(letter.inventoryItem);
    }
}
