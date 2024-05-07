using System.Collections.Generic;
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
        //Debug.Log($"parsing ${dialogue.objectID}" +
        //    $"\n Content count: {dialogue.content.Count}");

        for (int i = 0; i < dialogue.content.Count; i = i + 2)
        {
            // PARSE SPEAKER TAG
            var speakerLine = dialogue.content[i];

            if (!speakerLine.Contains("-")) // Treat as old step
            {
                string speakerTag = dialogue.content[i];
                Character foundSpeaker = Characters.all.Find((s) => s.dialogueTag.ToLower() == speakerTag.ToLower());
                if (foundSpeaker != null)
                {
                    //// PARSE CONTENT AND ADD STEP
                    //DialogueStep step = new() { name = $"{foundSpeaker.name} - step{Mathf.FloorToInt(i / 2 + 1)}" };
                    //step.speaker = foundSpeaker;
                    //step.text = dialogue.content[i + 1];
                    //dialogue.dialogueSteps.Add(step);

                    // PARSE CONTENT AND ADD EVENT
                    DialogueEvent dEvent = new () { speaker = foundSpeaker, eventName = $"{foundSpeaker.name} - step{Mathf.FloorToInt(i / 2 + 1)}" };
                    dEvent.content = dialogue.content[i + 1];
                    dialogue.dialogueEvents.Add(dEvent);

                    if (string.IsNullOrEmpty(dEvent.startingPlacement) && string.IsNullOrEmpty(dEvent.targetPlacement))
                    {
                        dEvent.targetPlacement = "NOR";
                    }
                }
                else
                {
                    Debug.LogWarning($"Could not parse dialogue content for {dialogue.questID} stage {dialogue.questStage} because speaker tag \"{speakerTag}\" return null.");
                }
            }
            else
            {
                try
                {
                    DialogueEvent dEvent = ParseDialogueEventID(speakerLine);
                    dEvent.content = dialogue.content[i + 1];
                    dialogue.dialogueEvents.Add(dEvent);

                    if (string.IsNullOrEmpty(dEvent.startingPlacement) && string.IsNullOrEmpty(dEvent.targetPlacement))
                    {
                        dEvent.targetPlacement = "NOR";
                    }
                }
                catch
                {
                    Debug.LogError($"Something went wrong when attempting to parse content for {dialogue.objectID} step {i}.");
                }
            }
        }
    }

    static DialogueEvent ParseDialogueEventID(string eventID)
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
    public string startingPlacement; //SP# OFF-FAR-NOR-CLO-MID
    public string targetPlacement; //TP# OFF-FAR-NOR-CLO-MID
    public string effect;
    public string moveAnimationSpeed; //MAS# NON-SLO-MED-FAS
    public bool isLeft = false;
    public bool hideOtherPortrait = false;

    public List<string> otherPortraitOverride; // characterID + any data besides isleft, hide, content
}