using System.Collections.Generic;

[System.Serializable]
public class DialogueStage
{
    public List<string> rawContent;
    public List<DialogueStep> dialogueSteps; //a step is one line of dialogue from one speaker
    public List<Choice> choices = new List<Choice>(); //LEAVE CHOICES BLANK TO LOOP DIALOGUE WITHOUT PROGRESSION
    public bool progressOnContinue; //automatically go to next stage when step is completed
    public bool noLeaveButton;
}