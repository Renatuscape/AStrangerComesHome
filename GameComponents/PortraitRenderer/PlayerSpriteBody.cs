using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerSpriteBody
{
    public Image lines;
    public Image cloak;
    public Image vest;
    public Image tights;
    public Image flats;
    public Image outline;

    public void LoadPackageWithColours(PlayerBodyPackage package)
    {
        if (package == null)
        {
            Debug.Log("Eyes package was null.");
        }
        else
        {
            ApplyBodyPackage(package);

            var dataManager = TransientDataScript.gameManager.dataManager;
            cloak.color = TransientDataScript.GetColourFromHex(dataManager.playerSprite.cloakHexColour);
            vest.color = TransientDataScript.GetColourFromHex(dataManager.playerSprite.vestHexColour);
            tights.color = TransientDataScript.GetColourFromHex(dataManager.playerSprite.tightsHexColour);
        }
    }

    public void ApplyBodyPackage(PlayerBodyPackage package)
    {
        //Color currentCloakColour = cloak.color;

        lines.sprite = package.lines;
        cloak.sprite = package.cloak;
        vest.sprite = package.vest;
        tights.sprite = package.tights;
        flats.sprite = package.flats;
        outline.sprite = package.outline;

        //iris.color = currentIrisColour;

        if (cloak.sprite == null)
        {
            cloak.gameObject.SetActive(false);
        }
        else
        {
            cloak.gameObject.SetActive(true);
        }

        if (vest.sprite == null)
        {
            vest.gameObject.SetActive(false);
        }
        else
        {
            vest.gameObject.SetActive(true);
        }

        if (tights.sprite == null)
        {
            tights.gameObject.SetActive(false);
        }
        else
        {
            tights.gameObject.SetActive(true);
        }
    }
}