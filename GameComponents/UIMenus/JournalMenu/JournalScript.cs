using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum JournalPage
{
    Inventory,
    Profile, //skills and upgrades
    Quests,
    Archive,//spells and recipes
    Collections //collectable items and unique treasures
}
public class JournalScript : MonoBehaviour
{
    public int numberofPages = System.Enum.GetValues(typeof(JournalPage)).Length;
    public int journalIndex;
    public JournalPage currentJournalPage;
    public GameObject Inventory;
    public GameObject Profile;
    public GameObject Quests;
    public GameObject Archive;
    public GameObject Collections;
    void Update()
    {
        if (TransientDataScript.GameState == GameState.JournalMenu)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                if (journalIndex < numberofPages - 1)
                    journalIndex++;
                else
                    journalIndex = 0;

            }

            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                if (journalIndex > 0)
                    journalIndex--;
                else
                    journalIndex = numberofPages - 1;

            }
        }

        currentJournalPage = (JournalPage)journalIndex;
    }

    void OnEnable()
    {
        Inventory.SetActive(true);
        Profile.SetActive(true);
        Archive.SetActive(true);
        Quests.SetActive(true);
        Collections.SetActive(true);
    }
}
