using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void AddToPlayer(int amount = 1, bool doNotLog = false)
    {
        Player.AddDynamicObject(this, amount, doNotLog, "Skill");
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
        return all.FirstOrDefault(s => s.objectID == searchWord);
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
