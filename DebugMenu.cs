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

    public void EnableDemoMode()
    {
        Player.Add("ALC000", 1, true);
        Player.Add("GAR000", 1, true);
        Player.Add("SCR004", 1, true);
        Player.Add("SCR006", 1, true);
        Player.Add("SCR007", 1, true);
        Player.Add("SCR010", 1, true);
        Player.Add("SCR012", 1, true);
        Player.Add("SCR013", 1, true);
        Player.Add("SCR016", 1, true);
    }

    public void SkipPrologue()
    {
        Player.Add("MIS021", 1, true);
        Player.Add("MIS022", 1, true);
        Player.Add("MIS023", 1, true);

        Player.SetQuest("ARC001-Q01", 18);
        Player.SetQuest("ARC000-Q00", 7);
        Player.SetQuest("ARC002-Q00", 100);
        Player.SetQuest("ARC003-Q00", 100);
        Player.SetQuest("ARC004-Q00", 100);
        Player.SetQuest("ARC006-Q00", 100);
    }
}
