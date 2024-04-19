using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileScript : MonoBehaviour
{
    public DataManagerScript dataManager;
    public GameObject profileSkillPrefab;
    public GameObject skillPrefabContainer;
    public GameObject upgradePrefabContainer;

    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerTime;

    void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
    }

    private void OnEnable()
    {
        playerName.text = "<color=#" + dataManager.playerNameColour + ">" + dataManager.playerName + "</color>";
        playerTime.text =
            $"Days in business: {dataManager.totalGameDays}\n" +
            $"Years in business: {Mathf.FloorToInt(dataManager.totalGameDays/365)}";

        foreach (Skill skill in Skills.all)
        {
            var prefab = Instantiate(profileSkillPrefab);
            prefab.name = skill.name;
            prefab.transform.SetParent(skillPrefabContainer.transform, false);
            prefab.GetComponent<ProfileSkillPrefab>().EnableObject(skill, this);
        }
    }

    private void OnDisable()
    {

        foreach (Transform child in skillPrefabContainer.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in upgradePrefabContainer.transform)
        {
            Destroy(child.gameObject);
        }

        TransientDataScript.DisableFloatText();
    }

    public void PrintFloatText(string text)
    {
        TransientDataScript.PrintFloatText(text);
    }

    public void DisableFloatText()
    {
        TransientDataScript.DisableFloatText();
    }

    public void SkillClick(Skill skill)
    {
        //sometext.text = skill.shortDescription
        //someothertext.text = skill.longDescription
    }
}
