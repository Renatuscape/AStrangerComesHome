using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    public PortraitRenderer portraitRenderer;
    public PlayerSprite playerSprite;
    public Image head;
    public Image eyeColour;
    public Image eyeLines;
    public Image mouth;
    public Image hairOutline;
    public Image hairColour;
    public Image hairLinework;

    private void OnEnable()
    {
        UpdateImages();
    }
    
    public void UpdateImages()
    {
        portraitRenderer.gameObject.SetActive(true);
        playerSprite.gameObject.SetActive(true);
        playerSprite.gameObject.SetActive(false);
        portraitRenderer.gameObject.SetActive(false);

        head.sprite = playerSprite.head.sprite;
        eyeColour.color = playerSprite.eyesIrises.color;
        GetComponent<Image>().color = new Color(eyeColour.color.g - 0.5f, eyeColour.color.b - 0.2f, eyeColour.color.r - 0.2f, 0.5f);
        eyeLines.sprite = playerSprite.eyesLines.sprite;
        mouth.sprite = playerSprite.expression.sprite;

        hairOutline.sprite = playerSprite.playerHair.outline.sprite;
        hairLinework.sprite = playerSprite.playerHair.frontLines.sprite;

        hairColour.sprite = playerSprite.playerHair.frontColour.sprite;
        hairColour.color = playerSprite.playerHair.frontColour.color;
    }
}
