using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PostBoxNode : MonoBehaviour
{
    public GameObject alertBobber;
    public List<Dialogue> letterList = new();
    public BoxCollider2D col;
    public bool playerAddress = false;
    public string addressID;
    void Start()
    {
        alertBobber.gameObject.SetActive(false);
        col = GetComponent<BoxCollider2D>();
        addressID = TransientDataScript.gameManager.dataManager.postLocationID;

        if (TransientDataScript.GetCurrentLocation().objectID == addressID)
        {
            playerAddress = true;

            GetLetterList();

            if (letterList.Count > 0)
            {
                alertBobber.gameObject.SetActive(true);
            }
        }
    }

    private void OnMouseDown()
    {
        if (letterList.Count > 0)
        {
            Book topLetter = Books.FindByID(letterList[0].content[0] + "-TEXT");

            if (topLetter != null)
            {
                Player.Add(topLetter.inventoryItem.objectID);
                Player.Add(letterList[0].questID, 1, false);
                letterList.RemoveAt(0);

                JournalController.ForceReadBook(topLetter);

                if (letterList.Count < 1)
                {
                    alertBobber.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("No letter found with ID " + letterList[0].content[0]);
            }
        }
        else
        {
            if (playerAddress)
            {
                LogAlert.QueueTextAlert("No new letters at the moment.");
            }
            else
            {
                LogAlert.QueueTextAlert($"I can check my mail in {Locations.FindByID(addressID).name}.");
            }
        }
    }

    void GetLetterList()
    {
        foreach (Quest quest in Quests.all)
        {
            var questStage = Player.GetCount(quest.objectID, name);

            if (questStage < 100)
            {
                Dialogue storyEvent = quest.dialogues.FirstOrDefault(d => d.questStage == questStage);

                if (storyEvent != null && storyEvent.stageType == StageType.Letter && storyEvent.CheckRequirements())
                {
                    letterList.Add(storyEvent);
                }
            }
        }
    }
}
