using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Quest : MotherObject
{
    public bool excludeFromJournal;
    public bool firstMeeting;
    public int daysPassedCheck;
    public List<MotherObject> checkMoreThan;
    public int checkMoreThanValue;
    public List<MotherObject> checkLessThan;
    public int checkLessThanValue;

    //Repeatable quests increase in dataLevel to 100 on completion, and are reset to 10 (skipping discovery stages) by the world clock
    public bool resetDaily; //common plants and materials that spawn in specific locations
    public bool resetWeekly; //less common plants and materials
    public bool resetMonthly; //character events like dates
    public bool resetYearly; //festivals, special dates, special character events like birthdays, very rare item spawns

    public bool dailyValue;//quest value is set to reflect the weekday (0 - 6), changing dialogue and other features
    public bool weeklyValue;//quest value is set to reflect week (0 - 3)
    public bool monthlyValue; //quest value is set to reflect month (0 - 11)
    public bool yearlyValue; //quest value equals years played, unlocking new things 

    //public MonoScript questBehaviour; //Additional quest behaviour

    public List<Dialogue> dialogues = new List<Dialogue>();
    // Add any other quest-related properties here

    // Check quest stage's dialogue list of characters and see if there is a match. Otherwise public SerializableDictionary<int, Character> perStageNPC;
    void Awake()
    {
        objectType = ObjectType.Quest;
    }
}
