public class BaseObject
{
    public string objectID;
    public string name;
    public string description;
    public int maxValue;
    public ObjectType objectType;
}

public enum ObjectType
{
    Item,
    Upgrade,
    Character,
    Quest,
    Recipe,
    Skill
}