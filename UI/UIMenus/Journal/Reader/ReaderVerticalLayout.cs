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

    public Book book;
    public List<Page> pages;
    int pageIndex;

    internal void Initialise(Book bookIn)
    {
        Debug.Log("Vertical reader has been initialised. Attempting to set active.");
        book = bookIn;
        pages = book.pages;
        pageIndex = 0;
        gameObject.SetActive(true);

        if (pages != null && pages.Count > 0)
        {
            PrintPage();
        }
        else
        {
            Debug.Log("Vertical layout found no pages.");
        }
    }

    void PrintPage()
    {
        reader.CleanReader();

        if (pages.Count > 2)
        {
            pageNumberL.text = $"{pageIndex + 1}";
            pageNumberR.text = $"{pageIndex + 2}";
            pageNumberL.transform.parent.gameObject.SetActive(true);
            pageNumberR.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            pageNumberL.transform.parent.gameObject.SetActive(false);
            pageNumberR.transform.parent.gameObject.SetActive(false);
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
