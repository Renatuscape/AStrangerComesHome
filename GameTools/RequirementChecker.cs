using System.Collections.Generic;
using UnityEngine;

public static class RequirementChecker
{

    public static bool CheckDialogueRequirements(Dialogue dialogue)
    {
        bool timeCheck = CheckTime(dialogue.startTime, dialogue.endTime);
        bool locationCheck = CheckLocation(dialogue.location);
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
        var currentLocation = TransientDataCalls.GetCurrentLocation();
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
            float currentTime = TransientDataCalls.GetTimeOfDay();

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
        DayOfWeek currentWeekday = TransientDataCalls.GetWeekDay();

        if ((int) currentWeekday == weekDay)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool CheckLocation(string objectID)
    {
        Location requiredLocation = Locations.FindByID(objectID);

        if (requiredLocation == null)
        {
            Debug.LogWarning("Location check returned true because required location returned null.");
            return true;
        }
        else
        {
            if (TransientDataCalls.GetCurrentLocation() == requiredLocation)
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
}