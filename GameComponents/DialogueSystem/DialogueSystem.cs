using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public DataManagerScript dataManager;
    public FontManager fontManager;
    public GameObject button;
    public GameObject topicMenu;
    public GameObject dialogueMenu;
    public GameObject popUpMenu;
    public GameState previousGameState = GameState.Overworld;

    private void Start()
    {
        topicMenu.SetActive(false);
        dialogueMenu.SetActive(false);
        button.GetComponentInChildren<TextMeshProUGUI>().font = fontManager.body.font;
    }
    public void OpenTopicMenu(string speakerID)
    {
        button.GetComponentInChildren<TextMeshProUGUI>().font = fontManager.body.font;
        previousGameState = TransientDataScript.GetGameState();
        topicMenu.SetActive(true);
        topicMenu.GetComponent<TopicMenu>().OpenTopicsMenu(speakerID);
        dialogueMenu.SetActive(false);
    }

    public void StartDialogue(Quest quest) //called primarily from topic manager
    {
        button.GetComponentInChildren<TextMeshProUGUI>().font = fontManager.body.font;

        DialogueTagParser.UpdateTags(dataManager);
        topicMenu.SetActive(false);
        dialogueMenu.SetActive(true);
        dialogueMenu.GetComponent<DialogueMenu>().StartDialogueStage(quest);
    }

    public void CloseTopicMenuAndLeave()
    {
        topicMenu.SetActive(false);
        TransientDataScript.SetGameState(previousGameState, "DialogueSystem", gameObject);
    }

    public void CloseDialogueMenu()
    {
        dialogueMenu.SetActive(false);
        TransientDataScript.SetGameState(previousGameState, "DialogueSystem", gameObject);
    }

    public void ColosePopUpMenu()
    {
        popUpMenu.gameObject.SetActive(false);
        StartCoroutine(GameStateDelay());

        IEnumerator GameStateDelay()
        {
            yield return new WaitForSeconds(1);
            TransientDataScript.SetGameState(GameState.Overworld, "DialogueSystem", gameObject);
        }
    }
}