using System;
using System.Linq;

public class BaseObject
{
    public string objectID;
    public string name;
    public string description;
    public string tags;
    public string[] tagsArray;
    public int maxValue;
    public ObjectType objectType;

    public void SetupTags()
    {
        if (tags != null && tags.Length > 0)
        {
            tagsArray = tags.Split(new[] { ", " }, StringSplitOptions.None);
        }
    }
}

public enum ObjectType
{
    Item,
    Upgrade,
    Character,
    Quest,
    Recipe,
    Skill,
    Memory
}