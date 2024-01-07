using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    Arcana,
    Unique,
    Generic
}

[System.Serializable]
public class Character
{
    public string objectID;
    public string dialogueTag;
    public string name;
    public string trueName;
    public string hexColour;
    public string description;
    public int maxValue = 99;

    public CharacterType type;
    public Texture2D image;
    public List<Texture2D> imageAnimation;
    public Sprite sprite;
    public List<Sprite> spriteAnimation;

    public string namePlate;
    public string trueNamePlate;

    public void NameSetup()
    {
        namePlate = "<color=#" + hexColour + ">" + name + "</color>";
        trueNamePlate = "<color=#" + hexColour + ">" + trueName + "</color>";
    }
    public string NamePlate()
    {
        if (Player.GetCount($"{objectID}-QTN", objectID + " NamePlate()") >= 50)
        {
            return trueNamePlate;
        }
        return namePlate;
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
        if (Player.GetCount($"{objectID}-QTN", objectID + " NamePlate()") >= 50)
        {
            return trueName;
        }
        return name;
    }
}

public static class Characters
{
    public static List<Character> all = new();

    public static Character FindByTag(string searchWord, string caller)
    {
        Character found = all.Find((s) => s.dialogueTag.ToLower() == searchWord.ToLower());

        if (found is null)
        {
            Debug.Log($"Find by tag returned no known character with name {searchWord}. Caller was {caller}. Check if you are passing an objectID or dialogueTag.");
        }
        return found;
    }

    public static Character FindByID(string searchWord)
    {
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
