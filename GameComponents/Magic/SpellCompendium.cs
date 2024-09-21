using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellCompendium : MonoBehaviour
{
    public static SpellCompendium instance;

    private void Start()
    {
        instance = this;
    }

    public static void RecoverMemory()
    {
        var viableMemories = Memories.all.Where(m => !m.isUnique).ToList();

        Player.Add(viableMemories[Random.Range(0, viableMemories.Count)].objectID);
    }

    public static void ForgetMemory()
    {
        var viableMemories = Memories.all.Where(m => !m.isUnique && Player.GetCount(m.objectID, "Forget Memory") > 0).ToList();

        if (viableMemories.Count > 0)
        {
            Player.Remove(viableMemories[Random.Range(0, viableMemories.Count)].objectID);
        }
    }
}
