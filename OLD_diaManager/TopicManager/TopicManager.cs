using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopicManager : MonoBehaviour
{
    public TransientDataScript transientData;
    public DataManagerScript dataManager;
    public Character topicMaster;
    public List<Quest> questList;
    public GameObject whatTopicPrefab;
    public GameObject topicButton;
    public GameObject leaveButton;
    public GameObject dialogueContainer;

    public GameState previousGameState;
    public List<GameObject> printedTopics;

    public List<Quest> characterTopics;
    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();

        FindQuestObjects();
        gameObject.SetActive(false);
    }

    public void OpenTopicManager(Character c)
    {
        topicMaster = c;
        gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        if (questList.Count < 1)
        {
            Invoke("FetchTopics", 0.06f);
        }
        else
        {
            FetchTopics();
        }

        previousGameState = TransientDataScript.GameState;
        TransientDataScript.SetGameState(GameState.Dialogue, "TopicManager", gameObject);
        //transientData.gameState = GameState.Dialogue;
        //Debug.Log(name + " changed GameState to " + GameState.Dialogue);
    }
    private void OnDisable()
    {
        DestroyTopics();
        characterTopics.Clear();

        if (previousGameState == GameState.ShopMenu || previousGameState == GameState.Overworld)
            TransientDataScript.SetGameState(previousGameState, "TopicManager", gameObject);
    }
    private void Update()
    {
        if (TransientDataScript.GameState != GameState.Dialogue)
            gameObject.SetActive(false);
    }
    public void DestroyTopics()
    {
        foreach (GameObject topic in printedTopics)
        {
            Destroy(topic.gameObject);
        }
        printedTopics.Clear();
    }

    void FindQuestObjects()
    {
        var placeHolderList = new List<MotherObject>();
        foreach (Quest q in Quests.all)
        {
            if (!questList.Contains(q))
            {
                questList.Add(q); //collect all game quests in a list
            }
        }
        if (questList.Count < 1)
        {
            Invoke("FindQuestObjects", 0.05f);
        }
    }

    void FetchTopics()
    {
        if (characterTopics.Count > 0)
            characterTopics.Clear();

        if (topicMaster != null)
        {
            dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = false;

            //if (topicMaster.bond > 0)
            //    GetRegularTopics();
            //else
                GetGreetingTopic();
        }

        dialogueContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    void GetGreetingTopic()
    {
        //foreach (Quest q in questList)
        //{
        //    if (q.firstMeeting == true)
        //    {
        //        Debug.Log(q + " is a first meeting quest. The topic master is " + q.dialogues[0].topicMaster);
        //        if (q.dialogues[0].topicMaster == topicMaster)
        //        {
        //            characterTopics.Add(q);
        //            break; //EACH CHARACTER SHOULD ONLY HAVE ONE WELCOME TOPIC
        //        }
        //    }
        //}
        //if (characterTopics.Count == 0)
        //{
        //    Debug.LogWarning("No greetning found for " + topicMaster);
        //    //topicMaster.bond++;
        //    FetchTopics();
        //}
        //else
        //{
        //    var quest = characterTopics[0];
        //    var dialogueManager = gameObject.GetComponent<DialogueManager>();

        //    dialogueManager.quest = quest;
        //    dialogueManager.dialogueIndex = 0;
        //    //dialogueManager.PrintChoiceText(quest.dialogues[quest.dataValue].topicName);
        //    dialogueManager.PrintDialogue();
        //    DestroyTopics();
        //}
    }

    void GetRegularTopics()
    {

        ////Filter topics. If quest does not appear properly, check if Dialogue object has correct topicMaster
        //foreach (Quest q in questList)
        //{
        //    var checkPassed = true;
        //    //ADD CHECKS FOR DAYS PASSED AND MOTHER OBJECT DATALEVELS HERE

        //    /*if (topicMaster.bond < 1)
        //    {
        //        checkPassed = false;
        //    }
        //    if (dataManager.totalGameDays < q.daysPassedCheck)
        //    {
        //        checkPassed = false;
        //    }*/

        //    if (q.checkLessThan.Count > 0)
        //    {
        //        for (int index = 0; index < q.checkLessThan.Count; index++)
        //        {
        //            if (q.checkLessThan[index].dataValue > q.checkLessThanValue)
        //            {
        //                checkPassed = false;
        //                break;
        //            }
        //        }
        //    }

        //    if (q.checkMoreThan.Count > 0)
        //    {
        //        for (int index = 0; index < q.checkMoreThan.Count; index++)
        //        {
        //            if (q.checkMoreThan[index].dataValue < q.checkMoreThanValue)
        //            {
        //                checkPassed = false;
        //                break;
        //            }
        //        }
        //    }

        //    if (checkPassed == true && q.dialogues.Count > q.dataValue)
        //    {
        //        if (q.dialogues.Count >= q.dataValue + 1)
        //        {
        //            if (q.dialogues[q.dataValue] != null)
        //            {
        //                if (q.dialogues[q.dataValue].topicMaster == topicMaster)
        //                {
        //                    characterTopics.Add(q);
        //                    //Debug.Log("Topic added: " + q.name);
        //                }
        //            }
        //            else
        //                Debug.LogWarning("Dialogue missing in " + q.name + " quest at dialogue index + "[q.dataValue]);
        //        }
        //    }
        //}
        //PrintTopics();
    }

    void PrintTopics()
    {
        ////Print "What should I talk about?" (always on top)
        //var whatPrefab = Instantiate(whatTopicPrefab);
        //whatPrefab.transform.SetParent(dialogueContainer.transform, false);
        //printedTopics.Add(whatPrefab);

        ////Print topics
        //foreach (Quest q in characterTopics)
        //{
        //    var topicPrefab = Instantiate(topicButton);
        //    var text = topicPrefab.gameObject.transform.GetChild(0);
        //    var topicName = q.dialogues[q.dataValue].topicName;

        //    var topicScript = topicPrefab.GetComponent<TopicButton>();
        //    topicScript.topicManager = this;
        //    topicScript.dialogueManager = gameObject.GetComponent<DialogueManager>();
        //    topicScript.quest = q;

        //    topicPrefab.name = topicName;
        //    text.GetComponent<TextMeshProUGUI>().text = topicName;
        //    topicPrefab.transform.SetParent(dialogueContainer.transform, false);
        //    printedTopics.Add(topicPrefab);
        //}

        ////Print leave button (always on bottom)
        //var leavePrefab = Instantiate(leaveButton); //must be last
        //leavePrefab.transform.SetParent(dialogueContainer.transform, false);
        //leavePrefab.GetComponent<LeaveButton>().dialogueManager = gameObject;
        //printedTopics.Add(leavePrefab);
    }
}
