using System.Collections;
using UnityEngine;

public class DialoguePrinter : MonoBehaviour
{
    DialogueDisplay dialogueParent;
    public bool readyToPrint = false;
    public string textToPrint;
    public string[] textArray;
    public int textIndex;

    public void Initialise(DialogueDisplay dialogueDisplay)
    {
        dialogueParent = dialogueDisplay;
        readyToPrint = true;
    }
    public void StartPrint(string textToPrint, bool isNarration)
    {
        if (readyToPrint)
        {
            this.textToPrint = textToPrint;
            textArray = textToPrint.Split(' ');

            textIndex = 0;
            dialogueParent.printSpeed = 0.08f;
            dialogueParent.isPrinting = true;
            dialogueParent.contentText.text = "";

            if (isNarration)
            {
                dialogueParent.contentText.color = new Color(dialogueParent.contentText.color.r, dialogueParent.contentText.color.g, dialogueParent.contentText.color.b, 0.7f);
            }
            else
            {
                dialogueParent.contentText.color = new Color(dialogueParent.contentText.color.r, dialogueParent.contentText.color.g, dialogueParent.contentText.color.b, 1);
            }

            StartCoroutine(Print());
        }
    }
    IEnumerator Print()
    {
        while (textIndex < textArray.Length)
        {
            PrintStep();
            yield return new WaitForSeconds(dialogueParent.printSpeed);
        }

        dialogueParent.isPrinting = false;
    }

    void PrintStep()
    {
        var wordToPrint = textArray[textIndex];


        if (dialogueParent.printSpeed == 0)
        {
            dialogueParent.contentText.text = textToPrint;
            textIndex = textArray.Length;
        }
        else
        {
            dialogueParent.contentText.text += wordToPrint + " ";
            textIndex++;
        }

        if (dialogueParent.textSoundEffect.clip != null)
        {
            dialogueParent.textSoundEffect.Play();
        }
    }
}