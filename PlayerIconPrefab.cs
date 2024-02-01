using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIconPrefab : MonoBehaviour
{
    public PlayerSprite playerSprite;
    public Image head;
    public Image eyeColour;
    public Image eyeLines;
    public Image mouth;
    public Image hairOutline;
    public Image hairColour;
    public Image hairLinework;

    private void Awake()
    {
        head = transform.Find("Head").GetComponent<Image>();
        eyeColour = transform.Find("Eyes_Irises").GetComponent<Image>();
        eyeLines = transform.Find("Eyes_Lines").GetComponent<Image>();
        mouth = transform.Find("Mouth").GetComponent<Image>();
        hairOutline = transform.Find("Hair_Outline").GetComponent<Image>();
        hairColour = transform.Find("Hair_Colour").GetComponent<Image>();
        hairLinework = transform.Find("Hair_Linework").GetComponent<Image>();

        if (playerSprite is not null)
        {
            UpdateImages();
        }
    }

    public bool UpdateImages()
    {
        if (head != null)
        {
            head.sprite = playerSprite.head.sprite;
        }
        else
        {
            return false;
        }

        if (eyeColour != null)
        {
            eyeColour.color = playerSprite.irises.color;
            GetComponent<Image>().color = new Color(eyeColour.color.g - 0.5f, eyeColour.color.b - 0.2f, eyeColour.color.r - 0.2f, 0.5f);
        }
        else
        {
            return false;
        }

        if (eyeLines != null)
        {
            eyeLines.sprite = playerSprite.eyes.sprite;

        }
        else
        {
            return false;
        }

        if (mouth != null)
        {

            mouth.sprite = playerSprite.mouth.sprite;

        }
        else
        {
            return false;
        }

        if (hairOutline != null)
        {

            hairOutline.sprite = playerSprite.hairOutline.sprite;

        }
        else
        {
            return false;
        }

        if (hairLinework != null)
        {

            hairLinework.sprite = playerSprite.hairLinework.sprite;

        }
        else
        {
            return false;
        }

        if (hairColour != null)
        {

            hairColour.sprite = playerSprite.hairColour.sprite;
            hairColour.color = playerSprite.hairColour.color;

        }
        else
        {
            return false;
        }

        return true;
    }
}
