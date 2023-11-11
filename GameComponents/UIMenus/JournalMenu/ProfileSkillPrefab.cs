using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileSkillPrefab : MonoBehaviour
{
    public TransientDataScript transientData;
    public ProfileScript profileScript;

    public Skill itemSource;
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

    public void EnableObject(Skill skill, ProfileScript script)
    {
        profileScript = script;
        itemSource = skill;
        valueText.text = $"";

        displayImage.sprite = itemSource.sprite;
        displayShadow.sprite = itemSource.sprite;
        isReady = true;
    }
    public void MouseOverItem()
    {
        if (isReady)
        {
            valueText.text = $"{itemSource.name}";

            itemFrame.SetActive(true);
            profileScript.PrintFloatText($"Level { itemSource.name}");
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
