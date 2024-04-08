using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Book
{
    public string objectID;
    public Item inventoryItem;
    public string name;

    public bool horizontalLayout = false;
    public bool scrolling = false;
    public bool autoPages;
    public List<Page> pages;
}

[System.Serializable]
public class Page
{
    public int columns = 1;
    public int rows = 0;
    public bool columnDividers = false;
    public bool rowDividers = false;
    public List<string> text = new();
}

public static class Books
{
    public static List<Book> all = new();

    public static Book FindByItemID(string itemID)
    {
        return all.FirstOrDefault(b => b.inventoryItem != null && b.inventoryItem.objectID == itemID);
    }

    public static Book FindByID(string objectID)
    {
        return all.FirstOrDefault(b => b.objectID == objectID);
    }

    public static List<Item> GetBookItems()
    {
        if (Items.all != null && Items.all.Count == 0)
        {
            Debug.Log("Game items have yet to be loaded.");
            return null;
        }
        else
        {
            return Items.all.Where(i => i.type == ItemType.Book).ToList();
        }
    }
}