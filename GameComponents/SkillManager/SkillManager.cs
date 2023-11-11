using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using static UnityEditor.Progress;
using UnityEditor;
public enum SkillType
{
    Attribute,
    Gardening,
    Alchemy,
    Magic
}

public static class Skills
{
    public static List<Skill> allSkills = new();

    public static void DebugList()
    {
        Debug.LogWarning("Skills.DebugList() called");

        foreach (Skill skill in allSkills)
        {
            Debug.Log($"Skill ID: {skill.objectID}\tSkill Name: {skill.name}");
        }
    }

    public static Skill FindByID(string searchWord)
    {
        foreach (Skill skill in allSkills)
        {
            if (skill.objectID.Contains(searchWord))
            {
                return skill;
            }
        }
        return null;
    }
}
public class Skill: IRewardable
{
    public string objectID;
    public string name;
    public SkillType type;
    public int maxLevel = 10;
    public string description;
    public Texture2D image; //retrieve from ID + folder
    public Sprite sprite;

    public void AddToPlayer(int amount = 1)
    {
        if (amount > 0)
        {
            Player.LevelUp(objectID, amount);
        }
    }
}

public class SkillManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
