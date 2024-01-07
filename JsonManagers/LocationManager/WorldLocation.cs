using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LocationType
{
    Stop,
    Outpost,
    Settlement,
    Town,
    City,
    Crossing,
    Temporary
}
[System.Serializable]
public class WorldLocation
{
    public string objectID;
    public string name;
    public string otherName;
    public LocationType type = LocationType.Stop;
    public bool isHidden = false;
    public string description;
    public int mapX;
    public int mapY;
    public List<IdIntPair> requirements = new();
    public List<IdIntPair> restrictions = new();
    public Texture2D backgroundTexture = null;
    public Sprite backgroundSprite = null;
    public List<PointOfInterest> pointsOfInterest = new();
}
public static class Locations
{
    public static List<WorldLocation> all = new();
}