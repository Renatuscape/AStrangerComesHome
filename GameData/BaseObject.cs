public class BaseObject
{
    public string objectID;
    public int maxValue;
    public BaseObjectType objectType;
}

public enum BaseObjectType
{
    Item,
    Upgrade,
    Character,
    Quest,
    Recipe,
    Skill
}