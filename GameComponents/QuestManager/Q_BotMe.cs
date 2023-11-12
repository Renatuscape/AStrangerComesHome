using System.Collections;
using System.Collections.Generic;

public class Q_BotMe : QuestManager
{
    private void OnEnable()
    {
        PopulateQuestDialogue();
    }
    public void PopulateQuestDialogue()
    {
        questStage = 0;
        topicMaster = Botanist;
        topicName = "Greeting";
        dialogueContent = new List<dynamic>();
        dialogueContent.AddMany
        (
            Botanist,
            "Traveller! It's been so long, hun.",
            Player,
            "It really has been.The Alchemist explained why you have turned to retail.",
            Botanist,
            "Oh, I don't mind it. I've been having a lot of fun, actually.Doing something new, earning a living.",
            Player,
            "I never took you for a capitalist.",
            Botanist,
            "It's not my fault if I have a head for business! What about you? I don't suppose you brought back any riches from your travels.",
            Player, "Out of everyone you know, I am literally the worst with money.I apparently racked up a massive debt while I was away, too.",
            Botanist,
            "Oh, my.You'll want to sort that out quickly. The Teller has only become more ruthless. What did you need a loan for in the first place? How come you have all this debt?"
        );

        quest.dialogues[questStage].choices[0].choiceText = "See, I got this coach...";
        quest.dialogues[questStage].choices[1].choiceText = "Fancy planters.";
        quest.dialogues[questStage].choices[1].succeededRequirementText = "I mean, I understand, but...";
        quest.dialogues[questStage].choices[2].choiceText = "Hats hats hats!";

        UpdateDialogue(topicMaster, topicName);


        questStage = 1;
        dialogueContent = new List<dynamic>();
        dialogueContent.AddMany
        (
            Botanist,
            "I will say, I'm not surprised by your poor economic choices.",
            Player,
            "Fortunately for me, I always stumble upon ways of making money."
        );

        quest.dialogues[questStage].choices[0].choiceText = "Right. I'll let you know if I have any further questions.";
        quest.dialogues[questStage].choices[0].succeededRequirementText = "You do that!";
        UpdateDialogue(topicMaster, topicName);
    }
    void UpdateDialogue(Character topicMaster, string topicName)
    {
        quest.dialogues[questStage].topicMaster = topicMaster;
        quest.dialogues[questStage].topicName = topicName;

        AddQuestStep(quest, questStage, topicMaster, topicName, dialogueContent);
        questStage++;
        dialogueContent = new List<dynamic>();
    }
}
