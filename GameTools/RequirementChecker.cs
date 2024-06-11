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
        Debug.Log("Current day: " + (int)currentWeekday + " " + currentWeekday);

        if ((int)currentWeekday == weekDay)
        {
            Debug.Log("Check against " + weekDay + (DayOfWeek)weekDay + " returned true.");
            return true;
        }
        else
        {
            Debug.Log("Check against " + weekDay + " returned false.");
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

    public static bool CheckMinMax(IdMinMax objectCheck)
    {
        int inventoryAmount = Player.GetCount(objectCheck.objectID, "RequirementChecker");

        if (inventoryAmount < objectCheck.minValue || inventoryAmount > objectCheck.maxValue)
        {
            return false;
        }

        return true;
    }

    public static bool CheckPackage(RequirementPackage package)
    {
        if (!CheckRequirements(package.requirements))
        {
            Debug.Log("Package failed at requirements.");
            return false;
        }
        else if (!CheckRestrictions(package.requirements))
        {
            Debug.Log("Package failed at restrictions.");
            return false;
        }
        else if (!CheckTime(package.minTimeOfDay, package.maxTimeOfDay))
        {
            Debug.Log("Package failed at time of day.");
            return false;
        }
        else if (!CheckAgainstCurrentLocation(package.requiredLocation))
        {
            Debug.Log("Package failed at location check.");
            return false;
        }
        else if (TransientDataScript.gameManager.dataManager.totalGameDays < package.requiredDaysPassed)
        {
            Debug.Log("Package failed at required days passed.");
            return false;
        }
        else if (package.minMaxRequirements != null && package.minMaxRequirements.Count > 0)
        {
            foreach (var entry in package.minMaxRequirements)
            {
                if (!CheckMinMax(entry))
                {
                    Debug.Log("Package failed at minMax check.");
                    return false;
                }
            }
        }
        else
        {
            if (package.weekDay != null && package.weekDay.Count > 0)
            {
                bool matchFound = false;
                foreach (int day in package.weekDay)
                {
                    if (CheckWeekDay(day))
                    {
                        Debug.Log("Found matching day");
                        matchFound = true;
                        break;
                    }
                }

                if (!matchFound)
                {
                    Debug.Log("Package failed at day of week.");
                    return false;
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
    public List<IdMinMax> minMaxRequirements;
}

[System.Serializable]
public class IdMinMax
{
    public string objectID;
    public int minValue;
    public int maxValue;
}