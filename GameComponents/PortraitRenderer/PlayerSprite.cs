using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSprite : MonoBehaviour
{
    public CharacterEyesCatalogue eyeCatalogue;
    public CharacterHairCatalogue hairCatalogue;
    public CharacterExpressionCatalogue expressionCatalogue;

    public DataManagerScript dataManager;
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
        UpdateAllFromGameData(out var hair, out var eyes);
    }

    public void SetExpressionToDefault()
    {
        var defaultPackage = expressionCatalogue.GetPackageByID("DEFAULT");
        expressionCatalogue.index = expressionCatalogue.expressionPackages.IndexOf(defaultPackage);
        SetExpression(defaultPackage);
    }

    public void SetExpression(PlayerExpressionPackage package)
    {
        expression.sprite = package.expression;
        Color lipColour = lipTint.color;
        lipTint.sprite = package.lipTint;
        lipTint.color = lipColour;

        Color browColor = playerHair.browColour.color;
        playerHair.browColour.sprite = package.browColour;
        playerHair.browColour.color = browColor;
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

    public void UpdateAllFromGameData(out PlayerHairPackage hairPackage, out PlayerEyesPackage eyePackage)
    {
        Debug.Log("Attempting to update Player Sprite from game data.");

        ChangeBody(dataManager.bodyIndex);
        ChangeHead(dataManager.headIndex);

        Color lipColour = TransientDataScript.GetColourFromHex(dataManager.playerSprite.lipTintHexColour);
        lipTint.color = new Color(lipColour.r, lipColour.g, lipColour.b, dataManager.playerSprite.lipTintTransparency);

        hairPackage = hairCatalogue.GetPackageByID(dataManager.playerSprite.hairID);
        eyePackage = eyeCatalogue.GetPackageByID(dataManager.playerSprite.eyesID);

        playerHair.LoadPackageWithColours(hairPackage);
        playerEyes.LoadPackageWithColours(eyePackage);
    }
}
