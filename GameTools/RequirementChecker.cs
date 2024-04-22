using System.Collections.Generic;
using UnityEngine;

public static class RequirementChecker
{
    public static bool CheckDialogueRequirements(Dialogue dialogue)
    {
        bool timeCheck = CheckTime(dialogue.startTime, dialogue.endTime);
        bool locationCheck = CheckAgainstCurrentLocation(dialogue.locationID);
        bool requirementCheck = CheckRequirements(dialogue.requirements);
        bool restrictionCheck = CheckRestrictions(dialogue.restrictions);

        if (timeCheck && locationCheck && requirementCheck && restrictionCheck)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CheckWalkingRequirements(Character character)
    {
        var currentLocation = TransientDataScript.GetCurrentLocation();
        bool validLocation = false;
        bool validTime = false;
        bool validDay = false;

        foreach (WalkingLocation walkingLocation in character.walkingLocations)
        {
            foreach (string locationID in walkingLocation.locations)
            {
                if (locationID == currentLocation.objectID)
                {
                    validLocation = true;
                    break;
                }
            }

            if (CheckTime(walkingLocation.timeStart, walkingLocation.timeEnd))
            {
                validTime = true;
            }

            foreach (int day in walkingLocation.daysOfWeek)
            {
                if (CheckWeekDay(day))
                {
                    validDay = true;
                    break;
                }
            }

            if (validDay && validLocation && validTime)
            {
                return true;
            }
            else
            {
                validDay = false;
                validLocation = false;
                validTime = false;
            }
        }

        return false;
    }

    public static bool CheckTime(float startTime, float endTime)
    {
        if (startTime != endTime)
        {
            float currentTime = TransientDataScript.GetTimeOfDay();

            if (currentTime > endTime)
            {
                return false;
            }
            else if (currentTime < startTime)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }

    }

    public static bool CheckWeekDay(int weekDay)
    {
        DayOfWeek currentWeekday = TransientDataScript.GetWeekDay();

        if ((int)currentWeekday == weekDay)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CheckAgainstCurrentLocation(string objectID)
    {
        Location requiredLocation = Locations.FindByID(objectID);

        return CheckAgainstCurrentLocation(requiredLocation);
    }
    public static bool CheckAgainstCurrentLocation(Location requiredLocation)
    {
        if (requiredLocation == null)
        {
            //Debug.LogWarning("Location check returned true because required location returned null.");
            return true;
        }
        else
        {
            if (TransientDataScript.GetCurrentLocation() == requiredLocation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public static bool CheckRequirements(List<IdIntPair> requirements)
    {
        if (requirements != null && requirements.Count > 0)
        {
            foreach (IdIntPair requirement in requirements)
            {
                int amount = Player.GetCount(requirement.objectID, "Choice Requirement Check");

                if (amount < requirement.amount)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool CheckRestrictions(List<IdIntPair> restrictions)
    {
        if (restrictions != null && restrictions.Count > 0) //don't run if checks already failed
        {
            foreach (IdIntPair restriction in restrictions)
            {
                int amount = Player.GetCount(restriction.objectID, "Choice Restriction Check");

                if (amount > restriction.amount)
                {
                    return false;
                }
            }
            Debug.Log("Returned true.");
        }

        return true;
    }

    public static bool CheckPackage(RequirementPackage package)
    {
        if (!CheckRequirements(package.requirements))
        {
            return false;
        }
        else if (!CheckRestrictions(package.requirements))
        {
            return false;
        }
        else if (!CheckTime(package.minTimeOfDay, package.maxTimeOfDay))
        {
            return false;
        }
        else if (!CheckAgainstCurrentLocation(package.requiredLocation))
        {
            return false;
        }
        else if (TransientDataScript.gameManager.dataManager.totalGameDays < package.requiredDaysPassed)
        {
            return false;
        }
        else
        {
            if (package.weekDay != null)
            {
                foreach (int day in package.weekDay)
                {
                    if (!CheckWeekDay(day))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}

[System.Serializable]
public class RequirementPackage
{
    public float minTimeOfDay;
    public float maxTimeOfDay;
    public int requiredDaysPassed;
    public List<int> weekDay;
    public string requiredLocation;
    public List<IdIntPair> restrictions;
    public List<IdIntPair> requirements;
}