using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterPersonaliaChoices : MonoBehaviour
{
    public Dictionary<int, List<string>> pronounOptions = new()
    {
        { 0, new(){"He", "Him", "His" } },
        { 1, new(){"She", "Her", "Her" } },
        { 2, new(){"They", "Them", "Their" } },
        { 3, new(){"It", "It", "Its" } },
    };
    public DataManagerScript dataManager;
    public TMP_Dropdown pronounDropdown;
    public TMP_Dropdown genderDropdown;
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

    public void SetDropDownsFromPreset(PlayerPreset preset)
    {
        if (!string.IsNullOrEmpty(preset.pronounSub))
        {
            bool foundPronouns = false;

            foreach (var kvp in pronounOptions)
            {
                if (kvp.Value[0] == preset.pronounSub)
                {
                    pronounDropdown.value = kvp.Key;
                    foundPronouns = true;
                    break;
                }
            }

            if (!foundPronouns)
            {
                pronounDropdown.value = 4;
            }
        }
        else
        {
            pronounDropdown.value = 0;
        }

        ChoosePronouns();
    }

    public void ChoosePronouns()
    {
        if (pronounDropdown.value > 5 || pronounDropdown.value < 0)
        {
            pronounDropdown.value = 0;
        }

        if (pronounDropdown.value == 4)
        {
            customFields.SetActive(true);
        }
        else
        {
            customFields.SetActive(false);
            pronounSub.text = pronounOptions[pronounDropdown.value][0];
            pronounObj.text = pronounOptions[pronounDropdown.value][1];
            pronounGen.text = pronounOptions[pronounDropdown.value][2];

            dataManager.pronounSub = pronounOptions[pronounDropdown.value][0];
            dataManager.pronounObj = pronounOptions[pronounDropdown.value][1];
            dataManager.pronounGen = pronounOptions[pronounDropdown.value][2];

            Debug.Log("Pronoun dropdown value was " + pronounDropdown.value + " and the first dictionary entry was " + pronounOptions[pronounDropdown.value][0]);
        }

        SetGender(pronounDropdown.value);
    }


    public void SetGender(int choice)
    {
        if (choice > 2)
        {
            choice = 2;
        }

        genderDropdown.value = choice;

        ChooseGender();
    }
    public void ChooseGender()
    {
        int choice = genderDropdown.value;

        if (choice == 0)
        {
            dataManager.playerGender = "Male";
        }
        else if (choice == 1)
        {
            dataManager.playerGender = "Female";
        }
        else if (choice == 2)
        {
            dataManager.playerGender = "Other";
        }
        else if (choice == 3)
        {
            dataManager.playerGender = "None";
        }
    }

    public void UpdatePronouns()
    {
        dataManager.pronounSub = pronounSub.text;
        dataManager.pronounObj = pronounObj.text;
        dataManager.pronounGen = pronounGen.text;
    }
}
