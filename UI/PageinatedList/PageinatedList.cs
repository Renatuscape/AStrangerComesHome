using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageinatedList : MonoBehaviour
{
    public GameObject categoryPrefab;
    public GameObject listItemPrefab;
    public GameObject listContainer;
    public GameObject categoryContainer;
    public List<GameObject> categoryPrefabs = new();
    public List<GameObject> entryPrefabs = new();
    public int pageLimit;
    public int pageIndex;
    public List<ListPage> pages = new();
    public List<ListPage> pageArchive = new();
    public List<string> categoryNames = new();

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

    public List<GameObject> InitialiseWithCategories(List<ListCategory> categoryList)
    {
        ClearPrefabs();

        Debug.Log("PList: Initialising with categories. List entries: " + categoryList.Count);

        if (categoryList.Count > 0)
        {
            foreach (var list in categoryList)
            {
                Debug.Log("PList: Settomg up category: " + list.categoryName);
                pageLimit = CalculatePageLimit();
                BuildPages(list.categoryName, list.listContent);
                categoryNames.Add(list.categoryName);
                CreateCategory(list.categoryName);
            }

            gameObject.SetActive(true);
            categoryContainer.SetActive(true);

            pageArchive = new List<ListPage>();

            foreach (var page in pages)
            {
                pageArchive.Add(page);
            }

            ChangeCategory(categoryNames[0]);

            return entryPrefabs;
        }
        else
        {
            btnNextPage.gameObject.SetActive(false);
        }

        return null;
    }

    void CreateCategory(string categoryName)
    {
        Debug.Log("PList: Attempting to create category: " + categoryName);
        Debug.Log("PList: Number of categories currently: " + categoryPrefabs.Count);

        var newCategory = Instantiate(categoryPrefab);
        newCategory.name = "btn" + categoryName;
        newCategory.transform.SetParent(categoryContainer.transform, false);
        categoryPrefabs.Add(newCategory);

        var script = newCategory.GetComponent<ListCategoryPrefab>();
        script.title.text = categoryName;
        script.btnCategory.onClick.AddListener(() => ChangeCategory(categoryName));

        Debug.Log("PList: Updated number of categories: " + categoryPrefabs.Count);
    }

    public List<GameObject> InitialiseWithoutCategories(List<IdIntPair> entryList)
    {
        ClearPrefabs();

        if (entryList.Count > 0)
        {
            categoryContainer.gameObject.SetActive(false);
            pageLimit = CalculatePageLimit();
            BuildPages("Default", entryList);
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

            return entryPrefabs;
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

    void BuildPages(string categoryName, List<IdIntPair> content)
    {
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
                entryPrefabs.Add(newItem);

                i++;
            }
        }
    }

    void ClearPrefabs()
    {
        foreach (var obj in entryPrefabs)
        {
            Destroy(obj);
        }

        foreach (var obj in categoryPrefabs)
        {
            Destroy(obj);
        }

        entryPrefabs.Clear();
        categoryPrefabs.Clear();
        categoryNames.Clear();
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

    void ChangeCategory(string category)
    {
        pages.Clear();

        foreach (var page in pageArchive)
        {
            if (page.category == category)
            {
                pages.Add(page);
            }

            foreach (var listItem in page.listItems)
            {
                listItem.SetActive(false);
            }
        }

        if (pages.Count < 2)
        {
            btnNextPage.gameObject.SetActive(false);
        }
        else
        {
            btnNextPage.gameObject.SetActive(true);
        }

        if (pageIndex >= pages.Count || pageIndex == 0)
        {
            pageIndex = 1;
        }

        OpenPage(pageIndex);
    }
}

[Serializable]
public class ListPage
{
    public string category;
    public int pageNumber;
    public List<GameObject> listItems;
}

[Serializable]
public class ListCategory
{
    public string categoryName;
    public List<IdIntPair> listContent;
}
