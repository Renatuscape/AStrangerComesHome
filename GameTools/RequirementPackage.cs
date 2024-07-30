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
    public string gender;
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
    public List<IdIntCondition> testRange = new();
    public List<IdIntPair> testExact = new();
    public int minPasses = 1;
    public int maxPasses = 99;

    public bool Check()
    {
        int checksPassed = 0;

        foreach (var test in testRange)
        {
            if (test.Check())
            {
                checksPassed++;
            }
        }

        foreach (var test in testExact)
        {
            if (Player.GetCount(test.objectID, "IdIntOr") == test.amount)
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