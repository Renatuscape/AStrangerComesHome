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
            string speakerTag = dialogue.content[i];
            Character foundSpeaker = Characters.all.Find((s) => s.dialogueTag.ToLower() == speakerTag.ToLower());
            if (foundSpeaker is not null)
            {
                DialogueStep step = new() { name = $"{foundSpeaker.name} - step{Mathf.FloorToInt(i / 2 + 1)}" };
                step.speaker = foundSpeaker;
                step.text = dialogue.content[i + 1];
                //Debug.Log($"New step {i} - {i + 1}| {step.speaker}: {step.text}");
                dialogue.dialogueSteps.Add(step);
            }
            else
            {
                Debug.LogWarning($"Could not parse dialogue content for {dialogue.questID} stage {dialogue.questStage} because speaker tag \"{speakerTag}\" return null.");
            }

        }
    }
}