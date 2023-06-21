using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public TransientDataScript transientData;
    public DataManagerScript dataManager;
    public MenuFloatTextScript floatText;
    public GameObject inventoryItemPrefab;
    public GameObject prefabContainer;
    public GameObject descriptionContainer;

    public TextMeshProUGUI playerGoldDisplay;
    public TextMeshProUGUI itemNameDisplay;
    public TextMeshProUGUI itemDescriptionDisplay;
    public TextMeshProUGUI itemStatsDisplay;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();

        descriptionContainer.SetActive(false);
        itemStatsDisplay.text = "";
    }
    private void OnEnable()
    {
        descriptionContainer.SetActive(false);
        itemStatsDisplay.text = "";
        playerGoldDisplay.text = "Gold: " + dataManager.playerGold;


        foreach (MotherObject x in transientData.objectIndex)
        {

            if (x is Item && x.dataValue > 0) //add this to filter out objects that the player does not own
            {
                var prefab = Instantiate(inventoryItemPrefab);
                prefab.name = x.printName;
                prefab.transform.SetParent(prefabContainer.transform, false);
                prefab.GetComponent<InventoryItemPrefab>().EnableObject(x, this);
            }
        }
    }

    private void OnDisable()
    {
        itemNameDisplay.text = " ";
        itemDescriptionDisplay.text = " ";
        descriptionContainer.SetActive(false);

        foreach (Transform child in prefabContainer.transform)
        {
            Destroy(child.gameObject);
        }

        floatText.DisableFloatText();
    }

    public void DisplayItemInfo(string name, string description)
    {
        itemNameDisplay.text = name;
        if (description != " ")
        {
            descriptionContainer.SetActive(true);
            if (description == null || description == "")
                itemDescriptionDisplay.text = "Description missing.";
            else
                itemDescriptionDisplay.text = description;
        }
        else
            descriptionContainer.SetActive(false);
    }

    public void DisplayItemStats(string text)
    {
        itemStatsDisplay.text = text;
    }

    public void PrintFloatText(string text)
    {
        floatText.PrintFloatText(text);
    }

    public void DisableFloatText()
    {
        floatText.DisableFloatText();
    }
}
