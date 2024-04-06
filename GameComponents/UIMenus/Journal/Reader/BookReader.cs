using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BookReader : MonoBehaviour
{
    public FontManager fontManager;
    public Book book;

    public GameObject verticalLayout;
    public GameObject pageA;
    public GameObject pageB;

    public GameObject horizontalLayout;
    public GameObject pageH;

    public GameObject textPrefab;
    public GameObject vDivPrefab;
    public GameObject hDivPrefab;

    public List<GameObject> printedPrefabs = new();
    public Page activePage;
    public int paragraphIndex;
    public int pageIndex;

    public AutoPageinator autoPageinator;

    public void Initialise(Book bookToRead)
    {
        CleanReader();

        book = bookToRead;
        gameObject.SetActive(true);

        if (book.autoPages)
        {
            PrintAutoPageBook();
        }
    }

    public void BtnForward()
    {
        if (book.autoPages)
        {
            pageIndex += 2;

            if (pageIndex >= autoPageinator.pages.Count)
            {
                pageIndex = 0;
            }

            PrintAutoBookPage();
        }
    }

    public void BtnBack()
    {
        pageIndex -= 2;

        if (pageIndex < 0)
        {
            pageIndex = 0;
        }

        PrintAutoBookPage(true);
    }

    void PrintAutoPageBook()
    {
        if (book.pages != null && book.pages.Count != 1)
        {
            Debug.Log("An autopage book should have no more or less than 1 page.");
        }
        else
        {
            activePage = book.pages[0];
            pageIndex = 0;
            BuildAutoBook();
            PrintAutoBookPage();
        }
    }

    void PrintAutoBookPage(bool goingBack = false)
    {
        CleanReader();

        foreach (var text in autoPageinator.pages[pageIndex].text)
        {
            var prefab = PrintLine(text);
            prefab.transform.SetParent(pageA.transform, false);
        }

        if (autoPageinator.pages.Count > pageIndex + 1)
        {
            if (autoPageinator.pages[pageIndex + 1].text != null && autoPageinator.pages[pageIndex + 1].text.Count > 0)
            {
                foreach (var text in autoPageinator.pages[pageIndex + 1].text)
                {
                    var prefab = PrintLine(text);
                    prefab.transform.SetParent(pageB.transform, false);
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

            var prefab = PrintLine(activePage.text[i]);

            prefab.transform.SetParent(pageA.transform, false);

            if (!CheckOverflow(pageA.GetComponent<RectTransform>()) ||
                (i != 0 && activePage.text[i].Contains("#H ")) ||
                activePage.text[i].Contains("#S "))
            {
                autoPageinator.pages.Add(new());
                autoPageIndex++;
                autoPageinator.pages[autoPageIndex].text = new();
                CleanReader();
            }

            autoPageinator.pages[autoPageIndex].text.Add(activePage.text[i]);

            Canvas.ForceUpdateCanvases();
        }
    }

    GameObject PrintLine(string text)
    {
        if (text.Contains("#hDiv"))
        {
            var hDiv = Instantiate(hDivPrefab);
            printedPrefabs.Add(hDiv);
            return hDiv;
        }
        else if (text.Contains("#vDiv"))
        {
            var vDiv = Instantiate(vDivPrefab);
            printedPrefabs.Add(vDiv);
            return vDiv;
        }
        else if (text.Contains("IMG-"))
        {
            Debug.Log("Image call detected. Add logic later");
            return null;
        }
        else
        {
            var textPrefab = Instantiate(this.textPrefab);
            printedPrefabs.Add(textPrefab);

            string parsedText = DialogueTagParser.ParseText(text);
            var textMesh = textPrefab.GetComponentInChildren<TextMeshProUGUI>();

            if (text.Contains("#H "))
            {
                parsedText = parsedText.Replace("#H ", "");
                textMesh.font = fontManager.header.font;
                textMesh.fontSize = fontManager.header.fontSize;
                textMesh.margin = new Vector4(textMesh.margin.x, textMesh.margin.y + 10, textMesh.margin.z, textMesh.margin.w + 5);
            }
            else if (text.Contains("#S "))
            {
                parsedText = parsedText.Replace("#S ", "");
                textMesh.font = fontManager.subtitle.font;
                textMesh.fontSize = fontManager.subtitle.fontSize;
                textMesh.margin = new Vector4(textMesh.margin.x, textMesh.margin.y + 5, textMesh.margin.z, textMesh.margin.w);
            }
            else
            {
                textMesh.font = fontManager.body.font;
                textMesh.fontSize = fontManager.body.fontSize;
            }

            if (text.Contains("#I "))
            {
                parsedText = parsedText.Replace("#I ", "");
                textMesh.fontStyle = FontStyles.Italic;
            }

            textMesh.text = parsedText;

            return textPrefab;
        }
    }

    void CleanReader()
    {
        foreach (var prefab in printedPrefabs)
        {
            Destroy(prefab);
        }

        printedPrefabs.Clear();
    }

    bool CheckOverflow(RectTransform parentRect)
    {
        float parentHeight = parentRect.rect.height;
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
                    // Adjust the height calculation based on the text content and other properties
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
            return false;
        }

        return true;
    }

    private void OnDisable()
    {
        if (!gameObject.scene.isLoaded) return;
        else
        {
            CleanReader();
        }
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