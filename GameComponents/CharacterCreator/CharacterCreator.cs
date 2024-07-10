using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    public DataManagerScript dataManager;
    public CharacterHairCatalogue hairCatalogue;
    public CharacterEyesCatalogue eyesCatalogue;
    public GameObject portraitRenderer;
    public GameObject portraitCanvas;

    public PlayerSprite playerSprite;
    public Character playerObject;

    public TextMeshProUGUI hairStyleNumber;
    public TextMeshProUGUI bodyTypeNumber;
    public TextMeshProUGUI bodyToneNumber;
    public TextMeshProUGUI eyesNumber;
    public TextMeshProUGUI characterName;

    public GameObject colourPicker;
    public GameObject iconContainer;
    public DialoguePlayerSprite playerIcon;
    public GameObject accessoryToggleContainer;
    public Toggle accessoryToggle;

    private void OnEnable()
    {
        if (TransientDataScript.GameState == GameState.CharacterCreation)
        {
            UpdatePlayerIcon();

            colourPicker.SetActive(false);
            portraitRenderer.SetActive(true);
            foreach (Transform child in portraitCanvas.transform)
            {
                child.gameObject.SetActive(false);
            }
            portraitCanvas.transform.Find("PlayerPortrait").gameObject.SetActive(true);
            UpdateSpriteFromData();

            hairStyleNumber.text = "0";

            accessoryToggle.isOn = dataManager.playerSprite.enableAccessory;
            ToggleAccessory();
        }
    }

    void Update()
    {
        if (TransientDataScript.GameState != GameState.CharacterCreation)
            gameObject.SetActive(false);

        if (dataManager.playerName != characterName.text)
        {
            dataManager.playerName = characterName.text;
        }

        if (!portraitRenderer.activeInHierarchy)
        {
            portraitRenderer.SetActive(true);
        }
    }

    void UpdatePlayerIcon()
    {
        // Replace with less expensive update menu in the future
        playerIcon.gameObject.SetActive(false);
        playerIcon.gameObject.SetActive(true);
    }

    public void UpdateSpriteFromData()
    {
        bodyTypeNumber.text = dataManager.bodyIndex.ToString();
        bodyToneNumber.text = dataManager.headIndex.ToString();

        playerSprite.UpdateAllFromGameData();
        UpdatePlayerIcon();
    }

    public void ChangeHair(bool isPrevious)
    {
        var package = hairCatalogue.GetNextPackageByIndex(isPrevious);

        if (package != null)
        {
            dataManager.playerSprite.hairID = package.hairID;
            playerSprite.playerHair.ApplyHairPackage(package, dataManager.playerSprite.enableAccessory, dataManager.playerSprite.enableAccent);
            hairStyleNumber.text = hairCatalogue.index.ToString();

            if (package.accessoryLines != null)
            {
                accessoryToggleContainer.SetActive(true);
            }
            else
            {
                accessoryToggleContainer.SetActive(false);
            }
        }
        
        UpdatePlayerIcon();
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

    public void ChangeEyes(bool isPrevious)
    {
        var package = eyesCatalogue.GetNextPackageByIndex(isPrevious);

        if (package != null)
        {
            dataManager.playerSprite.eyesID = package.eyesID;
            playerSprite.playerEyes.ApplyEyesPackage(package);
            eyesNumber.text = eyesCatalogue.index.ToString();
        }

        UpdatePlayerIcon();
    }

    public void ToggleAccessory()
    {
        dataManager.playerSprite.enableAccessory = accessoryToggle.isOn;
        playerSprite.playerHair.ToggleAccessory(dataManager.playerSprite.enableAccessory);
        if (!accessoryToggle.isOn)
        {
            colourPicker.gameObject.SetActive(false);
        }
        UpdatePlayerIcon();
    }

    public void EnableColourPicker(int targetIndex)
    {
        var oldIndex = colourPicker.GetComponent<ColourPicker>().targetIndex;

        if (targetIndex != oldIndex || !colourPicker.gameObject.activeInHierarchy)
        {
            colourPicker.SetActive(false);
            colourPicker.GetComponent<ColourPicker>().targetIndex = targetIndex;
            colourPicker.SetActive(true);

            if (targetIndex == 3 && !accessoryToggle.isOn)
            {
                accessoryToggle.isOn = true;
                ToggleAccessory();
            }
        }
        else
        {
            colourPicker.SetActive(false);
        }
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