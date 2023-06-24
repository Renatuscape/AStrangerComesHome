using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileScript : MonoBehaviour
{
    public TransientDataScript transientData;
    public DataManagerScript dataManager;
    public GameObject profileSkillPrefab;
    public GameObject skillPrefabContainer;
    public GameObject upgradePrefabContainer;

    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerTime;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
    }

    private void OnEnable()
    {
        playerName.text = "<color=#" + dataManager.playerNameColour + ">" + dataManager.playerName + "</color>";
        playerTime.text =
            $"Days in business: {dataManager.totalGameDays}\n" +
            $"Years in business: {Mathf.FloorToInt(dataManager.totalGameDays/365)}";

        foreach (MotherObject x in transientData.objectIndex)
        {
            if (x.objectType == ObjectType.Skill)// && x.dataValue > 0) //add this to filter out objects that the player does not own
            {

                var prefab = Instantiate(profileSkillPrefab);
                prefab.name = x.printName;
                prefab.transform.SetParent(skillPrefabContainer.transform, false);
                prefab.GetComponent<ProfileSkillPrefab>().EnableObject(x, this);
            }
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

        transientData.DisableFloatText();
    }

    public void PrintFloatText(string text)
    {
        transientData.PrintFloatText(text);
    }

    public void DisableFloatText()
    {
        transientData.DisableFloatText();
    }

    public void SkillClick(MotherObject skill)
    {
        //sometext.text = skill.shortDescription
        //someothertext.text = skill.longDescription
    }
}
