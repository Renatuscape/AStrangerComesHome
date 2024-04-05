using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionsBooks : MonoBehaviour
{
    public GameObject reader;
    public GameObject bookContainer;
    public GameObject bookDetails;
    public List<GameObject> prefabs;

    void Start()
    {
        reader.SetActive(false);
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        foreach (Item item in Items.all)
        {
            if (item.type == ItemType.Book) // filter by inventory later
            {
                var newBook = BoxFactory.CreateItemIcon(item, true, 64, 18);
                prefabs.Add(newBook);
                newBook.transform.SetParent(bookContainer.transform, false);
            }
        }
    }

    private void OnDisable()
    {
        foreach (var prefab in prefabs)
        {
            Destroy(prefab);
        }
        prefabs.Clear();

        reader.SetActive(false);
    }
}
