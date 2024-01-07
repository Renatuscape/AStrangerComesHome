using System.Collections.Generic;
using UnityEngine;

public enum PointOfInterestType
{
    Interior,
    Exterior,
    Character,
    Treasure
}
public class PointOfInterest
{
    public string objectID;
    public string name;
    public string ownerID;
    public Character owner;
    public PointOfInterestType type;
    public List<IdIntPair> requirements;
    public List<IdIntPair> restrictions;
    public List<IdIntPair> content;
    public decimal[] availableHours;
    public List<string> availableDays;
    public Texture2D texture;
    public Sprite sprite;
    public Shop shop;
    public bool isUnique;
    public List<Location> locations;
}