using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public static class DialogueTagParser
{
    public static Dictionary<string, string> tags;

    public static string ParseText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.Log("No text to parse");
            return text;
        }

        if (tags is null || tags.Count == 0)
        {
            Debug.Log("Attempted to parse text before dialogue tags were created.");
            return text;
        }

        foreach (var tagEntry in tags)
        {
            string tag = tagEntry.Key;
            string value = tagEntry.Value;

            // Use Regex to replace all occurrences of the tag
            text = text.Replace(tag, value);
        }

        return text;
    }

    public static void UpdateTags(DataManagerScript dataManager) {
        Debug.Log("Updating Dialogue Tags");
        tags = new();
        CreateTags(dataManager);
    }
    static void CreateTags(DataManagerScript dataManager)
    {
        //ADD PRONOUNS
        tags.Add("|he|", Uncapitalise(dataManager.pronounSub));
        tags.Add("|He|", Capitalise(dataManager.pronounSub));
        tags.Add("|him|", Uncapitalise(dataManager.pronounObj));
        tags.Add("|Him|", Capitalise(dataManager.pronounObj));
        tags.Add("|his|", Uncapitalise(dataManager.pronounGen));
        tags.Add("|His|", Capitalise(dataManager.pronounGen));

        string playerName = Characters.FindByID("ARC000").trueNamePlate ?? dataManager.playerName;
        tags.Add("|Morgan|", Capitalise(playerName));

        //ADD CHARACTERS

        foreach (Character c in Characters.all)
        {
            if (c.type == CharacterType.Arcana)
            {
                AddNewTag(Tag(c.objectID),          c.PersonaliseText(RemovePrefix(c.name)));
                AddNewTag(Tag("The " + c.objectID), c.PersonaliseText(Capitalise(c.name)));
                AddNewTag(Tag("the " + c.objectID), c.PersonaliseText(Uncapitalise(c.name)));

                AddNewTag(Tag(c.dialogueTag),          c.PersonaliseText(RemovePrefix(c.GetNameOnly())));
                AddNewTag(Tag("The " + c.dialogueTag), c.PersonaliseText(Capitalise(c.GetNameOnly())));
                AddNewTag(Tag("the " + c.dialogueTag), c.PersonaliseText(Uncapitalise(c.GetNameOnly())));
            }
            else
            {
                AddNewTag(Tag(c.objectID), c.PersonaliseText(c.GetNameOnly()));
                AddNewTag(Tag(c.dialogueTag), c.PersonaliseText(c.GetNameOnly()));
            }
        }

        //SET CORRECT PLAYER NAME
        if (GlobalSettings.AlwaysTrueNameEverywhere)
        {
            Character player = Characters.FindByTag("Traveller", nameof(This));

            tags["|Traveller|"] = player.PersonaliseText(player.trueName);
            tags["|the Traveller|"] = player.PersonaliseText(player.trueName);
            tags["|The Traveller|"] = player.PersonaliseText(player.trueName);
        }

        Debug.Log($"Tags contain {tags.Count} entries:");
        //foreach (KeyValuePair<string, string> kvp in tags)
        //{
        //    Debug.Log($"{kvp.Key}: {kvp.Value}");
        //}
    }

    static void AddNewTag(string key, string value)
    {
        if (!tags.ContainsKey(key))
        {
            tags.Add(key, value);
        }
        else
        {
            Debug.Log($"Key {key} already exists in tags with value {tags[key]}. Attempted valu was {value}.");
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
            // Remove the prefix
            text = text.Substring(4);
        }
        return text;
    }
}
