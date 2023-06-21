using System.Collections.Generic;
using UnityEngine;

public enum ChoiceType
{
    LeaveOnly,
    LoopDialogue,
    SetDialogueAndContinue,
    SetQuestAndContinue,
    SetQuestAndLeave
}

[CreateAssetMenu(fileName = "New Choice", menuName = "Scriptable Object/Choice")]
public class Choice : ScriptableObject
{
    public ChoiceType choiceType;
    public string choiceText;
    public int setQuestStage;
    public int setDialogueStage;


    //DELIVERY (subtracts value)
    public List<Item> deliveries;
    public List<int> deliveriesAmount;

    //LEVEL CHECK (only checks if dataValue is equal or more)

    public List<MotherObject> moreThanObject;
    public List<int> moreThanValue;
    public List<MotherObject> lessThanObject;
    public List<int> lessThanValue;

    //REWARD
    public List<MotherObject> rewards; //can be skill, item, upgrade, quest, recipe, whatever is a motherobject (so not gold, earn your own)
    public List<int> rewardsAmount;

    public string failedRequirementText;
    public string succeededRequirementText;
}