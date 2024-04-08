using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReaderAutoPageLayout : MonoBehaviour
{
    public BookReader reader;
    public Page activePage;
    public AutoPageinator autoPageinator;

    public GameObject pageA;
    public GameObject pageB;

    public TextMeshProUGUI pageNumberL;
    public TextMeshProUGUI pageNumberR;

    int pageIndex;
    Book prevBook;

    internal void InitialiseAutoBook(Book book)
    {
        if (book.pages != null && book.pages.Count != 1)
        {
            Debug.Log("An autopage book should have no more or less than 1 page.");
        }
        else
        {
            if (prevBook != book)
            {
                pageIndex = 0;
                prevBook = book;
            }

            gameObject.SetActive(true);

            activePage = book.pages[0];
            BuildAutoBook();
            PrintAutoPage();
        }
    }

    internal void PrintAutoPage()
    {
        reader.CleanReader();
        pageNumberL.text = $"{pageIndex + 1}";
        pageNumberR.text = $"{pageIndex + 2}";

        if (autoPageinator.pages != null && autoPageinator.pages.Count > 0)
        {
            foreach (var text in autoPageinator.pages[pageIndex].text)
            {
                var prefab = reader.PrintLine(text, pageA);
            }

            if (autoPageinator.pages.Count > pageIndex + 1)
            {
                if (autoPageinator.pages[pageIndex + 1].text != null && autoPageinator.pages[pageIndex + 1].text.Count > 0)
                {
                    foreach (var text in autoPageinator.pages[pageIndex + 1].text)
                    {
                        var prefab = reader.PrintLine(text, pageB);
                    }
                }
            }
        }
    }

    void BuildAutoBook()
    {
        autoPageinator = new();
        autoPageinator.pages = new() { new() };
        autoPageinator.pages[0].text = new();
        var autoPageIndex = 0;

        for (int i = 0; i < activePage.text.Count; i++)
        {
            Canvas.ForceUpdateCanvases();

            reader.PrintLine(activePage.text[i], pageA);

            if (!CheckOverflow(pageA.GetComponent<RectTransform>()) ||
                (i != 0 && activePage.text[i].Contains("#B ")))
            {
                autoPageinator.pages.Add(new());
                autoPageIndex++;
                autoPageinator.pages[autoPageIndex].text = new();
                reader.CleanReader();
            }

            autoPageinator.pages[autoPageIndex].text.Add(activePage.text[i]);

            Canvas.ForceUpdateCanvases();
        }
    }

    public void BtnForward()
    {
        pageIndex += 2;

        if (pageIndex >= autoPageinator.pages.Count)
        {
            pageIndex = 0;
        }
        PrintAutoPage();
    }

    public void BtnBack()
    {
        pageIndex -= 2;

        if (pageIndex < 0)
        {
            pageIndex = 0;
        }

        PrintAutoPage();
    }

    bool CheckOverflow(RectTransform parentRect)
    {
        float parentHeight = parentRect.rect.height - 30;
        float childrenHeight = 0f;

        // Ensure layout update before calculating preferred height
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

        // Calculate the combined height of all child objects
        foreach (Transform child in parentRect)
        {
            RectTransform childRect = child.GetComponent<RectTransform>();
            if (child.gameObject.activeSelf && childRect != null)
            {
                // If the child has a TextMeshProUGUI component, calculate its height based on content
                TMP_Text textComponent = child.GetComponentInChildren<TMP_Text>();
                if (textComponent != null)
                {
                    // Ensure layout update before calculating preferred height
                    LayoutRebuilder.ForceRebuildLayoutImmediate(textComponent.rectTransform);
                    float textHeight = LayoutUtility.GetPreferredHeight(textComponent.rectTransform);
                    childrenHeight += textHeight;
                }
                else
                {
                    // For regular RectTransforms, just add their height
                    childrenHeight += childRect.rect.height;
                }
            }
        }

        Debug.Log($"Parent height was {parentHeight}. Children height was {childrenHeight}. Should return {childrenHeight > parentHeight}");

        if (childrenHeight > parentHeight)
        {
            // Overflow detected
            Debug.Log("Overflow detected!");
            return false;
        }

        return true;
    }
}

[System.Serializable]
public class AutoPageinator
{
    public List<AutoPageObject> pages;
}

[System.Serializable]
public class AutoPageObject
{
    public List<string> text;
}