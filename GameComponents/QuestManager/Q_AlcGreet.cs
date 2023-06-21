using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text.RegularExpressions;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class Q_AlcGreet : QuestManager
{
    private void OnEnable()
    {
        PopulateQuestDialogue();
    }
    public void PopulateQuestDialogue()
    {
        questStage = 0;
        topicMaster = Alchemist;
        topicName = "Greeting";
        dialogueContent = new List<dynamic>();
        dialogueContent.AddMany
        (
            Alchemist, "Traveller! Ah, it truly has been a long time. What a delight that you should finally return to us. Pray tell, where have you been?",
            Player, "Oh, you know me – nowhere and everywhere. I straight up lost track of time. I take it that much has happened since I left? I barely recognise the place, or you, for that matter.",
            Alchemist, "It's... Ahem. Yes, things have happened in your absence. It's been nigh on two centuries since you left; a bit of change is to be expected.",
            Player, "Hm. Where is this?",
            Alchemist, "Stella Town. You surely remember how it was no more than a small encampment back when we last spoke.",
            Player, "When I advised you not to set up shop in such a forlorn place? Yes, and I see that my advice was sound. Not much has changed.",
            Alchemist, "Oh, you jest. There are plenty of people and plenty of business now.",
            Player, "And plenty of ruins. What happened? Did they build the town and regret their decisions? If this is the second attempt, I'm concerned.",
            Alchemist, "No, no I think you should perhaps reacquaint yourself with the world before we speak of that matter.",
            Player, "Not a bad idea. How are our friends doing?",
            Alchemist, "... Well enough. You may wish to speak with them in person for any further details.",
            Player, "Right. That doesn’t sound disconcerting at all.",
            Alchemist, "I am simply not at liberty to speak on their behalf when it comes to the topic of their wellbeing.",
            Player, "<i>Right.</i> Some things never change, and you are one of them, Alchemist.",
            Alchemist, "You flatter. At any rate, I kept your coach from falling into disrepair. Of course, it is terribly outdated now, and The Machinist will surely be able to modernise it. For a fee.",
            Player, "You concern yourself with money now?",
            Alchemist, "You will find that most of our old friends have taken up business of some sort. I trade in materials for alchemical syntheses, while The Botanist sells seeds and plants. Your coach has a rooftop garden, does it not?",
            Player, "I think so. What about The Teller?",
            Alchemist, "Business as usual at the bank. Be warned; they mumbled something about interest on your loans last time I saw them.",
            Player, "Ah. I suppose I'll stop by the Capital City and say hi. Eventually.",
            Alchemist, "As for the rest, I know little of their whereabouts. The common folk no longer revere us as they used to; expect nothing for free.",
            Player, "I am used to that sort of thing. Outside of (world name), I am just another traveller. Almost a Stranger, you could say!",
            Alchemist, "...",
            Player, "Lost your sense of humour, I see. What is the old bastard up to these days?",
            Alchemist, "The Stranger is as The Stranger does. You will not be seeing him any time soon.",
            Player, "Hm. What a pity.",
            Player, "Anyway, I appreciate that you kept my coach in order; I am excited to see how (world name) has changed since I left. I'll take on a few passengers here and there, and I'm always lucky in finding things along the road, so I won’t want for money.",
            Alchemist, "Do remember to ask for help if you need it. You may find some of the others to be a little cross with you, since you have been gone for so long, but you are a free spirit and cannot be caged. They know as much. I can only hope that (world name) will appear as new and fresh to you now as the first snowfall, and that you will stay with us for a long time."
        );

        quest.dialogues[questStage].choices[0].choiceText = "Let's speak again some time.";
        quest.dialogues[questStage].choices[0].succeededRequirementText = "It will be my pleasure, friend.";
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
