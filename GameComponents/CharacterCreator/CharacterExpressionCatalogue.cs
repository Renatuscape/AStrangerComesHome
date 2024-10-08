﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterExpressionCatalogue : MonoBehaviour
{
    public List<Sprite> unsortedSprites;
    public List<PlayerExpressionPackage> expressionPackages = new();
    public bool ready = false;
    public int index = 0;
    void Start()
    {
        if (unsortedSprites != null && unsortedSprites.Count > 0)
        {
            AssemblePackages();
        }
    }
    public PlayerExpressionPackage GetPackageByID(string expressionID)
    {
        string trimmedID;
        
        if (expressionID.Contains("-"))
        {
            trimmedID = expressionID.Split("-")[1];
        }
        else
        {
            trimmedID = expressionID;
        }

        if (ready)
        {
            var package = expressionPackages.FirstOrDefault(p => p.expressionID.Contains(trimmedID));

            if (package == null)
            {
                return expressionPackages.FirstOrDefault(p => p.expressionID.Contains("DEFAULT"));
            }
            else
            {
                return package;
            }
        }
        else
        {
            Debug.Log("Could not find expression by ID " + expressionID);
            return null;
        }
    }

    public PlayerExpressionPackage GetNextPackageByIndex(bool goBack)
    {
        if (goBack)
        {
            index--;

            if (index < 0)
            {
                index = expressionPackages.Count - 1;
            }
        }
        else
        {
            index++;
            if (index >= expressionPackages.Count)
            {
                index = 0;
            }
        }

        return expressionPackages[index];
    }

    void AssemblePackages()
    {
        foreach (var sprite in unsortedSprites)
        {
            var spriteData = sprite.name.Split('_');

            PlayerExpressionPackage package = expressionPackages.FirstOrDefault(p => p.expressionID == spriteData[0]);

            if (package == null)
            {
                package = new() { expressionID = spriteData[0] };
                expressionPackages.Add(package);
            }

            AddToPackage(package, sprite);
        }

        ready = true;
    }

    void AddToPackage(PlayerExpressionPackage package, Sprite sprite)
    {
        if (sprite.name.Contains("Expression"))
        {
            package.expression = sprite;
        }
        else if (sprite.name.Contains("BrowColour"))
        {
            package.browColour = sprite;
        }
        else if (sprite.name.Contains("LipTint"))
        {
            package.lipTint = sprite;
        }
    }
}

[Serializable]
public class PlayerExpressionPackage
{
    public string expressionID;
    public Sprite expression;
    public Sprite browColour;
    public Sprite lipTint;
}
