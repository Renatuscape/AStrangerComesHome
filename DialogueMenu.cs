using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMenu : MonoBehaviour
{
    public RectMask2D containerMask;
    public GameObject containerReplacer;
    public GameObject dialogueContainer;
    public PortraitRenderer portraitRenderer; //remember to use .gameObject for the object
    public Image dialogueContainerBG;

    private void Awake()
    {
        dialogueContainerBG = dialogueContainer.GetComponent<Image>();
    }

    private void OnEnable()
    {
        SetValuesToDefault();
    }

    private void OnDisable()
    {
        SetValuesToDefault();
    }
    void Update()
    {
        if (dialogueContainer.GetComponent<RectTransform>().rect.height > 389 && containerReplacer.activeInHierarchy == false)
        {
            EnableReplacerBG();
        }
        else if (dialogueContainer.GetComponent<RectTransform>().rect.height < 389 && containerReplacer.activeInHierarchy == true)
        {
            SetValuesToDefault();
        }
    }

    void SetValuesToDefault()
    {
        containerReplacer.SetActive(false);
        dialogueContainerBG.enabled = true;
        containerMask.enabled = false;
    }

    void EnableReplacerBG()
    {
        containerReplacer.SetActive(true);
        dialogueContainerBG.enabled = false;
        containerMask.enabled = true;
    }
}
