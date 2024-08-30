using UnityEngine;

public class JournalController : MonoBehaviour
{
    public static JournalController instance;
    public Journal journal;
    public JournalCollectionsPage collectionsPage;
    public CollectionsBooks bookCollection;
    public BookReader bookReader;

    private void Awake()
    {
        instance = this;
    }
    public void EnableJournal()
    {
        TransientDataScript.SetGameState(GameState.JournalMenu, "Journal.ForceReader", gameObject);
        journal.gameObject.SetActive(true);
        journal.mainPage = JournalMainPage.Collections;
    }

    public void ForceReader(Book book)
    {
        EnableJournal();

        if (book.isLetter)
        {
            collectionsPage.EnableLetters();
        }
        else
        {
            collectionsPage.EnableBooks();
        }

        bookReader.Initialise(book);
    }
    public static void ForceReadBook(Book book)
    {
        instance.ForceReader(book);
    }
}
