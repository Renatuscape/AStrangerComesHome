using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerSpriteEyes
{
    public Image sclera;
    public Image iris;
    public Image lines;

    public void LoadPackageWithColours(PlayerEyesPackage package)
    {
        if (package == null)
        {
            Debug.Log("Eyes package was null.");
        }
        else
        {
            sclera.sprite = package.sclera;
            iris.sprite = package.iris;
            lines.sprite = package.lines;

            var dataManager = TransientDataScript.gameManager.dataManager;
            iris.color = TransientDataScript.GetColourFromHex(dataManager.playerSprite.eyesHexColour);
        }
    }

    public void ApplyEyesPackage(PlayerEyesPackage eyePackage)
    {
        Color currentIrisColour = iris.color;

        sclera.sprite = eyePackage.sclera;
        iris.sprite = eyePackage.iris;
        lines.sprite = eyePackage.lines;

        iris.color = currentIrisColour;
    }
}
