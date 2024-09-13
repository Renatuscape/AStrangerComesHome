using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Region
{
    public string objectID; //REGION#
    public string name;
    public string themeDay;
    public string themeEvening;
    public string themeNight;
    public bool disablePassengers;
    public int columns;
    public int rows;
    public List<AutoMapData> mapLayout = new();
    public List<Location> locations = new();
    public Vector3 defaultStartingPosition = new Vector3(0,0,0);
    public float salvagePosition;
}

public static class Regions
{
    public static List<Region> all = new();

    public static Region FindByID(string searchTerm)
    {
        return all.FirstOrDefault(r => r.objectID == searchTerm);
    }
}

[Serializable]
public class AutoMapData
{
    public List<string> row;
}