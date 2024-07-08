using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePlayerSprite : MonoBehaviour
{
    public PlayerSprite playerSprite;
    //public PlayerSpriteHair playerHair;

    //public Image body;
    //public Image head;
    //public Image mouth;

    //public Image eyesLines;
    //public Image eyesIrises;
    //public Image eyesSclera;
    //public Image lipTint;

    public Image expression;
    public Image eyebrowColour;
    public Image lipTint;
    private void Awake()
    {
        eyebrowColour = playerSprite.playerHair.eyebrowColour;
        lipTint = playerSprite.lipTint;
    }
    private void OnEnable()
    {
        //playerHair.eyebrowColour.sprite = playerSprite.playerHair.eyebrowColour.sprite;
        //playerHair.accessoryLines.sprite = playerSprite.playerHair.accessoryLines.sprite;
        //playerHair.accessoryColour.sprite = playerSprite.playerHair.accessoryColour.sprite;
        //playerHair.accessoryOutline.sprite = playerSprite.playerHair.accessoryOutline.sprite;
        //playerHair.frontLines.sprite = playerSprite.playerHair.frontLines.sprite;
        //playerHair.frontColour.sprite = playerSprite.playerHair.frontColour.sprite;
        //playerHair.backLines.sprite = playerSprite.playerHair.backLines.sprite;
        //playerHair.backColour.sprite = playerSprite.playerHair.backColour.sprite;
        //playerHair.outline.sprite = playerSprite.playerHair.outline.sprite;

        //body.sprite = playerSprite.body.sprite;
        //head.sprite = playerSprite.head.sprite;
        //mouth.sprite = playerSprite.mouth.sprite;

        //eyesLines.sprite = playerSprite.eyesLines.sprite;
        //eyesIrises.sprite = playerSprite.eyesIrises.sprite;
        //eyesSclera.sprite = playerSprite.eyesSclera.sprite;

        //lipTint.sprite = playerSprite.lipTint.sprite;

        //lipTint.color = TransientDataScript.GetColourFromHex(playerSprite.dataManager.playerSprite.lipTintHexColour);
        //playerHair.LoadPackageWithColoursFromData(playerSprite.hairCatalogue.GetPackageByID(playerSprite.dataManager.playerSprite.hairID));

        CopyAllChildren();
    }

    public void CopyAllChildren()
    {
        foreach (Transform child in playerSprite.gameObject.transform)
        {
            GameObject copiedChild = Instantiate(child.gameObject);
            copiedChild.transform.SetParent(gameObject.transform);

            copiedChild.transform.localPosition = child.localPosition;
            copiedChild.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
