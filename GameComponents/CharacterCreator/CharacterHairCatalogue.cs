using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterHairCatalogue : MonoBehaviour
{
    public List<Sprite> unsortedSprites;
    public List<PlayerHairPackage> playerHairPackages = new();
    public bool ready = false;
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

            PlayerHairPackage package = playerHairPackages.FirstOrDefault(p => p.hairID == spriteData[0]);

            if (package == null)
            {
                package = new() { hairID = spriteData[0] };
                playerHairPackages.Add(package);
            }

            AddToPackage(package, sprite);
        }

        ready = true;
    }

    void AddToPackage(PlayerHairPackage package, Sprite sprite)
    {
        if (sprite.name.Contains("FrontLines"))
        {
            package.frontLines = sprite;
        }
        else if (sprite.name.Contains("FrontColour"))
        {
            package.frontColour = sprite;
        }
        else if (sprite.name.Contains("BackLines"))
        {
            package.backLines = sprite;
        }
        else if (sprite.name.Contains("BackColour"))
        {
            package.backColour = sprite;
        }
        else if (sprite.name.Contains("Outline"))
        {
            package.outline = sprite;
        }
        else if (sprite.name.Contains("AccessoryLines"))
        {
            package.accessoryLines = sprite;
        }
        else if (sprite.name.Contains("AccessoryColour"))
        {
            package.accessoryColour = sprite;
        }
        else if (sprite.name.Contains("AccessoryOutline"))
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
    public Sprite backLines;
    public Sprite backColour;
    public Sprite outline;
}
