using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSprite : MonoBehaviour
{
    public DataManagerScript dataManager;
    public List<Sprite> playerHairColours;
    public List<Sprite> playerHairLineworks;
    public List<Sprite> playerHairOutlines;
    public List<Sprite> playerBodyTypes;
    public List<Sprite> playerHeads;
    public List<Sprite> playerMouths;
    public List<Sprite> playerEyes;

    public Image hairColour;
    public Image hairLinework;
    public Image hairOutline;

    public Image body;
    public Image head;
    public Image mouth;
    public Image eyes;
    public Image irises;

    private void OnEnable()
    {
        UpdateAllFromGameData();
    }

    public void ChangeHair(int index)
    {
        hairColour.sprite = playerHairColours[index];
        hairLinework.sprite = playerHairLineworks[index];
        hairOutline.sprite = playerHairOutlines[index];
    }

    public void ChangeBody(int index)
    {
        body.sprite = playerBodyTypes[index];
    }
    public void ChangeHead(int index)
    {
        head.sprite = playerHeads[index];
    }
    public void ChangeMouth(int index)
    {
        mouth.sprite = playerMouths[index];
    }
    public void ChangeEyes(int index)
    {
        eyes.sprite = playerEyes[index];
    }
    
    public void ColourHair(string hexColour)
    {
        if (ColorUtility.TryParseHtmlString("#" + hexColour, out Color hairColor))
        {
            hairColour.color = hairColor;
        }
        else
        {
            hairColour.color = Color.white; // Default to white if parsing fails
        }
    }

    public void ColourEyes(string hexColour)
    {
        // Apply eye color
        if (ColorUtility.TryParseHtmlString("#" + hexColour, out Color eyeColor))
        {
            irises.color = eyeColor;
        }
        else
        {
            irises.color = Color.white;
        }
    }
    public void UpdateSpriteIndexFromGameData()
    {
        ChangeHair(dataManager.hairIndex);
        ChangeBody(dataManager.bodyIndex);
        ChangeHead(dataManager.headIndex);
        ChangeEyes(dataManager.eyesIndex);
        ChangeMouth(dataManager.mouthIndex);
    }

    public void UpdateSpriteColourFromGameData()
    {
        ColourHair(dataManager.hairHexColour);
        ColourEyes(dataManager.eyesHexColour);

    }

    public void UpdateAllFromGameData()
    {
        UpdateSpriteIndexFromGameData();
        UpdateSpriteColourFromGameData();
    }
}
