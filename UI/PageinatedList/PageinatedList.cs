using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageinatedList : MonoBehaviour
{
    public bool noCategories;
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
        categoryNames.Clear();

        if (categoryList.Count > 0)
        {
            foreach (var list in categoryList)
            {
                noCategories = true;
                categoryContainer.gameObject.SetActive(false);
                pageLimit = CalculatePageLimit();
                BuildPages(list.categoryName, list.listContent);
                categoryNames.Add(list.categoryName);
                CreateCategory(list.categoryName);
            }

            gameObject.SetActive(true);

            pageArchive = new List<ListPage>(pages);

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
        var newCategory = Instantiate(categoryPrefab);
        newCategory.name = "btn" + categoryName;
        newCategory.transform.SetParent(categoryContainer.transform, false);
        categoryPrefabs.Add(newCategory);

        var script = newCategory.AddComponent<ListCategoryPrefab>();
        script.title.text = categoryName;
        script.btnCategory.onClick.AddListener(() => ChangeCategory(categoryName));
    }

    public List<GameObject> InitialiseWithoutCategories(List<IdIntPair> entryList)
    {
        categoryNames.Clear();

        if (entryList.Count > 0)
        {
            noCategories = true;
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
        }

        if (pages.Count < 2)
        {
            btnNextPage.gameObject.SetActive(false);
        }
        else
        {
            btnNextPage.gameObject.SetActive(true);
        }

        if (pageIndex >= pages.Count)
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

public class ListCategoryPrefab: MonoBehaviour
{
    public TextMeshProUGUI title;
    public Button btnCategory;
}