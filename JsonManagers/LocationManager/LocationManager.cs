using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LocationManager : MonoBehaviour
{
    public List<WorldLocation> debugLocationList = Locations.all;
    public List<WorldRegion> debugRegionList = Regions.all;
    public bool allObjecctsLoaded = false;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad = 1;

    void Start()
    {
        LoadFromJson("Locations.json");
        //Remember to update numberOfFilesToLoad if more files are added
    }

    [System.Serializable]
    public class LocationWrapper //Necessary for Unity to read the .json contents as an object
    {
        public WorldLocation[] locations;
    }
    public void LoadFromJson(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Locations/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = File.ReadAllText(jsonPath);
            LocationWrapper dataWrapper = JsonUtility.FromJson<LocationWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.locations != null)
                {
                    foreach (WorldLocation location in dataWrapper.locations)
                    {
                        InitialiseLocation(location);
                    }
                    filesLoaded++;
                    if (filesLoaded == numberOfFilesToLoad)
                    {
                        allObjecctsLoaded = true;
                        Debug.Log("All LOCATIONS successfully loaded from Json.");
                    }
                }
                else
                {
                    Debug.LogError("Location array is null in JSON data. Check that the list has a wrapper with the \'locations\' tag and that the object class is tagged serializable.");
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

    public static void InitialiseLocation(WorldLocation location)
    {
        objectIDReader(ref location);
        Locations.all.Add(location);
    }

    public static void objectIDReader(ref WorldLocation location)
    {

        //SET REGION
        if (int.TryParse(location.objectID.Substring(1, 1), out var i))
        {
            var region = Regions.FindByID("REGION"+i);
            if (region is null)
            {
                region = new() {rows = 15, columns = 20 };
                region.objectID = "REGION" + i;
                Regions.all.Add(region);
            }
            region.locations.Add(location);
        }

        //SET TYPE
        string type = location.objectID.Substring(9, 4);
        if (type == "CITY")
        {
            location.type = LocationType.City;
        }
        else if (type == "TOWN")
        {
            location.type = LocationType.Town;
        }
        else if (type == "SETL")
        {
            location.type = LocationType.Settlement;
        }
        else if (type == "OUTP")
        {
            location.type = LocationType.Outpost;
        }
        else if (type == "STOP")
        {
            location.type = LocationType.Stop;
        }
        else if (type == "TEMP")
        {
            location.type = LocationType.Temporary;
        }
        else if (type == "CROSS")
        {
            location.type = LocationType.Crossing;
        }
        else
        {
            Debug.Log($"Location tag ({type}) not recognised at {location.objectID}");
        }
    }
}
