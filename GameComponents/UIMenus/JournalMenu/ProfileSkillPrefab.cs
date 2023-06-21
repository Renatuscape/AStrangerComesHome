using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileSkillPrefab : MonoBehaviour
{
    public TransientDataScript transientData;
    public ProfileScript profileScript;

    public MotherObject itemSource;
    public bool isReady = false;
    public TextMeshProUGUI valueText;
    public GameObject itemFrame;
    public Image displayImage;
    public Image displayShadow;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        itemFrame.SetActive(false);
    }

    public void EnableObject(MotherObject motherObject, ProfileScript script)
    {
        profileScript = script;
        itemSource = motherObject;
        valueText.text = $"";

        displayImage.sprite = itemSource.sprite;
        displayShadow.sprite = itemSource.sprite;
        isReady = true;
    }
    public void MouseOverItem()
    {
        if (isReady)
        {
            valueText.text = $"{itemSource.printName}";

            itemFrame.SetActive(true);
            profileScript.PrintFloatText($"Level { itemSource.dataValue}");
        }
    }

    public void MouseClickItem()
    {
        if (isReady)
        {
            profileScript.SkillClick(itemSource);
        }
    }

    public void MouseExitItem()
    {
        if (isReady)
        {
            valueText.text = $"";
            itemFrame.SetActive(false);
            profileScript.DisableFloatText();
        }
    }
}
