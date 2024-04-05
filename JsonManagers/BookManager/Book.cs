using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Book
{
    public string objectID;
    public Item book;
    public string name;

    public bool horizontalLayout = false;
    public bool autoPages;
    public List<Page> pages;
}

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
        return all.FirstOrDefault(b => b.book.objectID == itemID);
    }
}