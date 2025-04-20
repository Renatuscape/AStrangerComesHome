using System.Collections.Generic;

[System.Serializable]
public class DialogueEvent
{
    public string eventName;
    public Character speaker;
    public string spriteID; // S# any tag that corresponds to a sprite event
    public string content;
    public string startingPlacement; // SP# OFF / FAR / NOR / CLO / MID
    public string targetPlacement; // TP# OFF / FAR / NOR / CLO / MID
    public string effect;
    public string moveAnimationSpeed; // MAS# NON / SLO / MED / FAS
    public string backgroundID; // Does this actually work? Test - BG# Remove / RemoveWithoutFade / imageName / imageName-WithoutFade / imageName-SlowFade / imageName-ExSlowFade / imageName-OnWhite / imageName-#FF00FF (HEX MUST ALWAYS BE LAST)
    public bool isLeft = false; // L#TRUE
    public bool hideBothPortraits = false; // HB#TRUE
    public bool hideSpeakerPortrait = false; // HS#TRUE
    public bool hideOtherPortrait = false; // HO#TRUE

    public List<string> otherPortraitOverride; // characterID + any data besides isleft, hide, content
}