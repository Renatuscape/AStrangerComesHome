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
    public int maxValue = 1;

    public bool isSeller;
    public bool isBuyer;
    public bool isQuestGiver;

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
}

public static class Characters
{
    public static List<Character> all = new();

    public static Character FindByID(string searchWord)
    {
        if (string.IsNullOrWhiteSpace(searchWord))
        {
            Debug.LogWarning($"Search term was null or white-space. Returned null. Ensure correct ID in calling script.");
            return null;
        }
        foreach (Character character in all)
        {
            if (character.objectID.Contains(searchWord))
            {
                return character;
            }
        }
        Debug.LogWarning("No character found with ID containing this search term: " + searchWord);
        return null;
    }
}
