using System;
using System.Collections.Generic;
using UnityEngine;

public class ReaderHorizontalLayout : MonoBehaviour
{
    public BookReader reader;

    public List<Page> pages;
    public int pageIndex;
    Book prevBook;

    internal void Initialise(Book book)
    {
        if (prevBook != book)
        {
            pageIndex = 0;
            prevBook = book;
        }
        pages = book.pages;

        gameObject.SetActive(true);
    }

    void Start()
    {
        gameObject.SetActive(false);
    }
}
