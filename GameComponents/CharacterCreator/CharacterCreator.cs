using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    public DataManagerScript dataManager;
    public TransientDataScript transientData;
    public GameObject portraitRenderer;
    public GameObject portraitCanvas;

    public PlayerSprite playerSprite;
    public Character playerObject;

    public TextMeshProUGUI hairStyleNumber;
    public TextMeshProUGUI bodyTypeNumber;
    public TextMeshProUGUI bodyToneNumber;
    public TextMeshProUGUI mouthNumber;
    public TextMeshProUGUI eyesNumber;
    public TextMeshProUGUI characterName;

    public GameObject colourPicker;
    public GameObject iconContainer;
    public GameObject playerIconPrefab;
    PlayerIconPrefab playerIcon;

    private void Awake()
    {
        CreateIconPrefab();
    }
    private void OnEnable()
    {
        colourPicker.SetActive(false);
        portraitRenderer.SetActive(true);
        foreach (Transform child in portraitCanvas.transform)
        {
            child.gameObject.SetActive(false);
        }
        portraitCanvas.transform.Find("PlayerPortrait").gameObject.SetActive(true);
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

    void CreateIconPrefab()
    {
        playerIconPrefab.transform.SetParent(iconContainer.transform, false);
        playerIcon = playerIconPrefab.GetComponent<PlayerIconPrefab>();
        playerIcon.playerSprite = playerSprite;
        playerIcon.UpdateImages();
    }
    public void UpdateSpriteFromData()
    {
        hairStyleNumber.text = dataManager.hairIndex.ToString();
        bodyTypeNumber.text = dataManager.bodyIndex.ToString();
        bodyToneNumber.text = dataManager.headIndex.ToString();
        mouthNumber.text = dataManager.mouthIndex.ToString();
        eyesNumber.text = dataManager.eyesIndex.ToString();

        playerSprite.UpdateAllFromGameData();
        playerIcon.UpdateImages();
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
                dataManager.hairIndex = playerSprite.playerHairColours.Count - 1;
        }
        else
        {
            if (dataManager.hairIndex < playerSprite.playerHairColours.Count - 1)
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
                dataManager.bodyIndex = playerSprite.playerBodyTypes.Count - 1;
        }
        else
        {
            if (dataManager.bodyIndex < playerSprite.playerBodyTypes.Count - 1)
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
                dataManager.headIndex = playerSprite.playerHeads.Count - 1;
        }
        else
        {
            if (dataManager.headIndex < playerSprite.playerHeads.Count - 1)
            {
                dataManager.headIndex++;
            }
            else
                dataManager.headIndex = 0;
        }

        UpdateSpriteFromData();
    }

    public void ChangeMouth(bool isPrevious)
    {
        if (isPrevious)
        {
            if (dataManager.mouthIndex > 0)
            {
                dataManager.mouthIndex--;
            }
            else
                dataManager.mouthIndex = playerSprite.playerMouths.Count - 1;
        }
        else
        {
            if (dataManager.mouthIndex < playerSprite.playerMouths.Count - 1)
            {
                dataManager.mouthIndex++;
            }
            else
                dataManager.mouthIndex = 0;
        }

        UpdateSpriteFromData();
    }

    public void ChangeEyes(bool isPrevious)
    {
        if (isPrevious)
        {
            if (dataManager.eyesIndex > 0)
            {
                dataManager.eyesIndex--;
            }
            else
                dataManager.eyesIndex = playerSprite.playerEyes.Count - 1;
        }
        else
        {
            if (dataManager.eyesIndex < playerSprite.playerEyes.Count - 1)
            {
                dataManager.eyesIndex++;
            }
            else
                dataManager.eyesIndex = 0;
        }

        UpdateSpriteFromData();
    }

    public void EnableColourPicker(int targetIndex)
    {
        colourPicker.SetActive(false);
        colourPicker.GetComponent<ColourPicker>().targetIndex = targetIndex;
        colourPicker.SetActive(true);
    }

    public void FinaliseButton()
    {
        Character player = Characters.FindByTag("Traveller", gameObject.name);
        player.trueName = dataManager.playerName;
        player.hexColour = dataManager.playerNameColour;
        player.NameSetup();
        DialogueTagParser.UpdateTags(dataManager);
        portraitRenderer.SetActive(false);
        TransientDataScript.ReturnToOverWorld("Character Creator", gameObject);
    }
}