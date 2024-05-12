using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdvancementCheck
{
    public int minimumDaysPassed;

    //The player must have at least this much
    public List<IdIntPair> requirements;

    //The player must have less than this amount
    public List<IdIntPair> restrictions;

    public bool CheckRequirements(out int minDays)
    {
        minDays = minimumDaysPassed;

        if (requirements != null && requirements.Count > 0)
        {
            Debug.Log($"Checking requirements.");

            foreach (IdIntPair requirement in requirements)
            {
                int amount = Player.GetCount(requirement.objectID, "Choice Requirement Check");

                Debug.Log($"{requirement.amount} {requirement.objectID} is required. Player has {amount}");

                if (amount < requirement.amount)
                {
                    return false;
                }

                Debug.Log("Returned true.");
            }
        }

        if (restrictions != null && restrictions.Count > 0)
        {
            foreach (IdIntPair restriction in restrictions)
            {
                int amount = Player.GetCount(restriction.objectID, "Choice Restriction Check");
                if (amount >= restriction.amount)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
