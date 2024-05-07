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
}
