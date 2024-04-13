using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RegionManager : MonoBehaviour
{
    public List<Region> debugRegionList = Regions.all;
    public bool allObjecctsLoaded = false;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad = 1;

    public void StartLoading()
    {
        gameObject.SetActive(true);
        LoadFromJson("Regions.json");
        //Remember to update numberOfFilesToLoad if more files are added
    }

    [System.Serializable]
    public class RegionWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Region[] regions;
    }
    public void LoadFromJson(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Locations/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = File.ReadAllText(jsonPath);
            RegionWrapper dataWrapper = JsonUtility.FromJson<RegionWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.regions != null)
                {
                    foreach (Region region in dataWrapper.regions)
                    {
                        Initialise(region);
                    }
                    filesLoaded++;
                    if (filesLoaded == numberOfFilesToLoad)
                    {
                        allObjecctsLoaded = true;
                        Debug.Log("All REGIONS successfully loaded from Json.");
                    }
                }
                else
                {
                    Debug.LogError("Region array is null in JSON data. Check that the list has a wrapper with the \'regions\' tag and that the object class is tagged serializable.");
                }
            }
            else
            {
                Debug.LogError("JSON data is malformed. No wrapper found?");
                Debug.Log(jsonData); // Log the JSON data for inspection
            }
        }
        else
        {
            Debug.LogError("JSON file not found: " + jsonPath);
        }
    }

    public static void Initialise(Region region)
    {
        objectIDReader(ref region);
        Regions.all.Add(region);
    }

    public static void objectIDReader(ref Region region)
    {
    }
}
