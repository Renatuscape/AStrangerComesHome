﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BookReader : MonoBehaviour
{
    public FontManager fontManager;
    public ReaderAutoPageLayout autoPageLayout;
    public ReaderVerticalLayout verticalLayout;
    public ReaderHorizontalLayout horizontalLayout;
    public Book book;
    public TextMeshProUGUI bookTitle;

    public RectMask2D rectMask;

    public Image backgroundRend;
    public Sprite backgroundLetter;
    public Sprite backgroundBook;

    public GameObject textPrefab;
    public GameObject vDivPrefab;
    public GameObject hDivPrefab;

    public List<GameObject> printedPrefabs = new();
    bool isBuilding;
    bool isBuildSuccessful;
    private void Update()
    {
        if (!isBuilding && !isBuildSuccessful)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(UnveilText());
            }
            else
            {
                Debug.LogWarning("BookReader was not active in hierarchy.");
                isBuildSuccessful = false;
            }
        }
    }
    public void Initialise(Book bookToRead)
    {
        isBuilding = true;
        CleanReader();
        rectMask.padding = new Vector4(0, 700, 0, 0);

        book = bookToRead;
        bookTitle.text = book.inventoryItem.name;
        SetBackground();

        autoPageLayout.gameObject.SetActive(false);
        verticalLayout.gameObject.SetActive(false);
        horizontalLayout.gameObject.SetActive(false);

        gameObject.SetActive(true);

        if (book.autoPages)
        {
            autoPageLayout.InitialiseAutoBook(book);
        }
        else
        {
            if (!book.horizontalLayout)
            {
                verticalLayout.Initialise(book);
            }
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(UnveilText());
        }
        else
        {
            Debug.LogWarning("Reader was not active in hierarchy.");
            isBuildSuccessful = false;
        }
        isBuilding = false;
    }

    void SetBackground()
    {
        if (book.isLetter)
        {
            backgroundRend.sprite = backgroundLetter;
        }
        else
        {
            backgroundRend.sprite = backgroundBook;
        }
    }

    IEnumerator UnveilText()
    {
        isBuildSuccessful = true;
        yield return new WaitForSeconds(0.5f);
        float padding = rectMask.padding.y;

        while (padding > -1)
        {
            padding = padding - 15;
            rectMask.padding = new Vector4(0, padding, 0, 0);

            yield return new WaitForSeconds(0.0001f);
        }
        rectMask.padding = new Vector4(0, 0, 0, 0);
    }

    public GameObject PrintLine(string text, GameObject container)
    {
        if (text.Contains("#hDiv"))
        {
            var hDiv = Instantiate(hDivPrefab);
            printedPrefabs.Add(hDiv);
            SetParentAndUpdate(hDiv, container);
            return hDiv;
        }
        else if (text.Contains("#vDiv"))
        {
            var vDiv = Instantiate(vDivPrefab);
            printedPrefabs.Add(vDiv);
            SetParentAndUpdate(vDiv, container);
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

            if (text.Contains("#B "))
            {
                parsedText = parsedText.Replace("#B ", "");
                if (!book.autoPages)
                {
                    Debug.Log($"Book {book.objectID} contains a #B break, but is not an auto-book. Cannot add break automatically.");
                }
            }

            textMesh.text = parsedText;
            SetParentAndUpdate(textPrefab, container);
            return textPrefab;
        }
    }

    void SetParentAndUpdate(GameObject prefab, GameObject container)
    {
        Canvas.ForceUpdateCanvases();
        if (!book.horizontalLayout)
        {
            container.GetComponent<VerticalLayoutGroup>().enabled = false;
        }
        else
        {
            container.GetComponent<HorizontalLayoutGroup>().enabled = false;
        }

        prefab.transform.SetParent(container.transform, false);

        if (!book.horizontalLayout)
        {
            container.GetComponent<VerticalLayoutGroup>().enabled = true;
        }
        else
        {
            container.GetComponent<HorizontalLayoutGroup>().enabled = true;
        }

        Canvas.ForceUpdateCanvases();
    }

    public void CleanReader()
    {
        foreach (var prefab in printedPrefabs)
        {
            Destroy(prefab);
        }

        printedPrefabs.Clear();
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