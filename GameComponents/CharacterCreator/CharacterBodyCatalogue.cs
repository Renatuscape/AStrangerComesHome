using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterBodyCatalogue : MonoBehaviour
{
    public List<Sprite> unsortedSprites;
    public List<PlayerBodyPackage> bodyPackages = new();
    public bool ready = false;
    public int index = 0;
    void Start()
    {
        if (unsortedSprites != null && unsortedSprites.Count > 0)
        {
            AssembleHairPackages();
        }
    }

    void AssembleHairPackages()
    {
        foreach (var sprite in unsortedSprites)
        {
            var spriteData = sprite.name.Split('_');

            PlayerBodyPackage package = bodyPackages.FirstOrDefault(p => p.bodyID == spriteData[0]);

            if (package == null)
            {
                package = new() { bodyID = spriteData[0] };
                bodyPackages.Add(package);
            }

            AddToPackage(package, sprite);
        }

        ready = true;
    }

    void AddToPackage(PlayerBodyPackage package, Sprite sprite)
    {
        if (sprite.name.Contains("_Lines"))
        {
            package.lines = sprite;
        }
        else if (sprite.name.Contains("_Cloak"))
        {
            package.cloak = sprite;
        }
        else if (sprite.name.Contains("_Vest"))
        {
            package.vest = sprite;
        }
        else if (sprite.name.Contains("_Tights"))
        {
            package.tights = sprite;
        }
        else if (sprite.name.Contains("_Flats"))
        {
            package.flats = sprite;
        }
        else if (sprite.name.Contains("_Outline"))
        {
            package.outline = sprite;
        }
    }

    public PlayerBodyPackage GetPackageByID(string bodyID)
    {
        if (ready)
        {
            var package = bodyPackages.FirstOrDefault(p => p.bodyID == bodyID);

            if (package == null)
            {
                return bodyPackages[0];
            }
            else
            {
                return package;
            }
        }
        else
        {
            return null;
        }
    }

    public PlayerBodyPackage GetNextPackageByIndex(bool isPrevious)
    {
        if (isPrevious)
        {
            index--;

            if (index < 0)
            {
                index = bodyPackages.Count - 1;
            }
        }
        else
        {
            index++;
            if (index >= bodyPackages.Count)
            {
                index = 0;
            }
        }

        return bodyPackages[index];
    }
}

[Serializable]
public class PlayerBodyPackage
{
    public string bodyID;
    public Sprite lines;
    public Sprite cloak;
    public Sprite vest;
    public Sprite tights;
    public Sprite flats;
    public Sprite outline;
}