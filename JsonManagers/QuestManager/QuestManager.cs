using UnityEngine;

public static class QuestManager
{
    public static void Initialise(Quest quest)
    {
        quest.objectType = ObjectType.Quest;
        quest.maxValue = StaticGameValues.maxQuestValue;
        objectIDReader(ref quest);

        if (quest.dialogues == null || quest.dialogues.Count == 0) // Quest has yet to be updated to new standard and has dialogue in separate file
        {
            quest.dialogues = Dialogues.FindQuestDialogues(quest.objectID);
            Report.WriteWarning(quest.objectID + " quest dialogues are in separate file. Update to new standard.");
        }
        else
        {
            foreach (Dialogue dialogue in quest.dialogues)
            {
                DialogueManager.Initialise(dialogue);
                Repository.instance.dialogues.Add(dialogue);
            }
        }
    }

    public static void objectIDReader(ref Quest quest)
    {
        quest.questGiver = Characters.FindByID(quest.objectID.Substring(0, 6));
    }
}