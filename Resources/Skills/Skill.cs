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
[CreateAssetMenu(fileName = "New Skill", menuName = "Scriptable Object/Skill")]
public class Skill : MotherObject
{
    public SkillType skillType;

    void Awake()
    {
        objectType = ObjectType.Skill;
    }
}
