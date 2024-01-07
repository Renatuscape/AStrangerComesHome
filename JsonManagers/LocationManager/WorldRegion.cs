using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WorldRegion
{
    public string objectID; //REGION#
    public string name;
    public int columns;
    public int rows;
    public List<WorldLocation> locations = new();
    public Vector3 defaultStartingPosition = new Vector3(0,0,0);
}

public static class Regions
{
    public static List<WorldRegion> all = new();

    public static WorldRegion FindByID(string searchTerm)
    {
        return all.FirstOrDefault(r => r.objectID == searchTerm);
    }
}
