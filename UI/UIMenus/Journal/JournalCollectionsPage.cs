using System.Collections;
using UnityEngine;

public enum CollectionPage
{
    None,
    Recipes,
    Treasures,
    Letters,
    People,
    Books,
    Memories
}

public class JournalCollectionsPage : MonoBehaviour
{
    public FontManager fontManager;
    public CollectionPage page = CollectionPage.Recipes;
    public GameObject recipes;
    public GameObject people;
    public GameObject treasures;
    public GameObject memories;
    public CollectionsBooks books;

    private void Awake()
    {
        ChangePage(CollectionPage.None);
    }

    public void EnableRecipes()
    {
        ChangePage(CollectionPage.Recipes);
    }

    public void EnablePeople()
    {
        ChangePage(CollectionPage.People);
    }

    public void EnableLetters()
    {
        ChangePage(CollectionPage.Letters);
    }   

    public void EnableTreasures()
    {
        ChangePage(CollectionPage.Treasures);
    }

    public void EnableBooks()
    {
        ChangePage(CollectionPage.Books);
    }

    public void EnableMemories()
    {
        ChangePage(CollectionPage.Memories);
    }

    void ChangePage(CollectionPage page)
    {
        recipes.SetActive(page == CollectionPage.Recipes);
        people.SetActive(page == CollectionPage.People);
        treasures.SetActive(page == CollectionPage.Treasures);
        memories.SetActive(page == CollectionPage.Memories);

        if (page == CollectionPage.Books)
        {
            books.Initialise(false);
        }
        else if (page == CollectionPage.Letters)
        {
            books.Initialise(true);
        }
        else
        {
            books.gameObject.SetActive(false);
        }
    }
}
