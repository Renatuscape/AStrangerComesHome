using System.Collections;
using TMPro;
using UnityEngine;

public class GenericTextPrinter : MonoBehaviour
{
    public string textToPrint;
    public string[] textArray;
    public int textIndex;
    public float printSpeed;
    public bool isPrinting = false;
    public AudioSource textSoundEffect;
    public void StartPrint(string textToPrint, TextMeshProUGUI textMesh)
    {
        if (textSoundEffect.clip == null)
        {
            textSoundEffect.clip = AudioManager.GetSoundEffect("knockSmall");
        }

        textSoundEffect.volume = GlobalSettings.EffectVolume - 0.1f;

        this.textToPrint = textToPrint;
        textArray = textToPrint.Split(' ');

        textIndex = 0;
        UpdatePrintSpeed();
        isPrinting = true;
        textMesh.text = "";

        StartCoroutine(PrintContent(textMesh));
    }
    IEnumerator PrintContent(TextMeshProUGUI textMesh)
    {
        while (textIndex < textArray.Length)
        {
            PrintWord(textMesh);

            if (printSpeed != 0) // Do not reset print speed
            {
                UpdatePrintSpeed();
            }

            yield return new WaitForSeconds(printSpeed);
        }

        isPrinting = false;
    }

    void PrintWord(TextMeshProUGUI textMesh)
    {
        var wordToPrint = textArray[textIndex];


        if (printSpeed == 0)
        {
            textMesh.text = textToPrint;
            textIndex = textArray.Length;
        }
        else
        {
            textMesh.text += wordToPrint + " ";
            textIndex++;
        }

        if (!GlobalSettings.DisableTextSound && textSoundEffect.clip != null)
        {
            textSoundEffect.Play();
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