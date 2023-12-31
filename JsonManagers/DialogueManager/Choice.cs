using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice
{
    public bool endConversation = true;
    public bool doesNotAdvance = false;
    public int advanceTo; //set to 100 to complete quest
    public string optionText;
    public string successSpeaker;
    public string successText;
    public string failureSpeaker;
    public string failureText;
    public List<IdIntPair> deliveryRequirements;
    public List<IdIntPair> checkRequirements;
    public List<IdIntPair> checkRestrictions;
    public List<IdIntPair> rewards;

    public bool AttemptAllChecks(bool grantRewards, out bool passedRequirements, out bool passedRestrictions, out List<IdIntPair> missingItems)
    {
        bool checks = AttemptChecks(out passedRequirements, out passedRestrictions);
        bool delivery = AttemptDelivery(out missingItems);

        if (checks && delivery && grantRewards)
        {
            GrantRewards();
        }
        return checks && delivery;
    }

    public bool AttemptChecks(out bool passedRequirements, out bool passedRestrictions)
    {
        passedRequirements = true;
        passedRestrictions = true;

        if (checkRequirements is not null && checkRequirements.Count > 0)
        {
            foreach (IdIntPair entry in checkRequirements)
            {
                int amount = Player.GetCount(entry.objectID, "Choice Requirement Check");
                if (amount < entry.amount)
                {
                    passedRequirements = false;
                    break;
                }
            }
        }
        if (checkRestrictions is not null && checkRestrictions.Count > 0) //don't run if checks already failed
        {
            foreach (IdIntPair entry in checkRestrictions)
            {
                int amount = Player.GetCount(entry.objectID, "Choice Restriction Check");
                if (amount >= entry.amount)
                {
                    passedRestrictions = false;
                    break;
                }
            }
        }
        return passedRequirements == true && passedRestrictions == true;
    }

    public bool AttemptDelivery(out List<IdIntPair> missingItems)
    {
        bool successfulDelivery = true;
        missingItems = new();

        foreach (IdIntPair entry in deliveryRequirements)
        {
            int amount = Player.GetCount(entry.objectID, "Choice Delivery Check");
            if (amount < entry.amount)
            {
                IdIntPair missingItem = new() { objectID = entry.objectID, amount = entry.amount - amount };
                missingItems.Add(missingItem);
                successfulDelivery = false;
            }
        }

        if (successfulDelivery == true)
        {
            foreach(IdIntPair entry in deliveryRequirements)
            {
                Player.Remove(entry);
            }
        }
        return successfulDelivery;
    }

    public void GrantRewards()
    {
        Debug.Log($"Delivering reward for successful choice: {optionText}");
        foreach (IdIntPair entry in rewards)
        {
            Debug.Log($"Delivered {entry.objectID} ({entry.amount})");
            Player.Add(entry.objectID, entry.amount);
        }
    }
}