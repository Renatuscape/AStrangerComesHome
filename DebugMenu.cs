using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    public GameObject characterCreator;

    public void OpenCharacterCreation()
    {
        TransientDataScript.SetGameState(GameState.CharacterCreation, "Debug Menu", gameObject);
        characterCreator.SetActive(true);
    }

    public void TripQuestTimers()
    {
        TransientDataScript.gameManager.dataManager.totalGameDays++;
        TransientDataScript.DailyTick();
        LogAlert.QueueTextAlert("Days passed +1");
    }

    public void DebugItems()
    {
        foreach (Item item in Items.all)
        {
            if (item.type != ItemType.Script)
            {
                Player.Add(item.objectID, 50, true);
            }
        }
    }

    public void DebugSkills()
    {
        foreach (Skill skill in Skills.all)
        {
            Player.Add(skill.objectID, 50, true);
        }
    }

    public void DebugUpgrades()
    {
        foreach (Upgrade upgrade in Upgrades.all)
        {
            Player.Add(upgrade.objectID, 50, true);
        }
    }

    public void DebugRecipes()
    {
        foreach (Recipe recipe in Recipes.all)
        {
            Player.Add(recipe.objectID, 50, true);
        }
    }

    public void PurgePassengers()
    {
        TransientDataScript.gameManager.dataManager.seatA.isActive = false;
        TransientDataScript.gameManager.dataManager.seatB.isActive = false;
    }

    public void ForgetMemory()
    {
        SpellCompendium.ForgetMemory();
    }

    public void RecoverMemory()
    {
        SpellCompendium.RecoverMemory();
    }

    public void EnableDemoMode()
    {
        Player.Add(StaticTags.Alchemy, 1, true);
        Player.Add("MIS022", 1, true); // Chemistry license object
        Player.Add("SCR006", 1, true); // Alchemy set Stella Point
        Player.Add("SCR007", 1, true); // Alchemy set Capital
        Player.Add(StaticTags.CoachSynths, 1, true);

        Player.Add(StaticTags.CoachPlanters, 1, true);

        Player.Add(StaticTags.GuildLicense, 1, true); // Guild license script
        Player.Add("MIS021", 1, true); // Guild license object

        Player.Add("SCR013", 1, true); // Merchantile license level
        Player.Add("SCR016", 1, true); // Traveller estate unlock

        TransientDataScript.isDemoEnabled = true;
    }

    public void SkipPrologue()
    {
        // Ensure all mandatory rewards and quests are set to the least necessary level to complete the prologue chapter

        // Alchemist Quest
        Player.SetQuest("ARC001-Q00", 100);
        Player.Add("ARC001", 3, true);
        Player.Add(StaticTags.Fate, 1, true);


        // Machinist Quest
        Player.SetQuest("ARC002-Q00", 100);
        Player.Add("ARC002", 3, true);

        // Gardener Quest - also unlocks gardening
        Player.SetQuest("ARC003-Q00", 100);
        Player.Add("ARC003", 3, true);
        Player.Add(StaticTags.Gardening, 1, true);
        Player.Add(StaticTags.CoachPlanters, 1, true);

        // Teller Quest
        Player.SetQuest("ARC004-Q00", 100);
        Player.Add("ARC004", 3, true);

        // Occultist Quest
        Player.SetQuest("ARC006-Q00", 100);
        Player.Add("ARC006", 3, true);
    }
}
