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
        eyeColour.color = playerSprite.irises.color;
        GetComponent<Image>().color = new Color(eyeColour.color.g - 0.5f, eyeColour.color.b - 0.2f, eyeColour.color.r - 0.2f, 0.5f);
        eyeLines.sprite = playerSprite.eyes.sprite;
        mouth.sprite = playerSprite.mouth.sprite;
        hairOutline.sprite = playerSprite.hairOutline.sprite;

        hairLinework.sprite = playerSprite.hairLinework.sprite;

        hairColour.sprite = playerSprite.hairColour.sprite;
        hairColour.color = playerSprite.hairColour.color;
    }
}
