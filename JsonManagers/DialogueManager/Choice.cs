using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice
{
    public bool endConversation = false;
    public bool doesNotAdvance = false;
    public bool hiddenOnFail = false;
    public int advanceTo; //set to 100 to complete quest
    public int advanceToOnFailure = -1;
    public string optionText;
    public string successSpeaker;
    public string successText;
    public string failureSpeaker;
    public string failureText;
    public List<IdIntPair> deliveryRequirements;
    public List<IdIntPair> requirements;
    public List<IdIntPair> restrictions;
    public List<IdIntPair> rewards;
    public ChoiceNodeData nodeData;

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
        passedRequirements = RequirementChecker.CheckRequirements(requirements);
        passedRestrictions = RequirementChecker.CheckRestrictions(restrictions);
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

            var foundObject = GameCodex.GetBaseObject(entry.objectID); // ensure any item ID is trimmed properly

            if (foundObject != null)
            {
                Player.Add(new IdIntPair() { objectID = foundObject.objectID, amount = entry.amount, description = entry.description });
            }
            else
            {
                Debug.Log($"{entry.objectID} NOT FOUND IN GAME CODEX. INTENTIONAL?");
                Player.Add(new IdIntPair() { objectID = entry.objectID, amount = entry.amount, description = entry.description });
            }
        }
    }
}

[System.Serializable]
public class ChoiceNodeData
{
    public string nodeID;
    public bool removeSpeakerNode;
}