﻿using System.Collections.Generic;
using UnityEngine;

public static class DialogueSetup
{
    public static void InitialiseDialogue(Dialogue dialogue)
    {
        if (string.IsNullOrEmpty(dialogue.objectID))
        {
            Debug.Log($"Missing objectID for {dialogue}");
        }
        else
        {
            ParseDialogueID(dialogue);

            if (dialogue.stageType == StageType.Dialogue)
            {
                ParseDialogueSteps(dialogue); //set up dialogue steps with proper speaker objects
            }

            if (string.IsNullOrEmpty(dialogue.speakerID))
            {
                dialogue.speakerID = dialogue.objectID.Split('-')[0];
            }

            Dialogues.all.Add(dialogue);
        }
    }
    static void ParseDialogueID(Dialogue dialogue)
    {
        //SET STAGE TYPE
        if (dialogue.objectID.Substring(14, 1) == "M")
        {
            dialogue.stageType = StageType.Memory;
        }
        else if (dialogue.objectID.Substring(14, 1) == "P")
        {
            dialogue.stageType = StageType.PopUp;
        }
        else if (dialogue.objectID.Substring(14, 1) == "N")
        {
            dialogue.stageType = StageType.Node;
        }
        else if (dialogue.objectID.Substring(14, 1) == "L")
        {
            dialogue.stageType = StageType.Letter;
        }

        //SET QUEST STAGE
        dialogue.questStage = int.Parse(dialogue.objectID.Substring(11, 2));

        //SET QUEST ID
        dialogue.questID = dialogue.objectID.Substring(0, 10);
    }
    static void ParseDialogueSteps(Dialogue dialogue)
    {
        dialogue.dialogueEvents = CreateEventsFromStringList(dialogue.content);
    }

    public static List<DialogueEvent> CreateEventsFromStringList (List<string> content)
    {
        List<DialogueEvent> events = new();

        for (int i = 0; i < content.Count; i = i + 2)
        {
            // PARSE SPEAKER TAG
            var speakerLine = content[i];

            if (!speakerLine.Contains("-")) // Treat as old step
            {
                string speakerTag = content[i];
                Character foundSpeaker = Characters.all.Find((s) => s.dialogueTag.ToLower() == speakerTag.ToLower());
                if (foundSpeaker != null)
                {
                    // PARSE CONTENT AND ADD EVENT
                    DialogueEvent dEvent = new() { speaker = foundSpeaker, eventName = $"{foundSpeaker.name} - step{Mathf.FloorToInt(i / 2 + 1)}" };
                    dEvent.content = content[i + 1];
                    events.Add(dEvent);

                    if (string.IsNullOrEmpty(dEvent.startingPlacement) && string.IsNullOrEmpty(dEvent.targetPlacement))
                    {
                        dEvent.targetPlacement = "NOR";
                    }
                }
                else
                {
                    Debug.LogError($"Could not parse dialogue content for content stage {i} ({content[i]}) because speaker tag \"{speakerTag}\" return null.");
                }
            }
            else
            {
                try
                {
                    DialogueEvent dEvent = ParseDialogueEventID(speakerLine);
                    dEvent.content = content[i + 1];
                    events.Add(dEvent);

                    if (string.IsNullOrEmpty(dEvent.startingPlacement) && string.IsNullOrEmpty(dEvent.targetPlacement))
                    {
                        dEvent.targetPlacement = "NOR";
                    }
                }
                catch
                {
                    Debug.LogError($"Something went wrong when attempting to parse content for step {i} ({content[i + 1]} ).");
                }
            }
        }

        return events;
    }

    public static DialogueEvent ParseDialogueEventID(string eventID)
    {
        // Example ID: Arcanist-S#SAD-SP#MID-TP#FAR-E#SHAKE-MAS#SLO-L#TRUE-HO#TRUE
        // = show sad arcanist sprite, start mid and move slowly to far position, shaking effect, left position portrait and hide other

        // Example ID: Gardener-S#HAPPY-OPO#Arcanist
        // = show happy gardener and override other portrait with alchemist. Other portrait override will also be parsed if it exists.

        string[] eventTags = eventID.Split('-');
        Character speaker = Characters.FindByTag(eventTags[0], "ParseDialogueEvent");

        if (speaker != null)
        {
            DialogueEvent dEvent = new();
            dEvent.speaker = speaker;

            if (eventID.Contains("L#TRUE"))
            {
                dEvent.isLeft = true;
            }

            if (eventID.Contains("HO#TRUE"))
            {
                dEvent.hideOtherPortrait = true;
            }

            if (eventID.Contains("HB#TRUE"))
            {
                dEvent.hideBothPortraits = true;
            }

            if (eventID.Contains("HS#TRUE"))
            {
                dEvent.hideSpeakerPortrait = true;
            }

            foreach (string tag in eventTags)
            {
                if (tag.Contains("SP#"))
                {
                    dEvent.startingPlacement = tag.Replace("SP#", "");
                }
                else if (tag.Contains("TP#"))
                {
                    dEvent.targetPlacement = tag.Replace("TP#", "");
                }
                else if (tag.Contains("MAS#"))
                {
                    dEvent.moveAnimationSpeed = tag.Replace("MAS#", "");
                }
                else if (tag.Contains("OPO#"))
                {
                    dEvent.otherPortraitOverride.Add(tag.Replace("OPO#", ""));
                    // Add override parse call here
                }
                else if (tag.Contains("E#"))
                {
                    dEvent.effect = tag.Replace("E#", "");
                }
                else if (tag.Contains("S#"))
                {
                    // Format in a way that a future SpriteManager can handle.

                    var spriteTag = dEvent.speaker.objectID + "-" + tag.Replace("S#", "");
                    // Debug.Log("Parsed sprite tag: " + spriteTag);

                    dEvent.spriteID = spriteTag;
                }
                else if (tag.Contains("BG#"))
                {
                    // Format in a way that a future SpriteManager can handle.

                    var spriteTag = dEvent.speaker.objectID + "-" + tag.Replace("BG#", "");
                    // Debug.Log("Parsed sprite tag: " + spriteTag);

                    dEvent.backgroundID = spriteTag;
                }
            }

            if (string.IsNullOrEmpty(dEvent.spriteID))
            {
                dEvent.spriteID = "DEFAULT";
            }

            return dEvent;
        }
        else
        {
            Debug.LogWarning("Could not find speaker object for event ID " + eventID);
            return null;
        }
    }

    static DialogueEvent ParseOverrideEventID(string eventID)
    {
        return new();
    }
}

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