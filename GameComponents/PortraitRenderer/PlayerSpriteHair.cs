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

        ApplyHairPackage(hairPackage, dataManager.playerSprite.enableAccessory, dataManager.playerSprite.enableAccent);
        ApplyHairColour(TransientDataScript.GetColourFromHex(dataManager.playerSprite.hairHexColour));
        ApplyAccessoryColour(TransientDataScript.GetColourFromHex(dataManager.playerSprite.accessoryHexColour));
    }

    public void ApplyHairPackage(PlayerHairPackage package, bool enableAccessory, bool enableAccent)
    {
        if (package == null)
        {
            Debug.Log("Hair package was null.");
        }
        else
        {
            Color existingHairColour = frontColour.color;
            Debug.Log("Stored hair colour is " + existingHairColour.ToString());

            frontLines.sprite = package.frontLines;
            frontColour.sprite = package.frontColour;
            outline.sprite = package.outline;

            if (package.frontAccent != null && enableAccent)
            {
                frontAccent.sprite = package.frontAccent;
                frontAccent.gameObject.SetActive(true);
            }
            else
            {
                frontAccent.gameObject.SetActive(false);
            }

            if (package.backLines == null)
            {
                backLines.gameObject.SetActive(false);
                backColour.gameObject.SetActive(false);
                backAccent.gameObject.SetActive(false);
            }
            else
            {
                backLines.sprite = package.backLines;
                backColour.sprite = package.backColour;

                if (!backLines.gameObject.activeInHierarchy)
                {
                    backLines.gameObject.SetActive(true);
                    backColour.gameObject.SetActive(true);
                }

                if (package.backAccent != null && enableAccent)
                {
                    backAccent.sprite = package.backAccent;
                    backAccent.gameObject.SetActive(true);
                }
                else
                {
                    backAccent.gameObject.SetActive(false);
                }
            }

            ApplyHairColour(existingHairColour);

            if (package.accessoryLines == null)
            {
                accessoryLines.gameObject.SetActive(false);
                accessoryColour.gameObject.SetActive(false);
                accessoryOutline.gameObject.SetActive(false);
            }
            else
            {
                Color existingAccessoryColour = accessoryColour.color;

                accessoryLines.sprite = package.accessoryLines;
                accessoryColour.sprite = package.accessoryColour;
                accessoryOutline.sprite = package.accessoryOutline;

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
