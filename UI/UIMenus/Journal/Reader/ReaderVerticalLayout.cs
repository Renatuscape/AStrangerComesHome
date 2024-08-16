using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReaderVerticalLayout : MonoBehaviour
{
    public BookReader reader;
    public GameObject pageA;
    public GameObject pageB;

    public TextMeshProUGUI pageNumberL;
    public TextMeshProUGUI pageNumberR;

    List<Page> pages;
    int pageIndex;

    Book prevBook;

    internal void Initialise(Book book)
    {
        Debug.Log("Vertical reader has been initialised. Attempting to set active.");

        gameObject.SetActive(true);

        if (prevBook != book)
        {
            pageIndex = 0;
            prevBook = book;
        }

        if (pages != null && pages.Count > 0)
        {
            pages = book.pages;
            PrintPage();
        }
    }

    void PrintPage()
    {
        reader.CleanReader();

        if (pages.Count > 2)
        {
            pageNumberL.text = $"{pageIndex + 1}";
            pageNumberR.text = $"{pageIndex + 2}";
            pageNumberL.gameObject.SetActive(true);
            pageNumberR.gameObject.SetActive(true);
        }
        else
        {
            pageNumberL.gameObject.SetActive(false);
            pageNumberR.gameObject.SetActive(false);
        }

        foreach (var text in pages[pageIndex].text)
            {
                var prefab = reader.PrintLine(text, pageA);
            }

            if (pages.Count > pageIndex + 1)
            {
                if (pages[pageIndex + 1].text != null && pages[pageIndex + 1].text.Count > 0)
                {
                    foreach (var text in pages[pageIndex + 1].text)
                    {
                        var prefab = reader.PrintLine(text, pageB);
                    }
                }
            }
    }

    public void BtnForward()
    {
        pageIndex += 2;

        if (pageIndex >= pages.Count)
        {
            pageIndex = 0;
        }

        PrintPage();
    }

    public void BtnBack()
    {
        pageIndex -= 2;

        if (pageIndex < 0)
        {
            pageIndex = 0;
        }

        PrintPage();
    }
}
