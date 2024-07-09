using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerSpriteExpression
{
    public Image expression;
    public Image eyebrowColour;
    public Image lipTint;

    public void SetExpression(PlayerExpressionPackage package)
    {
        Color currentBrowColour = eyebrowColour.color;
        Color currentLipColour = lipTint.color;

        expression.sprite = package.expression;
        eyebrowColour.sprite = package.eyebrowColour;
        lipTint.sprite = package.lipTint;

        eyebrowColour.color = currentBrowColour;
        lipTint.color = currentLipColour;
    }
}
