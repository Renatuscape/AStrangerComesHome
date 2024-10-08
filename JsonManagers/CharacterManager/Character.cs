using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CharacterType
{
    Arcana,
    Unique,
    Generic
}

[System.Serializable]
public class RestrictedInfo
{
    public string content;
    public RequirementPackage checks;
}

[System.Serializable]
public class Character : BaseObject
{
    public string dialogueTag;
    public string trueName;
    public string hexColour;
    public List<RestrictedInfo> dynamicDescriptions;
    public int giftableLevel;
    public bool excludeFromPrint = false;
    public bool excludeFromDating = false; // for characters that may gain romance points, but should not appear in dating features

    public CharacterType type;
    public Sprite sprite;
    public SpriteCollection spriteCollection;
    public List<Sprite> spriteAnimation;

    public string namePlate;
    public string trueNamePlate;

    public List<Shop> shops = new();
    public bool canAccessGarage;
    public bool canAccessGuild;
    public bool canEditCharacter;
    public List<IdIntPair> editRequirements;
    public List<string> giftsDislike;
    public List<string> giftsLike;
    public List<string> giftsLove;

    public void NameSetup()
    {
        namePlate = $"<color=#{hexColour}>{name}</color>";
        trueNamePlate = $"<color=#{hexColour}>{ trueName}</color>";
    }
    public string NamePlate()
    {
        var nameUnlocked = TransientDataScript.gameManager.dataManager.unlockedNames.FirstOrDefault(n => n == objectID + "-NAME");

        if (nameUnlocked != null)
        {
            return trueNamePlate;
        }
        else
        {
            return namePlate;
        }

    }

    public string ForceTrueNamePlate()
    {
        return trueNamePlate;
    }

    public string PersonaliseText(string text)
    {
        return "<color=#" + hexColour + ">" + text + "</color>";
    }

    public string GetNameOnly()
    {
        var nameUnlocked = TransientDataScript.gameManager.dataManager.unlockedNames.FirstOrDefault(n => n == objectID + "-NAME");

        if (nameUnlocked != null)
        {
            return trueName;
        }
        return name;
    }

    public Shop GetShop(string shopID)
    {
        if (shops.Count > 0)
        {
            if (shops.Count == 1)
            {
                return shops[0];
            }
            else
            {
                return shops.Where(s => s.objectID == shopID).FirstOrDefault();
            }
        }
        else
        {
            Debug.Log($"No shops found.");

            return null;
        }
    }
}

public static class Characters
{
    public static List<Character> all = new();

    public static Character FindByTag(string searchWord, string caller)
    {
        // Debug.Log("Attempting to find tag: " + searchWord);

        Character found = all.Find((s) => s.dialogueTag.ToLower() == searchWord.ToLower());

        if (found is null)
        {
            Debug.LogWarning($"Find by tag returned no known character with name {searchWord}. Caller was {caller}. Check if you are passing an objectID or dialogueTag.");
        }

        // Debug.Log("Found: " + searchWord);
        return found;
    }

    public static Character FindByID(string searchWord)
    {
        if (all.Count == 0)
        {
            Debug.LogWarning("Characters.all was empty. Something called on Characters.FindByID before JSON was loaded.");
            return null;
        }
        if (string.IsNullOrWhiteSpace(searchWord))
        {
            Debug.LogWarning($"Search term was null or white-space. Returned null. Ensure correct ID in calling script.");
            return null;
        }
        foreach (Character c in all)
        {
            if (c.objectID == searchWord)
            {
                return c;
            }
        }
        Debug.LogWarning("No character found with ID containing this search term: " + searchWord + ". Attempting to search by tag.");

        Character character = FindByTag(searchWord, "Characters.FindByID");

        if (character is not null)
        {
            return character;
        }

        return null;
    }
}

[System.Serializable]
public class WalkingLocation
{
    public List<string> locations;
    public float timeStart;
    public float timeEnd;
    public List<int> daysOfWeek;
    public List<IdIntPair> requirements;
    public List<IdIntPair> restrictions;
    public bool isBuying;
    public bool isSelling;

    public bool CheckRequirement()
    {
        // CHECK LOCATION
        if (locations != null && locations.Count > 0)
        {
            bool hasFoundLocation = false;
            foreach (string location in locations)
            {
                if (location == TransientDataScript.transientData.currentLocation.objectID)
                {
                    hasFoundLocation = true;
                }
            }
            if (!hasFoundLocation)
            {
                return false;
            }
        }
        else
        {
            Debug.Log("Walking Location had no actual locations.");
        }

        // CHECK TIME OF DAY
        if (timeStart != timeEnd) // Skip if start and end time is identical, i. e. not set to any meaningful value
        {
            var timeOfDay = TransientDataScript.GetTimeOfDay();

            if (timeOfDay < timeStart || timeOfDay > timeEnd)
            {
                return false;
            }
        }

        // CHECK DAYS OF WEEK
        if (daysOfWeek != null && daysOfWeek.Count > 0) //if no days are listed, spawn any day
        {
            bool isValidDay = false;
            int dayOfWeek = (int)TransientDataScript.GetWeekDay();
            foreach (int day in daysOfWeek)
            {
                if (day == dayOfWeek)
                {
                    isValidDay = true;
                }
            }

            if (!isValidDay)
            {
                return false;
            }
        }

        // CHECK REQUIREMENTS
        if (requirements != null && requirements.Count > 0)
        {
            foreach (var requirement in requirements)
            {
                if (Player.GetCount(requirement.objectID, "Walking Location Check") < requirement.amount)
                {
                    return false;
                }
            }
        }

        // CHECK RESTRICTIONS
        if (restrictions != null && restrictions.Count > 0)
        {
            foreach (var restriction in restrictions)
            {
                if (Player.GetCount(restriction.objectID, "Walking Location Check") > restriction.amount)
                {
                    return false;
                }
            }
        }

        return true;
    }
}