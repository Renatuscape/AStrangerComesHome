using System.Collections.Generic;
using System.Linq;

public static class CharacterNodeTracker
{
    public static List<CharacterNode> allExistingNodes = new();
    static List<CharacterNode> spawnedCharacterNodes = new();
    public static void DisableNodeWithFade(string speakerID)
    {
        foreach (var cNode in spawnedCharacterNodes)
        {
            if (cNode.characterID == speakerID)
            {
                cNode.DisableWithFade();

                break;
            }
        }
    }

    public static void UpdateNodesOnDayTick()
    {
        foreach (var cNode in allExistingNodes)
        {
            if (cNode.updateAtMidnight)
            {
                cNode.RefreshNode();
            }
        }
    }

    public static void ClearCharacterNodes()
    {
        spawnedCharacterNodes.Clear();
        allExistingNodes.Clear();
    }

    public static bool AddCharacterNode(CharacterNode cNode)
    {
        var foundCharacter = spawnedCharacterNodes.FirstOrDefault(c => c.characterID == cNode.characterID);

        if (foundCharacter == null)
        {
            spawnedCharacterNodes.Add(cNode);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CheckIfCharacterExistsInWorld(string charID)
    {
        return spawnedCharacterNodes.FirstOrDefault(c => c.characterID == charID) != null;
    }

    public static void RemoveWorldCharacterFromList(string charID)
    {
        var foundCharacter = spawnedCharacterNodes.FirstOrDefault(c => c.characterID == charID);
        spawnedCharacterNodes.Remove(foundCharacter);
    }
}