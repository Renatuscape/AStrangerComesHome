using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBobber : MonoBehaviour
{
    public DialogueDisplay dialogueDisplay;
    public GameObject continueBobber;
    public GameObject endBobber;

    void Update()
    {
        if (!dialogueDisplay.waitingForChoice && !dialogueDisplay.autoEnabled && !dialogueDisplay.endConversation && !continueBobber.activeInHierarchy)
        {
            continueBobber.SetActive(true);
            endBobber.SetActive(false);
        }
        else if (!dialogueDisplay.isPrinting && dialogueDisplay.endConversation && !endBobber.activeInHierarchy)
        {
            continueBobber.SetActive(false);
            endBobber.SetActive(true);
        }
        else
        {
            if ((dialogueDisplay.waitingForChoice || dialogueDisplay.autoEnabled || dialogueDisplay.endConversation) && continueBobber.activeInHierarchy)
            {
                continueBobber.SetActive(false);
            }
            if (!dialogueDisplay.isPrinting && !dialogueDisplay.endConversation && endBobber.activeInHierarchy)
            {
                endBobber.SetActive(false);
            }
        }
    }
}
