using System;
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
    public Image frontAccent;
    public Image backLines;
    public Image backColour;
    public Image backAccent;
    public Image outline;

    public void LoadPackageWithColours(PlayerHairPackage hairPackage)
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

            if (hairPackage.frontAccent != null)
            {
                frontAccent.sprite = hairPackage.frontAccent;
                backAccent.gameObject.SetActive(true);
            }
            else
            {
                backAccent.gameObject.SetActive(true);
            }

            if (hairPackage.backLines == null)
            {
                backLines.gameObject.SetActive(false);
                backColour.gameObject.SetActive(false);
                backAccent.gameObject.SetActive(false);
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

                if (hairPackage.backAccent != null)
                {
                    backAccent.sprite = hairPackage.backAccent;
                    backAccent.gameObject.SetActive(true);
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
