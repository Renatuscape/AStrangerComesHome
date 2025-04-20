using System;
using System.Collections.Generic;
using UnityEngine;

public static class DialogueTagParser
{
    public static Dictionary<string, string> tags = new();

    public static string ParseText(string text)
    {
        // Debug.Log("DTagParser: received " + text);
        if (string.IsNullOrEmpty(text))
        {
            // Debug.Log("DTagParser: No text to parse");
            return text;
        }

        if (tags is null || tags.Count == 0)
        {
            Debug.Log("DTagParser: Attempted to parse text before dialogue tags were created.");
            return text;
        }

        foreach (var tagEntry in tags)
        {
            string tag = tagEntry.Key;
            string value = tagEntry.Value;

            text = text.Replace(tag, value);
        }

        // Debug.Log("DTagParser: returned " + text);
        return text;
    }

    public static void UpdateTags(DataManagerScript dataManager)
    {
        Report.Write("Updating Dialogue Tags");

        UpdatePlayerTags(dataManager);

        foreach (Character c in Characters.all)
        {
            UpdateCharacterTags(c);
        }
    }

    public static void CreateAllTags(DataManagerScript dataManager)
    {
        tags = new();

        //ADD PLAYER
        CreatePlayerTags(dataManager);

        //ADD CHARACTERS

        foreach (Character c in Characters.all)
        {
            CreateCharacterTags(c);
        }

        Debug.Log($"Tags contain {tags.Count} entries:");
    }

    public static void UpdatePlayerTags(DataManagerScript dataManager)
    {
        //Add player pronouns
        tags["|he|"] = Uncapitalise(dataManager.pronounSub);
        tags["|He|"] = Capitalise(dataManager.pronounSub);
        tags["|him|"] = Uncapitalise(dataManager.pronounObj);
        tags["|Him|"] = Capitalise(dataManager.pronounObj);
        tags["|his|"] = Uncapitalise(dataManager.pronounGen);
        tags["|His|"] = Capitalise(dataManager.pronounGen);

        //Add player name
        string playerName = Characters.FindByID("ARC000").trueNamePlate ?? dataManager.playerName;
        tags["|Morgan|"] = Capitalise(playerName);
    }

    public static void CreatePlayerTags(DataManagerScript dataManager)
    {
        if (dataManager == null)
        {
            // Add dummy data
            tags.Add("|Morgan|", "Morgan");
            tags.Add("|he|", "he");
            tags.Add("|He|", "He");
            tags.Add("|him|", "him");
            tags.Add("|Him|", "Him");
            tags.Add("|his|", "his");
            tags.Add("|His|", "His");
        }
        else
        {
            //Add player name
            string playerName = Characters.FindByID("ARC000").trueNamePlate ?? dataManager.playerName;
            tags.Add("|Morgan|", Capitalise(playerName));

            //Add player pronouns
            tags.Add("|he|", Uncapitalise(dataManager.pronounSub));
            tags.Add("|He|", Capitalise(dataManager.pronounSub));
            tags.Add("|him|", Uncapitalise(dataManager.pronounObj));
            tags.Add("|Him|", Capitalise(dataManager.pronounObj));
            tags.Add("|his|", Uncapitalise(dataManager.pronounGen));
            tags.Add("|His|", Capitalise(dataManager.pronounGen));
        }
    }

    public static void CreateCharacterTags(Character c)
    {
        if (c.type == CharacterType.Arcana)
        {
            AddNewTag(Tag(c.objectID), c.PersonaliseText(RemovePrefix(c.name)));
            AddNewTag(Tag("The " + c.objectID), c.PersonaliseText(Capitalise(c.name)));
            AddNewTag(Tag("the " + c.objectID), c.PersonaliseText(Uncapitalise(c.name)));

            AddNewTag(Tag(c.dialogueTag), c.PersonaliseText(RemovePrefix(c.GetNameOnly())));
            AddNewTag(Tag("The " + c.dialogueTag), c.PersonaliseText(Capitalise(c.GetNameOnly())));
            AddNewTag(Tag("the " + c.dialogueTag), c.PersonaliseText(Uncapitalise(c.GetNameOnly())));
        }
        else
        {
            AddNewTag(Tag(c.objectID), c.PersonaliseText(c.GetNameOnly()));
            AddNewTag(Tag(c.dialogueTag), c.PersonaliseText(c.GetNameOnly()));
        }
    }

    public static void UpdateCharacterTags(Character c)
    {
        if (c.type == CharacterType.Arcana)
        {
            tags[c.objectID] = c.PersonaliseText(RemovePrefix(c.name));
            tags["The " + c.objectID] = c.PersonaliseText(Capitalise(c.name));
            tags["the " + c.objectID] = c.PersonaliseText(Uncapitalise(c.name));
            tags[c.dialogueTag] = c.PersonaliseText(RemovePrefix(c.GetNameOnly()));
            tags["The " + c.dialogueTag] = c.PersonaliseText(Capitalise(c.GetNameOnly()));
            tags["the " + c.dialogueTag] = c.PersonaliseText(Uncapitalise(c.GetNameOnly()));
        }
        else
        {
            tags[c.objectID] = c.PersonaliseText(c.GetNameOnly());
            tags[c.dialogueTag] = c.PersonaliseText(c.GetNameOnly());
        }
    }

    static void AddNewTag(string key, string value)
    {
        if (!tags.ContainsKey(key))
        {
            tags.Add(key, value);
        }
        else
        {
            Debug.LogError($"Key {key} already exists in tags with value {tags[key]}. Attempted value was {value}.");
        }
    }
    static string Capitalise(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            text = char.ToUpper(text[0]) + text.Substring(1);
        }
        return text;
    }
    static string Uncapitalise(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            text = char.ToLower(text[0]) + text.Substring(1);
        }
        return text;
    }

    static string Tag(string text)
    {
        return $"|{text}|";
    }

    static string RemovePrefix(string text)
    {
        if (text.StartsWith("The ", StringComparison.OrdinalIgnoreCase) || text.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
        {
            text = text.Substring(4);
        }
        return text;
    }

    public static void DebugTags()
    {
        foreach (KeyValuePair<string, string> kvp in tags)
        {
            Report.Write("Tags entry: " + kvp.Key + ", " + kvp.Value);
        }
    }
}
