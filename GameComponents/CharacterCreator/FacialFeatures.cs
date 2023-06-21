using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FacialFeatures : MonoBehaviour
{
    public DataManagerScript dataManager;
    public GameObject featureContainer;
    public List<Toggle> toggleList;

    private void Awake()
    {
        //check if the elements exist
        if (dataManager.faceMods == null || dataManager.faceMods.Count == 0)
        {
            Debug.Log("No entries found in faceMods dictionary. Populating dictionary.");

            foreach (Transform child in featureContainer.transform)
            {
                if (child.name != "TargetText")
                {
                    string cName = child.name;
                    bool isActive = false;

                    if (dataManager.faceMods.ContainsKey(cName))
                        dataManager.faceMods[cName] = isActive;
                    else
                        dataManager.faceMods.Add(cName, isActive);

                    toggleList.Add(child.GetComponentInChildren<Toggle>());
                }
            }
        }

    }
    private void OnEnable()
    {
        UpdateToggleElements();

    }
    
    void UpdateToggleElements()
    {
        List<Toggle> togglesToUpdate = new List<Toggle>();

        foreach (KeyValuePair<string, bool> kvp in dataManager.faceMods)
        {
            string cName = kvp.Key;
            bool isActive = kvp.Value;

            // Get the toggle object by name
            foreach (Toggle t in toggleList)
            {
                if (t.gameObject.name == cName + "_Toggle" && t.isOn != dataManager.faceMods[cName])
                {
                    togglesToUpdate.Add(t);
                    break;
                }
            }
        }

        foreach (Toggle t in togglesToUpdate) //updating the toggles after iterating over the dictionary avoids errors
        {
            t.isOn = !t.isOn;
        }
    }

    public void StoreFeatureToggle(GameObject mod)
    {
        var cName = mod.name;
        Toggle toggle = null;

        foreach (Toggle t in toggleList)
        {
            if (t.gameObject.name == cName + "_Toggle")
            {
                toggle = t;
                break;
            }
        }

        if (toggle != null)
        {
            if (dataManager.faceMods.ContainsKey(cName))
                dataManager.faceMods[cName] = toggle.isOn;
            else
                dataManager.faceMods.Add(cName, toggle.isOn);

            mod.SetActive(toggle.isOn);
        }
    }

    // Retrieve the child object data from the dictionary
    public void RetrieveFeatureToggleData()
    {
        if (dataManager.faceMods == null || dataManager.faceMods.Count == 0)
        {
            Debug.Log("No entries found in faceMods dictionary.");
        }
        else
        {
            foreach (KeyValuePair<string, bool> pair in dataManager.faceMods)
            {
                string cName = pair.Key;
                bool isActive = pair.Value;

                // Retrieve the child object using the objectName
                Transform childTransform = featureContainer.transform.Find(cName);
                GameObject childObject = childTransform?.gameObject;

                if (childObject != null)
                {
                    // Set the isActive state of the child object
                    childObject.SetActive(isActive);
                }
            }
        }

    }
}
