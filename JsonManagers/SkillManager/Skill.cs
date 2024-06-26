using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SkillType
{
    Attunement,
    Gardening,
    Alchemy,
    Magic
}

[System.Serializable]
public class Skill : BaseObject
{
    public int basePrice; //automatically calculated from type, rarity and ID
    public SkillType type; //retrieve from ID
    public Texture2D image; //retrieve from ID + folder
    public Sprite sprite;
    public List<RestrictedInfo> dynamicDescriptions;
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
            Player.Add(skill.objectID, 25, true);
        }
    }

    public static Skill FindByID(string searchWord)
    {
        return all.FirstOrDefault(s => s.objectID == searchWord);
    }

    public static int GetMax(string searchWord)
    {
        foreach (Skill skill in all)
        {
            if (skill.objectID == searchWord)
            {
                return skill.maxValue;
            }
        }
        return 10;
    }
}
