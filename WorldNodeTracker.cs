using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public static class WorldNodeTracker
{
    public static List<CharacterNode> allExistingNodes = new();
    public static List<CharacterNode> spawnedCharacterNodes = new();
    public static void DisableNodeWithFade(string speakerID)
    {
        UnityEngine.Debug.Log("WNT: Attempting to disable node with ID " + speakerID);
        foreach (var cNode in spawnedCharacterNodes)
        {
            if (cNode.characterID.Contains(speakerID))
            {
                cNode.DisableWithFade();

                break;
            }
        }
    }

    public static void UpdateNodesOnDayTick()
    {
        UnityEngine.Debug.Log("WNT: Updating nodes on daily tick.");
        foreach (var cNode in allExistingNodes)
        {
            if (cNode.updateAtMidnight)
            {
                cNode.RefreshNode();
            }
        }
        
        if (Player.claimedLoot.Count > 0)
        {
            CheckLootNodes();
        }
    }

    static void CheckLootNodes()
    {
        // UnityEngine.Debug.Log("Checking loot nodes.");

        var respawningLoot = Player.claimedLoot.Where(e => e.objectID.Contains("WorldNodeLoot") && !e.objectID.Contains("disableRespawn")).ToList();

        foreach (var entry in respawningLoot)
        {
            // UnityEngine.Debug.Log("Checking node with ID " + entry.objectID);
            entry.amount++;

            if (entry.objectID.Contains("_CD#")) // By default, roll respawn chance
            {
                // UnityEngine.Debug.Log("Found _CD#");
                var data = entry.objectID.Split("_CD#");

                if (int.TryParse(data[1], out var result))
                {
                    if (result <= entry.amount)
                    {
                        RollForRespawn(entry, result);
                    }
                }
                else
                {
                    UnityEngine.Debug.Log("WNT: Parsing cooldown failed: " + data[1]);
                }
            }
            else if (entry.objectID.Contains("_ECD#")) // Exact cooldown means no random roll
            {
                var data = entry.objectID.Split("_ECD#");

                if (int.TryParse(data[1], out var result))
                {
                    if (result <= entry.amount)
                    {
                        UnityEngine.Debug.Log("WNT: Removing loot with exact cooldown.");
                        Player.claimedLoot.Remove(entry);
                    }
                }
                else
                {
                    UnityEngine.Debug.Log("WNT: Parsing exact cooldown failed: " + data[1]);
                }
            }
            else
            {
                UnityEngine.Debug.Log("WNT: Could not find cooldown data for " + entry.objectID);
            }
        }

        static void RollForRespawn(IdIntPair entry, int minCooldown)
        {
            // UnityEngine.Debug.Log("Rolling for respawn.");
            int extraTime = entry.amount - minCooldown;

            if (UnityEngine.Random.Range(0, 100) < (10 + (extraTime * 10)))
            {
                Player.claimedLoot.Remove(entry);
            }
        }
    }

    public static void ClearCharacterNodes()
    {
        UnityEngine.Debug.Log("WNT: Clearing character nodes.");
        spawnedCharacterNodes.Clear();
        allExistingNodes.Clear();
    }

    public static bool AddCharacterNode(CharacterNode cNode)
    {
        UnityEngine.Debug.Log("WNT: Adding character node " + cNode.generatedNodeID);
        var foundNode = spawnedCharacterNodes.FirstOrDefault(n => n.generatedNodeID == cNode.generatedNodeID);
        // PrintAllCharacterNodes();

        if (foundNode == null)
        {
            spawnedCharacterNodes.Add(cNode);

            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CheckIfCharacterExistsInDifferentNode(CharacterNode cNode)
    {
        var matchingNode = spawnedCharacterNodes.FirstOrDefault(n => n.character.objectID == cNode.characterID);
        // PrintAllCharacterNodes();

        if (matchingNode == null)
        {
            // UnityEngine.Debug.Log("No node was found with the same character object ID as " + cNode.characterID);
            return false;
        }
        else if (matchingNode.generatedNodeID != cNode.generatedNodeID)
        {
            // UnityEngine.Debug.Log("Matching node was found, but nodeID was not the same.");
            return true;
        }
        else
        {
            // UnityEngine.Debug.Log("Matching node was found, nodeID was identical.");
            return false;
        }
    }
    public static bool CheckIfCharacterExistsInWorld(string charID)
    {
        return spawnedCharacterNodes.FirstOrDefault(c => c.characterID == charID) != null;
    }

    public static void RemoveNodeFromList(CharacterNode cNode)
    {
        spawnedCharacterNodes.Remove(cNode);
    }

    public static void PrintAllCharacterNodes()
    {
        UnityEngine.Debug.Log($"WNT: List of nodes had {spawnedCharacterNodes.Count} nodes.");
        foreach (var node in spawnedCharacterNodes)
        {
            UnityEngine.Debug.Log($"Node {node.generatedNodeID}");
        }
    }
}