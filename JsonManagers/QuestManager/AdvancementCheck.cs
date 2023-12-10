using System.Collections.Generic;

[System.Serializable]
public class AdvancementCheck
{
    public int minimumCharacterLevel;
    public int minimumDaysPassed;

    //The player must have at least this much
    public List<IdIntPair> requirements;

    //The player must have less than this amount
    public List<IdIntPair> restrictions;
}
