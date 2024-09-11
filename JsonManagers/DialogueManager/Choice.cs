using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice
{
    public bool endConversation = false;
    public bool endConversationOnFailure = true;
    public bool hiddenOnFail = false;
    public bool dieOnFailure = false;
    public bool dieOnSuccess = false;
    public int advanceTo; //set to 100 to complete quest
    public int advanceToOnFailure = -1;
    public string optionText;
    public string successSpeaker;
    public string successText;
    public string failureSpeaker;
    public string failureText;
    public Gate gate;
    public bool gateOnFailOnly = false;
    public bool gateOnFailAndSuccess = false;

    public RequirementPackage checks;
    //public List<IdIntPair> requirements;
    //public List<IdIntPair> restrictions;

    public List<IdIntPair> deliveries; // deliveryRequirements;
    public List<IdIntPair> rewards;

    public ChoiceNodeData nodeData;
    public DialogueEvent successEvent;
    public DialogueEvent failureEvent;

    public bool AttemptAllChecks(bool grantRewards, out List<IdIntPair> missingItems)
    {
        bool testResults = RequirementChecker.CheckPackage(checks);
        bool delivery = AttemptDelivery(out missingItems);

        if (testResults && delivery && grantRewards)
        {
            GrantRewards();
        }
        return testResults && delivery;
    }

    public bool AttemptDelivery(out List<IdIntPair> missingItems)
    {
        bool successfulDelivery = true;
        missingItems = new();

        foreach (IdIntPair entry in deliveries)
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
            foreach(IdIntPair entry in deliveries)
            {
                Player.Remove(entry);
            }
        }
        return successfulDelivery;
    }

    public void GrantRewards()
    {
        // Debug.Log($"Delivering reward for successful choice: {optionText}");
        foreach (IdIntPair entry in rewards)
        {
            // Debug.Log($"Delivered {entry.objectID} ({entry.amount})");

            var foundObject = GameCodex.GetBaseObject(entry.objectID); // ensure any item ID is trimmed properly

            if (foundObject != null)
            {
                if (foundObject.objectType == ObjectType.Quest)
                {
                    Player.SetQuest(entry.objectID, entry.amount);
                }
                else
                {
                    Player.Add(new IdIntPair() { objectID = foundObject.objectID, amount = entry.amount, description = entry.description });
                }
            }
            else
            {
                if (entry.objectID.Length > 8 && entry.objectID.Contains("-Q"))//objectID.Substring(6, 2) == "-Q")Substring(6, 2) == "-Q")
                {
                    Player.SetQuest(entry.objectID, entry.amount);
                }
                else
                {
                    Debug.Log($"{entry.objectID} NOT FOUND IN GAME CODEX. INTENTIONAL?");
                    Player.Add(new IdIntPair() { objectID = entry.objectID, amount = entry.amount, description = entry.description });
                }
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