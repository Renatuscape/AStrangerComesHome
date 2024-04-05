using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


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

    public List<GameObject> printedPrefabs;

    public void Initialise(Book bookToRead)
    {
        book = bookToRead;

        if (book.autoPages)
        {
            PrintAutoPageBook();
        }
    }

    void PrintAutoPageBook()
    {
        if (book.pages != null && book.pages.Count != 1)
        {
            Debug.Log("An autopage book should have no more or less than 1 page.");
        }
        else
        {
            // Print everything to test
            foreach (string text in book.pages[0].text)
            {
                var prefab = PrintLine(text);

                if (book.horizontalLayout)
                {
                    prefab.transform.SetParent(pageH.transform, false);
                }
                else
                {
                    prefab.transform.SetParent(pageA.transform, false);
                }
            }
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
            }
            else if (text.Contains("#S "))
            {
                parsedText = parsedText.Replace("#S ", "");
                textMesh.font = fontManager.subtitle.font;
                textMesh.fontSize= fontManager.subtitle.fontSize;
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

    private void OnDisable()
    {
        CleanReader();
    }
}