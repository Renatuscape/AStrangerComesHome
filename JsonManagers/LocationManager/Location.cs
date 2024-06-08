using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LocationType
{
    Stop,
    Outpost,
    Settlement,
    Town,
    City,
    Crossing,
    Hidden
}
[System.Serializable]
public class Location
{
    public string objectID;
    public string name;
    public string otherName;
    public string customSpriteID;
    public LocationType type = LocationType.Stop;
    public bool isHidden = false; // use this for locations without icons, but which can still be visited. Can combine with unlockables.
    public bool noPassengers = false;
    public string description;
    public int mapX;
    public int mapY;
    public List<IdIntPair> requirements = new(); //use this for unlockable locations
    public List<IdIntPair> restrictions = new(); //use this for unlockable locations
    public List<Gate> gates = new();

    public bool CheckIfUnlocked()
    {
        bool passedRequirements = RequirementChecker.CheckRequirements(requirements);
        bool passedRestrictions = RequirementChecker.CheckRestrictions(restrictions);

        if (passedRequirements && passedRestrictions)
        {
            return true;
        }
        return false;
    }
}
public static class Locations
{
    public static List<Location> all = new();

    public static Location FindByCoordinates(string region, int mapX, int mapY, bool excludeLocked)
    {
        var locationList = Regions.FindByID(region).locations;
        var match = locationList.FirstOrDefault(l => l.mapX == mapX && l.mapY == mapY);

        if (excludeLocked && match != null)
        {
            if (match.CheckIfUnlocked())
            {
                Debug.Log("Station passed unlock check: " + match.objectID);
            }
            else
            {
                Debug.Log("Station failed unlock check and returned null.");
                return null;
            }
        }

        return match;
    }

    public static Location FindByID(string objectID)
    {
        return all.FirstOrDefault(l => l.objectID == objectID);
    }
}

[Serializable]
public class Gate
{
    public string objectID;
    public string name;
    public string description;
    public string failText;
    public bool isHiddenBeforeCheck = false; //Does not appear as an option unless checks are cleared 
    public string destinationRegion;
    public float xCoordinate;
    public float yCoordinate;
    public List<IdIntPair> requirements = new();
    public List<IdIntPair> restrictions = new();

    public bool AttemptChecks(out bool passedRequirements, out bool passedRestrictions)
    {
        passedRequirements = true;
        passedRestrictions = true;

        if (requirements is not null && requirements.Count > 0)
        {
            foreach (IdIntPair entry in requirements)
            {
                int amount = Player.GetCount(entry.objectID, "Choice Requirement Check");
                if (amount < entry.amount)
                {
                    passedRequirements = false;
                    break;
                }
            }
        }
        if (restrictions is not null && restrictions.Count > 0) //don't run if checks already failed
        {
            foreach (IdIntPair entry in restrictions)
            {
                int amount = Player.GetCount(entry.objectID, "Choice Restriction Check");
                if (amount >= entry.amount)
                {
                    passedRestrictions = false;
                    break;
                }
            }
        }
        return passedRequirements == true && passedRestrictions == true;
    }
}