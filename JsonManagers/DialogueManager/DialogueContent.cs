using System.Collections.Generic;

[System.Serializable]
public class DialogueContent
{
    public string speaker;
    public string text;
    public List<Choice> choices = new List<Choice>(); //LEAVE CHOICES BLANK TO LOOP DIALOGUE WITHOUT PROGRESSION
    public bool noLeaveButton;
}