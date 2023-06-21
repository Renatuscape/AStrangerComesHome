using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string topicName;
    public Character topicMaster;
    public List<Character> speakers;//keep blank to default to topicMaster for every line
    [TextArea(5, 20)]
    public List<string> content;
    public List<Choice> choices = new List<Choice>(); //LEAVE CHOICES BLANK TO LOOP DIALOGUE WITHOUT PROGRESSION
    public bool noLeaveButton; //should the dialogue include a default leave button or not?
    // Add any other dialogue-related properties here
}