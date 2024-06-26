﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        if (Player.claimedLoot.Count > 0)
        {
            CheckLootNodes();
        }
    }

    static void CheckLootNodes()
    {
        UnityEngine.Debug.Log("Checking loot nodes.");

        var respawningLoot = Player.claimedLoot.Where(e => e.objectID.Contains("WorldNodeLoot") && !e.objectID.Contains("disableRespawn")).ToList();

        foreach (var entry in respawningLoot)
        {
            UnityEngine.Debug.Log("Checking node with ID " + entry.objectID);
            entry.amount++;

            if (entry.objectID.Contains("_CD#")) // By default, roll respawn chance
            {
                UnityEngine.Debug.Log("Found _CD#");
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
                    UnityEngine.Debug.Log("Parsing cooldown failed: " + data[1]);
                }
            }
            else if (entry.objectID.Contains("_ECD#")) // Exact cooldown means no random roll
            {
                var data = entry.objectID.Split("_ECD#");

                if (int.TryParse(data[1], out var result))
                {
                    if (result <= entry.amount)
                    {
                        UnityEngine.Debug.Log("Removing loot with exact cooldown.");
                        Player.claimedLoot.Remove(entry);
                    }
                }
                else
                {
                    UnityEngine.Debug.Log("Parsing exact cooldown failed: " + data[1]);
                }
            }
            else
            {
                UnityEngine.Debug.Log("Could not find cooldown data for " + entry.objectID);
            }
        }

        static void RollForRespawn(IdIntPair entry, int minCooldown)
        {
            UnityEngine.Debug.Log("Rolling for respawn.");
            int extraTime = entry.amount - minCooldown;

            if (UnityEngine.Random.Range(0, 100) < (10 + (extraTime * 10)))
            {
                Player.claimedLoot.Remove(entry);
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