using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    public DataManagerScript dataManager;
    public TransientDataScript transientData;
    public Character playerObject;
    public Texture2D combinedTexture;
    public int width = 877;
    public int height = 1240;

    public List<Sprite> playerHairList;
    public List<Sprite> playerHairOutlineList;
    public List<Sprite> playerBodyList;
    public List<Sprite> playerHeadList;

    public Image playerHair;
    public Image playerHairOutline;
    public Image playerEyes;
    public Image playerBody;
    public Image playerHead;
    public TextMeshProUGUI hairStyleNumber;
    public TextMeshProUGUI bodyTypeNumber;
    public TextMeshProUGUI bodyToneNumber;
    public TextMeshProUGUI characterName;

    public GameObject colourPicker;
    public GameObject facialFeatures;

    private void OnEnable()
    {
        facialFeatures.SetActive(false);
        colourPicker.SetActive(false);
        UpdateSpriteFromData();
    }

    void Update()
    {
        if (TransientDataScript.GameState != GameState.CharacterCreation)
            gameObject.SetActive(false);

        if (dataManager.playerName != characterName.text)
        {
            dataManager.playerName = characterName.text;
        }
    }
    public void UpdateSpriteFromData()
    {
        playerHair.sprite = playerHairList[dataManager.hairIndex];
        playerHairOutline.sprite = playerHairOutlineList[dataManager.hairIndex];
        playerBody.sprite = playerBodyList[dataManager.bodyIndex];
        playerHead.sprite = playerHeadList[dataManager.headIndex];

        hairStyleNumber.text = $"{dataManager.hairIndex}";
        bodyTypeNumber.text = $"{dataManager.bodyIndex}";
        bodyToneNumber.text = $"{dataManager.headIndex}";

        // Apply hair color
        Color hairColor;
        if (ColorUtility.TryParseHtmlString("#" + dataManager.hairHexColour, out hairColor))
        {
            playerHair.color = hairColor;
        }
        else
        {
            playerHair.color = Color.white; // Default to white if parsing fails
        }

        // Apply eye color
        Color eyeColor;
        if (ColorUtility.TryParseHtmlString("#" + dataManager.eyesHexColour, out eyeColor))
        {
            playerEyes.color = eyeColor;
        }
        else
        {
            playerEyes.color = Color.white;
        }
    }

    public void ChangeHair(bool isPrevious)
    {
        if (isPrevious)
        {
            if (dataManager.hairIndex > 0)
            {
                dataManager.hairIndex--;
            }
            else
                dataManager.hairIndex = playerHairList.Count - 1;
        }
        else
        {
            if (dataManager.hairIndex < playerHairList.Count - 1)
            {
                dataManager.hairIndex++;
            }
            else
                dataManager.hairIndex = 0;
        }

        UpdateSpriteFromData();
    }

    public void ChangeBody(bool isPrevious)
    {
        if (isPrevious)
        {
            if (dataManager.bodyIndex > 0)
            {
                dataManager.bodyIndex--;
            }
            else
                dataManager.bodyIndex = playerBodyList.Count - 1;
        }
        else
        {
            if (dataManager.bodyIndex < playerBodyList.Count - 1)
            {
                dataManager.bodyIndex++;
            }
            else
                dataManager.bodyIndex = 0;
        }

        UpdateSpriteFromData();
    }

    public void ChangeTone(bool isPrevious)
    {
        if (isPrevious)
        {
            if (dataManager.headIndex > 0)
            {
                dataManager.headIndex--;
            }
            else
                dataManager.headIndex = playerHeadList.Count - 1;
        }
        else
        {
            if (dataManager.headIndex < playerHeadList.Count - 1)
            {
                dataManager.headIndex++;
            }
            else
                dataManager.headIndex = 0;
        }

        UpdateSpriteFromData();
    }

    public void EnableColourPicker(int targetIndex)
    {
        facialFeatures.SetActive(false);
        colourPicker.SetActive(false);
        colourPicker.GetComponent<ColourPicker>().targetIndex = targetIndex;
        colourPicker.SetActive(true);
    }

    public void EnableFeaturePicker()
    {
        colourPicker.SetActive(false);
        facialFeatures.SetActive(true);
    }

    public void FinaliseButton()
    {
        TransientDataScript.ReturnToOverWorld("Character Creator", gameObject);
    }
}