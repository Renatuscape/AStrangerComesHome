using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalPageScript : MonoBehaviour
{
    public JournalScript journalManager;
    public JournalPage pageType;
    public GameObject pageContent;
    void Start()
    {
        journalManager = transform.parent.GetComponent<JournalScript>();
    }

    void Update()
    {
        if (journalManager.currentJournalPage != pageType)
            pageContent.SetActive(false);
        else if (journalManager.currentJournalPage == pageType)
            pageContent.SetActive(true);
    }
}
