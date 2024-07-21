using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReusableDialogueBox : MonoBehaviour
{
    public TextMeshProUGUI printedText;
    public Image portrait;
    public GenericTextPrinter printer;
    public Button btnConfirm;
    public Button btnClose;

    private void Start()
    {
        btnClose.onClick.AddListener(() => gameObject.SetActive(false));
        printedText.AddComponent<Button>().onClick.AddListener(() => ForceCompletePrint());
    }

    public void ForceCompletePrint()
    {
        if (printer.isPrinting)
        {
            printer.printSpeed = 0;
        }
    }

    public void SetPortrait(string characterID, string eventID = "", float newX = -1, float newY = -1)
    {
        var newSprite = SpriteFactory.GetUiSprite(characterID);

        if (newSprite != null)
        {
            if (!string.IsNullOrEmpty(eventID))
            {
                portrait.sprite = newSprite.GetFirstFrameFromEvent(eventID);
            }
            else
            {
                portrait.sprite = newSprite.GetDefaultFrame();
            }

            if (newX != -1 && newY != -1)
            {
                portrait.gameObject.transform.localPosition = new Vector3(newX, newY, 0);
            }
        }
    }

    public void OpenAndPrintText(string text, UnityAction confirmAction = null)
    {
        gameObject.SetActive(true);
        printer.StartPrint(text, printedText);

        if (confirmAction != null)
        {
            StartCoroutine(WaitAndConfigureConfirm(confirmAction));
        }
    }

    IEnumerator WaitAndConfigureConfirm(UnityAction confirmAction)
    {
        btnConfirm.onClick.RemoveAllListeners();

        while (printer.isPrinting)
        {
            yield return new WaitForSeconds(0.1f);
        }

        btnConfirm.onClick.AddListener(confirmAction);
        btnConfirm.onClick.AddListener(() => btnConfirm.gameObject.SetActive(false));
        btnConfirm.onClick.AddListener(() => gameObject.SetActive(false));
        btnConfirm.gameObject.SetActive(true);
    }
}
