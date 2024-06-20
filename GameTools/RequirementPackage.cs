using System.Collections.Generic;

[System.Serializable]
public class RequirementPackage
{
    public float minTimeOfDay;
    public float maxTimeOfDay;
    public int requiredDaysPassed;
    public List<int> weekDay;
    public string requiredLocation;
    public List<IdIntPair> restrictions;
    public List<IdIntPair> requirements;
    public List<IdMinMax> minMaxRequirements;
}
