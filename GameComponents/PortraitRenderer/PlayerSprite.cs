using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Image body;
    public Image head;
    public Image expression;

    public PlayerSpriteEyes playerEyes;
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
        if (ColorUtility.TryParseHtmlString("#" + hexColour, out Color eyeColor))
        {
            playerEyes.iris.color = eyeColor;
        }
        else
        {
            playerEyes.iris.color = Color.white;
        }
    }
    public void UpdateSpriteIndexFromGameData()
    {
        // playerHair.ApplyHairPackage(hairCatalogue.GetPackageByID(dataManager.playerSprite.hairID), dataManager.playerSprite.enableAccessory);

        ChangeBody(dataManager.bodyIndex);
        ChangeHead(dataManager.headIndex);
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

        playerHair.LoadPackageWithColours(hairCatalogue.GetPackageByID(dataManager.playerSprite.hairID));
        playerEyes.LoadPackageWithColours(eyeCatalogue.GetPackageByID(dataManager.playerSprite.eyesID));
    }
}
