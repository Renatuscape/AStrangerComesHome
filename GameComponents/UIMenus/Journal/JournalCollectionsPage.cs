using System.Collections;
using UnityEngine;

public enum CollectionPage
{
    Recipes,
    Treasures,
    Cards,
    People,
    Books
}

public class JournalCollectionsPage : MonoBehaviour
{
    public FontManager fontManager;
    public CollectionPage page = CollectionPage.Recipes;
    public GameObject recipes;
    public GameObject people;
    public GameObject cards;
    public GameObject treasures;
    public GameObject books;

    public void EnableRecipes()
    {
        ChangePage(CollectionPage.Recipes);
    }

    public void EnablePeople()
    {
        ChangePage(CollectionPage.People);
    }

    public void EnableCards()
    {
        ChangePage(CollectionPage.Cards);
    }   

    public void EnableTreasures()
    {
        ChangePage(CollectionPage.Treasures);
    }

    public void EnableBooks()
    {
        ChangePage(CollectionPage.Books);
    }

    void ChangePage(CollectionPage page)
    {
        recipes.SetActive(page == CollectionPage.Recipes);
        people.SetActive(page == CollectionPage.People);
        cards.SetActive(page == CollectionPage.Cards);
        treasures.SetActive(page == CollectionPage.Treasures);
        books.SetActive(page == CollectionPage.Books);
    }
}
