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
    public Character character;
    public SpriteCollection spriteData;
    public Image portrait;
    public GenericTextPrinter printer;
    public Button btnConfirm;
    public Button btnClose;
    bool portraitIsReady = false;

    private void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        });

        printedText.AddComponent<Button>().onClick.AddListener(() => ForceCompletePrint());
    }

    public void ForceCompletePrint()
    {
        if (printer.isPrinting)
        {
            printer.printSpeed = 0;
        }
    }

    public void SetExpression(string eventID)
    {
        if (portraitIsReady)
        {
            portrait.sprite = spriteData.GetFirstFrameFromEvent(eventID);
        }
    }

    public void SetupPortrait(string characterID, bool setSpriteToDefault = true, float newX = -1, float newY = -1)
    {
        character = Characters.FindByID(characterID);
        spriteData = SpriteFactory.GetUiSprite(characterID);

        if (setSpriteToDefault)
        {
            portrait.sprite = spriteData.GetDefaultFrame();
        }

        if (newX != -1 && newY != -1)
        {
            portrait.gameObject.transform.localPosition = new Vector3(newX, newY, 0);
        }

        portraitIsReady = true;
    }
    public void ForceAnyPortrait(string characterID, string eventID = "", float newX = -1, float newY = -1)
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

    public void OpenAndPrintText(string text, string eventID = "", UnityAction confirmAction = null)
    {
        gameObject.SetActive(true);
        printer.StartPrint(text, printedText);

        if (confirmAction != null)
        {
            StartCoroutine(WaitAndConfigureConfirm(confirmAction));
        }

        if (portraitIsReady && spriteData != null)
        {
            if (string.IsNullOrEmpty(eventID))
            {
                portrait.sprite = spriteData.GetDefaultFrame();
            }
            else
            {
                SetExpression(eventID);
            }
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
