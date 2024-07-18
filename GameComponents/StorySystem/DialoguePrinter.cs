using System.Collections;
using UnityEngine;

public class DialoguePrinter : MonoBehaviour
{
    DialogueDisplay dialogueParent;
    public bool readyToPrint = false;
    public string textToPrint;
    public string[] textArray;
    public int textIndex;
    public float printSpeed;

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
            UpdatePrintSpeed();
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

            StartCoroutine(PrintContent());
        }
    }
    IEnumerator PrintContent()
    {
        while (textIndex < textArray.Length)
        {
            PrintWord();

            if (printSpeed != 0) // Do not reset print speed
            {
                UpdatePrintSpeed();
            }

            yield return new WaitForSeconds(printSpeed);
        }

        dialogueParent.isPrinting = false;
    }

    void PrintWord()
    {
        var wordToPrint = textArray[textIndex];


        if (printSpeed == 0)
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

    void UpdatePrintSpeed()
    {
        if (GlobalSettings.TextSpeed == 4)
        {
            printSpeed = 0;
        }
        else
        {
            printSpeed = 0.2f / GlobalSettings.TextSpeed;
        }
    }
}