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
public class WorldLocation
{
    public string objectID;
    public string name;
    public string otherName;
    public LocationType type;
    public bool isHidden = false;
    public string description;
    public decimal[] mapLocation;
    public List<IdIntPair> requirements = new();
    public List<IdIntPair> restrictions = new();
    public Texture2D backgroundTexture;
    public Sprite backgroundSprite;
    public List<PointOfInterest> pointsOfInterest;
}
