﻿using System.Collections.Generic;
using UnityEngine;

public static class RequirementChecker
{
    public static bool CheckGender(string gender)
    {
        if (string.IsNullOrEmpty(gender))
        {
            return true;
        }
        else
        {
            return gender == TransientDataScript.gameManager.dataManager.playerGender;
        }
    }

    public static bool CheckDialogueRequirements(Dialogue dialogue)
    {
        return CheckPackage(dialogue.checks);
    }

    public static bool CheckTime(float startTime, float endTime)
    {
        if (startTime != endTime)
        {
            float currentTime = TransientDataScript.GetTimeOfDay();

            if (startTime < endTime) // DAYTIME EVENT
            {
                if (currentTime > startTime && currentTime < endTime)
                {
                    //Debug.Log($"Daytime check returned true. Current time was {currentTime}, which checked out as less than {endTime} and more than {startTime}");
                    return true;
                }
                else
                {
                    //Debug.Log($"Daytime check returned false. Current time ({currentTime}) was less than ({startTime}) OR more than ({endTime}).");
                    return false;
                }
            }
            else // NIGHT TIME EVENT
            {
                if (currentTime < endTime || currentTime > startTime)
                {
                    //Debug.Log($"Nighttime check returned true. Current time was {currentTime}, which checked out as less than {endTime} and more than {startTime}");
                    return true;
                }
                else
                {
                    //Debug.Log($"Nighttime check returned false. Current time ({currentTime}) was less than ({startTime}) AND more than ({endTime}).");
                    return false;
                }
            }
        }
        else
        {
            //Debug.Log($"Time check returned true because {endTime} and {startTime} was the same.");
            return true;
        }

    }

    public static bool CheckWeekDay(int weekDay)
    {
        DayOfWeek currentWeekday = TransientDataScript.GetWeekDay();
        // Debug.Log("Current day: " + (int)currentWeekday + " " + currentWeekday);

        if ((int)currentWeekday == weekDay)
        {
            // Debug.Log("Check against " + weekDay + (DayOfWeek)weekDay + " returned true.");
            return true;
        }
        else
        {
            // Debug.Log("Check against " + weekDay + " returned false.");
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
                // Debug.Log("RChecker: Location returned true");
                return true;
            }
            else
            {
                // Debug.Log("RChecker: Location returned false");
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

    public static bool CheckConditions(List<IdIntOr> conditions)
    {
        if (conditions != null && conditions.Count > 0)
        {
            foreach (var condition in conditions)
            {
                if (!condition.Check())
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool CheckForTag(BaseObject obj, string tag)
    {
        if (obj.tagsArray != null)
        {
            foreach (var t in obj.tagsArray)
            {
                if (t == tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool CheckForTags(BaseObject obj, List<string> tags, int tagsNeeded = 0)
    {
        if (obj.tagsArray != null)
        {
            if (tagsNeeded == 0)
            {
                tagsNeeded = tags.Count;
            }

            int tagsFound = 0;

            foreach (var t in obj.tagsArray)
            {
                if (tags.Contains(t))
                {
                    tagsFound++;
                }

                if (tagsNeeded == tagsFound)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool CheckPackage(RequirementPackage package)
    {
        if (package == null)
        {
            Debug.Log("Requirement package was null. Returning true.");
            return true;
        }

        if (!CheckRequirements(package.requirements))
        {
            // Debug.Log("Package failed at requirements.");
            return false;
        }
        else if (!CheckRestrictions(package.restrictions))
        {
            // Debug.Log("Package failed at restrictions.");
            return false;
        }
        else if (!CheckTime(package.minTimeOfDay, package.maxTimeOfDay))
        {
            // Debug.Log("Package failed at time of day.");
            return false;
        }
        else if (!CheckAgainstCurrentLocation(package.locationID))
        {
            // Debug.Log("Package failed at location check.");
            return false;
        }
        else if (TransientDataScript.gameManager.dataManager.totalGameDays < package.requiredDaysPassed)
        {
            // Debug.Log("Package failed at required days passed.");
            return false;
        }
        else if (!CheckConditions(package.conditions))
        {
            return false;
        }
        else if (!CheckGender(package.gender))
        {
            return false;
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
