using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice
{
    public bool doesNotAdvance;
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
}