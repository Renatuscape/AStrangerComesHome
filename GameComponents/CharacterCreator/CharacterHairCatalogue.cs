using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CharacterHairCatalogue : MonoBehaviour
{
    public List<Sprite> unsortedSprites;
    public List<PlayerHairPackage> hairPackages = new();
    public bool ready = false;
    public int index = 0;
    void Start()
    {
        if (unsortedSprites != null && unsortedSprites.Count > 0)
        {
            AssembleHairPackages();
        }
    }
    public PlayerHairPackage GetPackageByID(string hairID)
    {
        if (ready)
        {
            var package = hairPackages.FirstOrDefault(p => p.hairID == hairID);

            if (package == null)
            {
                return hairPackages[0];
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

    public PlayerHairPackage GetNextPackageByIndex(bool goBack)
    {
        if (goBack)
        {
            index--;

            if (index < 0)
            {
                index = hairPackages.Count - 1;
            }
        }
        else
        {
            index++;
            if (index >= hairPackages.Count)
            {
                index = 0;
            }
        }

        return hairPackages[index];
    }

    void AssembleHairPackages()
    {
        foreach (var sprite in unsortedSprites)
        {
            var spriteData = sprite.name.Split('_');

            PlayerHairPackage package = hairPackages.FirstOrDefault(p => p.hairID == spriteData[0]);

            if (package == null)
            {
                package = new() { hairID = spriteData[0] };
                hairPackages.Add(package);
            }

            AddToPackage(package, sprite);
        }

        ready = true;
    }

    void AddToPackage(PlayerHairPackage package, Sprite sprite)
    {
        if (sprite.name.Contains("_FrontLines"))
        {
            package.frontLines = sprite;
        }
        else if (sprite.name.Contains("_FrontColour"))
        {
            package.frontColour = sprite;
        }
        else if (sprite.name.Contains("_FrontAccent"))
        {
            package.frontAccent = sprite;
        }
        else if (sprite.name.Contains("_BackLines"))
        {
            package.backLines = sprite;
        }
        else if (sprite.name.Contains("_BackColour"))
        {
            package.backColour = sprite;
        }
        else if (sprite.name.Contains("_BackAccent"))
        {
            package.backAccent = sprite;
        }
        else if (sprite.name.Contains("_Outline"))
        {
            package.outline = sprite;
        }
        else if (sprite.name.Contains("_AccessoryLines"))
        {
            package.accessoryLines = sprite;
        }
        else if (sprite.name.Contains("_AccessoryColour"))
        {
            package.accessoryColour = sprite;
        }
        else if (sprite.name.Contains("_AccessoryOutline"))
        {
            package.accessoryOutline = sprite;
        }
    }
}

[Serializable]
public class PlayerHairPackage
{
    public string hairID;
    public Sprite accessoryLines;
    public Sprite accessoryColour;
    public Sprite accessoryOutline;
    public Sprite frontLines;
    public Sprite frontColour;
    public Sprite frontAccent;
    public Sprite backLines;
    public Sprite backColour;
    public Sprite backAccent;
    public Sprite outline;
}
