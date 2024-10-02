using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageinatedList : MonoBehaviour
{
    public bool noCategories;
    public GameObject categoryPrefab;
    public GameObject listItemPrefab;
    public GameObject listContainer;
    public GameObject categoryContainer;
    public List<IdIntPair> content;
    public List<GameObject> prefabs = new();
    public int pageLimit;
    public int pageIndex;
    public List<ListPage> pages = new();

    public Button btnNextPage;

    private void Start()
    {
        btnNextPage.onClick.RemoveAllListeners();
        btnNextPage.onClick.AddListener(() =>
        {
            pageIndex++;

            if (pageIndex > pages.Count)
            {
                pageIndex = 1;
            }

            OpenPage(pageIndex);
        });
    }
    public List<GameObject> InitialiseWithoutCategories(List<IdIntPair> listItems)
    {
        if (listItems.Count > 0)
        {
            noCategories = true;
            categoryContainer.gameObject.SetActive(false);
            content = listItems;
            pageLimit = CalculatePageLimit();
            BuildPages("Default");
            gameObject.SetActive(true);

            if (pages.Count < 2)
            {
                btnNextPage.gameObject.SetActive(false);
            }
            else
            {
                btnNextPage.gameObject.SetActive(true);
            }

            pageIndex = 1;
            OpenPage(pageIndex);

            return prefabs;
        }
        else
        {
            btnNextPage.gameObject.SetActive(false);
        }

        return null;
    }

    public void OpenPage(int pageNumber)
    {
        foreach (var item in pages)
        {
            if (item.pageNumber == pageNumber)
            {
                foreach (var listItem in item.listItems)
                {
                    listItem.SetActive(true);
                }
            }
            else
            {
                foreach (var listItem in item.listItems)
                {
                    listItem.SetActive(false);
                }
            }
        }
    }

    void BuildPages(string categoryName)
    {
        ClearPrefabs();

        int i = 0;
        int pageNumber = 1;
        ListPage currentPage = new();
        currentPage.category = categoryName;
        pages.Add(currentPage);

        currentPage.pageNumber = pageNumber;
        currentPage.listItems = new();

        foreach (IdIntPair entry in content)
        {
            if (i >= pageLimit)
            {
                i = 0;

                pageNumber++;

                var newPage = new ListPage();
                newPage.pageNumber = pageNumber;
                pages.Add(newPage);
                currentPage = newPage;
                currentPage.listItems = new();
                currentPage.category = categoryName;
            }

            if (i < pageLimit)
            {
                var newItem = Instantiate(listItemPrefab);
                newItem.transform.SetParent(listContainer.transform, false);
                newItem.GetComponent<ListItemPrefab>().entry = entry;
                newItem.GetComponent<ListItemPrefab>().textMesh.text = DialogueTagParser.ParseText(entry.description);

                newItem.SetActive(false);

                currentPage.listItems.Add(newItem);
                prefabs.Add(newItem);

                i++;
            }
        }
    }

    void ClearPrefabs()
    {
        foreach (var obj in prefabs)
        {
            Destroy(obj);
        }

        prefabs.Clear();
        pages.Clear();
    }

    int CalculatePageLimit()
    {
        float itemHeight = listItemPrefab.GetComponent<RectTransform>().rect.height;

        float spacing = listContainer.GetComponent<VerticalLayoutGroup>().spacing;

        float containerHeight = listContainer.GetComponent<RectTransform>().rect.height;

        float totalItemHeight = itemHeight + spacing;
        int pageLimit = Mathf.FloorToInt((containerHeight + spacing) / totalItemHeight);

        return pageLimit;
    }

    private void OnDisable()
    {
        ClearPrefabs();
    }
}

[Serializable]
public class ListPage
{
    public string category;
    public int pageNumber;
    public List<GameObject> listItems;
}
