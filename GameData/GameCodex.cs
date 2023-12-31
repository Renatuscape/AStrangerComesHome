using System.Collections.Generic;
using Unity.VisualScripting;

public static class GameCodex
{
    public static string GetNameFromID(string objectID)
    {
        return ParseID(objectID).name;
    }
    public static bool ParseDynamicValues(dynamic dynamicObject, out int max, out string id)
    {
        if (dynamicObject is Item)
        {
            var item = (Item)dynamicObject;
            max = item.maxStack;
            id = item.objectID;
            return true;
        }
        else if (dynamicObject is Skill)
        {
            var skill = (Skill)dynamicObject;
            max = skill.maxLevel;
            id = skill.objectID;
            return true;
        }
        else if (dynamicObject is Upgrade)
        {
            var upgrade = (Upgrade)dynamicObject;
            max = upgrade.maxLevel;
            id = upgrade.objectID;
            return true;
        }
        else if (dynamicObject is Quest)
        {
            var quest = (Quest)dynamicObject;
            max = quest.maxValue;
            id = quest.objectID;
            return true;
        }
        else if (dynamicObject is Character)
        {
            var character = (Character)dynamicObject;
            max = character.maxValue;
            id = character.objectID;
            return true;
        }
        max = 0;
        id = string.Empty;
        return false;
    }
    public static dynamic ParseID(string searchID)
    {
        Item foundItem = Items.FindByID(searchID);
        if (foundItem != null)
        {
            return foundItem;
        }

        Skill foundSkill = Skills.FindByID(searchID);
        if (foundItem != null)
        {
            return foundSkill;
        }

        Upgrade foundUpgrade = Upgrades.FindByID(searchID);
        if (foundUpgrade != null)
        {
            return foundUpgrade;
        }

        Quest foundQuest = Quests.FindByID(searchID);
        if (foundQuest != null)
        {
            return foundQuest;
        }

        Character foundCharacter = Characters.FindByID(searchID);
        if (foundCharacter != null)
        {
            return foundCharacter;
        }

        return null;
    }
}
