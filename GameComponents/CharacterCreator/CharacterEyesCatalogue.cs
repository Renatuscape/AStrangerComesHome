using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterEyesCatalogue : MonoBehaviour
{
    public List<Sprite> unsortedSprites;
    public List<PlayerEyesPackage> playerHairPackages = new();
    public bool ready = false;
    public int index = 0;
    void Start()
    {
        if (unsortedSprites != null && unsortedSprites.Count > 0)
        {
            AssembleEyesPackages();
        }
    }
    void AssembleEyesPackages()
    {
        foreach (var sprite in unsortedSprites)
        {
            var spriteData = sprite.name.Split('_');

            PlayerEyesPackage package = playerHairPackages.FirstOrDefault(p => p.eyesID == spriteData[0]);

            if (package == null)
            {
                package = new() { eyesID = spriteData[0] };
                playerHairPackages.Add(package);
            }

            AddToPackage(package, sprite);
        }

        ready = true;
    }

    void AddToPackage(PlayerEyesPackage package, Sprite sprite)
    {
        if (sprite.name.Contains("Lines"))
        {
            package.lines = sprite;
        }
        else if (sprite.name.Contains("Iris"))
        {
            package.iris = sprite;
        }
        else if (sprite.name.Contains("Sclera"))
        {
            package.sclera = sprite;
        }
    }
}

[Serializable]
public class PlayerEyesPackage
{
    public string eyesID;
    public Sprite lines;
    public Sprite iris;
    public Sprite sclera;
}