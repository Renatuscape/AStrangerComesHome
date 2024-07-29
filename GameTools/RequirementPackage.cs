using System.Collections.Generic;

[System.Serializable]
public class RequirementPackage
{
    public float minTimeOfDay;
    public float maxTimeOfDay;
    public int requiredDaysPassed;
    public List<int> weekDay;
    public string locationID; // requiredLocation;
    public List<IdIntPair> restrictions;
    public List<IdIntPair> requirements;
    public List<IdIntOr> conditions;
}

[System.Serializable]
public class IdIntCondition
{
    public string objectID;
    public int minAmount = -99999;
    public int maxAmount = 99999;

    public bool Check()
    {
        var count = Player.GetCount(objectID, "IdIntCondition");

        if (count < minAmount && count > maxAmount)
        {
            return false;
        }
        return true;
    }
}

[System.Serializable]
public class IdIntOr
{
    public List<IdIntCondition> tests;
    public int minPasses = 1;
    public int maxPasses = 99;

    public bool Check()
    {
        int checksPassed = 0;

        foreach (var test in tests)
        {
            if (test.Check())
            {
                checksPassed++;
            }
        }

        if (checksPassed < minPasses || checksPassed > maxPasses)
        {
            return false;
        }
        return true;
    }
}