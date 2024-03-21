using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPrefab : MonoBehaviour
{
    public Quest quest;
    public JournalQuestPage journalQuestPage;

    public void DisplayQuestDetails()
    {
        journalQuestPage.DisplayQuestDetails(quest);
    }
}
