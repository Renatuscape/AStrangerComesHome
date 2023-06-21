using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CharacterPronouns : MonoBehaviour
{
    public DataManagerScript dataManager;
    public TMP_Dropdown dropdown;
    public GameObject customFields;

    public TextMeshProUGUI pronounSub;
    public TextMeshProUGUI pronounObj;
    public TextMeshProUGUI pronounGen;
    private void Awake()
    {
        customFields.SetActive(false);
        dataManager.pronounSub = pronounSub.text;
        dataManager.pronounObj = pronounObj.text;
        dataManager.pronounGen = pronounGen.text;
    }
    public void ChoosePronouns()
    {
        customFields.SetActive(false);
        if (dropdown.value > 5 || dropdown.value < 0)
            dropdown.value = 0;

        if (dropdown.value == 0)
        {
            pronounSub.text = "he";
            pronounObj.text = "him";
            pronounGen.text = "his";
        }

        else if (dropdown.value == 1)
        {
            pronounSub.text = "she";
            pronounObj.text = "her";
            pronounGen.text = "her";
        }
        else if (dropdown.value == 2)
        {
            pronounSub.text = "they";
            pronounObj.text = "them";
            pronounGen.text = "their";
        }
        else if (dropdown.value == 3)
        {
            pronounSub.text = "it";
            pronounObj.text = "it";
            pronounGen.text = "its";
        }
        else if (dropdown.value == 4)
        {
            customFields.SetActive(true);
        }

        UpdatePronouns();
    }

    public void UpdatePronouns()
    {
        StartCoroutine(LatePronounUpdate());
    }

    IEnumerator LatePronounUpdate()
    {
        yield return new WaitForSeconds(0.1f);
        dataManager.pronounSub = pronounSub.text;
        dataManager.pronounObj = pronounObj.text;
        dataManager.pronounGen = pronounGen.text;
    }
}
