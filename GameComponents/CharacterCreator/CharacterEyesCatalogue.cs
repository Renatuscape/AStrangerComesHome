using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterEyesCatalogue : MonoBehaviour
{
    public Sprite defaultIrises;
    public Sprite defaultSclera;
    public List<Sprite> unsortedSprites;
    public List<PlayerEyesPackage> eyePackages = new();
    public bool ready = false;
    public int index = 0;

    void Start()
    {
        if (unsortedSprites != null && unsortedSprites.Count > 0)
        {
            AssembleEyesPackages();
        }
    }

    public PlayerEyesPackage GetPackageByID(string eyesID)
    {
        if (ready)
        {
            var package = eyePackages.FirstOrDefault(p => p.eyesID == eyesID);

            if (package == null)
            {
                return eyePackages[0];
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

    public PlayerEyesPackage GetNextPackageByIndex(bool goBack)
    {
        if (goBack)
        {
            index--;

            if (index < 0)
            {
                index = eyePackages.Count - 1;
            }
        }
        else
        {
            index++;
            if (index >= eyePackages.Count)
            {
                index = 0;
            }
        }

        return eyePackages[index];
    }

    void AssembleEyesPackages()
    {
        foreach (var sprite in unsortedSprites)
        {
            var spriteData = sprite.name.Split('_');

            PlayerEyesPackage package = eyePackages.FirstOrDefault(p => p.eyesID == spriteData[0]);

            if (package == null)
            {
                package = new() { eyesID = spriteData[0] };
                eyePackages.Add(package);
            }

            AddToPackage(package, sprite);
        }

        foreach (var package in eyePackages)
        {
            if (package.sclera == null)
            {
                package.sclera = defaultSclera;
            }

            if (package.iris == null)
            {
                package.iris = defaultIrises;
            }
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