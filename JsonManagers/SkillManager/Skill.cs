using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Attribute,
    Gardening,
    Alchemy,
    Magic
}

[System.Serializable]
public class Skill
{
    public string objectID;
    public string name;
    public int basePrice; //automatically calculated from type, rarity and ID
    public int maxLevel = 10;
    public SkillType type; //retrieve from ID
    public Texture2D image; //retrieve from ID + folder
    public Sprite sprite;
    public string description;

    public void AddToPlayer(int amount = 1)
    {
        Player.AddDynamicObject(this, amount, "Skill");
    }
    public int GetCountPlayer()
    {
        return Player.GetCount(objectID, "Skill");
    }

}

public static class Skills
{
    public static List<Skill> all = new();

    public static void DebugList()
    {
        Debug.LogWarning("Skills.DebugList() called");

        foreach (Skill skill in all)
        {
            Debug.Log($"Skill ID: {skill.objectID}\tSkill Name: {skill.name}");
        }
    }

    public static void DebugAllSkills()
    {
        foreach (Skill skill in all)
        {
            skill.AddToPlayer(10);
            skill.AddToPlayer(25);
            skill.AddToPlayer(100);
        }
    }

    public static Skill FindByID(string searchWord)
    {
        if (string.IsNullOrWhiteSpace(searchWord))
        {
            Debug.LogWarning("Search term was empty, returned null. Ensure correct ID in calling script.");
            return null;
        }
        foreach (Skill skill in all)
        {
            if (skill.objectID.Contains(searchWord))
            {
                return skill;
            }
        }
        //Debug.LogWarning("No skill found with ID contianing this search term: " + searchWord);
        return null;
    }

    public static int GetMax(string searchWord)
    {
        foreach (Skill skill in all)
        {
            if (skill.objectID == searchWord)
            {
                return skill.maxLevel;
            }
        }
        return 10;
    }
}
