using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerSpriteHair
{
    public Image eyebrowColour;
    public Image accessoryLines;
    public Image accessoryColour;
    public Image accessoryOutline;
    public Image frontLines;
    public Image frontColour;
    public Image backLines;
    public Image backColour;
    public Image outline;

    public void LoadPackageWithColoursFromData(PlayerHairPackage hairPackage)
    {
        var dataManager = TransientDataScript.gameManager.dataManager;

        ApplyHairPackage(hairPackage, dataManager.playerSprite.enableAccessory);
        ApplyHairColour(TransientDataScript.GetColourFromHex(dataManager.playerSprite.hairHexColour));
        ApplyAccessoryColour(TransientDataScript.GetColourFromHex(dataManager.playerSprite.accessoryHexColour));
    }

    public void ApplyHairPackage(PlayerHairPackage hairPackage, bool enableAccessory)
    {
        if (hairPackage != null)
        {
            Color existingHairColour = frontColour.color;
            Debug.Log("Stored hair colour is " + existingHairColour.ToString());

            frontLines.sprite = hairPackage.frontLines;
            frontColour.sprite = hairPackage.frontColour;
            outline.sprite = hairPackage.outline;

            if (hairPackage.backLines == null)
            {

                backLines.gameObject.SetActive(false);
                backColour.gameObject.SetActive(false);
            }
            else
            {
                backLines.sprite = hairPackage.backLines;
                backColour.sprite = hairPackage.backColour;

                if (!backLines.gameObject.activeInHierarchy)
                {
                    backLines.gameObject.SetActive(true);
                    backColour.gameObject.SetActive(true);
                }
            }

            ApplyHairColour(existingHairColour);

            if (hairPackage.accessoryLines == null)
            {
                accessoryLines.gameObject.SetActive(false);
                accessoryColour.gameObject.SetActive(false);
                accessoryOutline.gameObject.SetActive(false);
            }
            else
            {
                Color existingAccessoryColour = accessoryColour.color;

                accessoryLines.sprite = hairPackage.accessoryLines;
                accessoryColour.sprite = hairPackage.accessoryColour;
                accessoryOutline.sprite = hairPackage.accessoryOutline;

                if (!accessoryLines.gameObject.activeInHierarchy && enableAccessory)
                {
                    accessoryLines.gameObject.SetActive(true);
                    accessoryColour.gameObject.SetActive(true);
                    accessoryOutline.gameObject.SetActive(true);
                }

                ApplyAccessoryColour(existingAccessoryColour);
            }
        }
    }
    public void ApplyHairColour(Color color)
    {
        eyebrowColour.color = color;
        frontColour.color = color;
        if (backColour != null)
        { backColour.color = color; }
    }

    public void ApplyAccessoryColour(Color color)
    {
        if (accessoryColour != null)
        { accessoryColour.color = color; }
    }

    public void ToggleAccessory(bool isEnabled)
    {
        if (accessoryLines != null)
        {
            accessoryLines.gameObject.SetActive(isEnabled);
        }
        if (accessoryColour != null)
        {
            accessoryColour.gameObject.SetActive(isEnabled);
        }
        if (accessoryOutline != null)
        {
            accessoryOutline.gameObject.SetActive(isEnabled);
        }
    }
}

public class PlayerSprite : MonoBehaviour
{
    public CharacterEyesCatalogue eyeCatalogue;
    public CharacterHairCatalogue hairCatalogue;

    public DataManagerScript dataManager;
    public List<Sprite> playerHairColours;
    public List<Sprite> playerHairLineworks;
    public List<Sprite> playerHairOutlines;
    public List<Sprite> playerBodyTypes;
    public List<Sprite> playerHeads;
    public List<Sprite> playerEyes;

    public Image body;
    public Image head;
    public Image expression;

    public Image eyesLines;
    public Image eyesIrises;
    public Image eyesSclera;

    public PlayerSpriteHair playerHair;

    public Image lipTint;

    private void OnEnable()
    {
        UpdateAllFromGameData();
    }

    public void ChangeBody(int index)
    {
        body.sprite = playerBodyTypes[index];
    }
    public void ChangeHead(int index)
    {
        head.sprite = playerHeads[index];
    }
    public void ChangeEyes(int index)
    {
        eyesLines.sprite = playerEyes[index];
    }

    public void ColourHair(string hexColour)
    {
        if (ColorUtility.TryParseHtmlString("#" + hexColour, out Color hairColor))
        {
            playerHair.ApplyHairColour(hairColor);
        }
        else
        {
            playerHair.ApplyHairColour(Color.white); // Default to white if parsing fails
        }
    }

    public void ColourEyes(string hexColour)
    {
        // Apply eye color
        if (ColorUtility.TryParseHtmlString("#" + hexColour, out Color eyeColor))
        {
            eyesIrises.color = eyeColor;
        }
        else
        {
            eyesIrises.color = Color.white;
        }
    }
    public void UpdateSpriteIndexFromGameData()
    {
        // playerHair.ApplyHairPackage(hairCatalogue.GetPackageByID(dataManager.playerSprite.hairID), dataManager.playerSprite.enableAccessory);

        ChangeBody(dataManager.bodyIndex);
        ChangeHead(dataManager.headIndex);
        ChangeEyes(dataManager.eyesIndex);
    }

    public void UpdateSpriteColourFromGameData()
    {
        ColourEyes(dataManager.eyesHexColour);
    }

    public void UpdateAllFromGameData()
    {
        UpdateSpriteIndexFromGameData();
        UpdateSpriteColourFromGameData();

        Color lipColour = TransientDataScript.GetColourFromHex(dataManager.playerSprite.lipTintHexColour);
        lipTint.color = new Color(lipColour.r, lipColour.g, lipColour.b, dataManager.playerSprite.lipTintTransparency);

        playerHair.LoadPackageWithColoursFromData(hairCatalogue.GetPackageByID(dataManager.playerSprite.hairID));
    }
}
