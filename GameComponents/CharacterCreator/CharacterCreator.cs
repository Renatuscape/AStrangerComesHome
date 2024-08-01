using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    public DataManagerScript dataManager;
    public CharacterPersonaliaChoices personalia;
    public SpriteFactory spriteFactory;
    public GameObject portraitRenderer;
    public GameObject portraitCanvas;

    public PlayerSprite playerSprite;
    public Character playerObject;

    public TextMeshProUGUI hairStyleNumber;
    public TextMeshProUGUI bodyTypeNumber;
    public TextMeshProUGUI bodyToneNumber;
    public TextMeshProUGUI eyesNumber;
    public TextMeshProUGUI characterName;
    public TMP_InputField nameInput;

    public GameObject colourPicker;
    public GameObject iconContainer;
    public DialoguePlayerSprite playerIcon;
    public GameObject accessoryToggleContainer;
    public GameObject placeholderDotAccessory;
    public Toggle accessoryToggle;
    public GameObject accentToggleContainer;
    public GameObject placeholderDotAccent;
    public Toggle accentToggle;
    public Toggle trueNameToggle;

    private void OnEnable()
    {
        if (TransientDataScript.GameState == GameState.CharacterCreation)
        {
            playerSprite.SetExpressionToDefault();

            colourPicker.SetActive(false);
            portraitRenderer.SetActive(true);

            foreach (Transform child in portraitCanvas.transform)
            {
                child.gameObject.SetActive(false);
            }

            portraitCanvas.transform.Find("PlayerPortrait").gameObject.SetActive(true);
            UpdateSpriteFromData();

            hairStyleNumber.text = "0";

            accentToggle.isOn = dataManager.playerSprite.enableAccent;
            ToggleAccent();

            accessoryToggle.isOn = dataManager.playerSprite.enableAccessory;
            ToggleAccessory();

            trueNameToggle.isOn = GlobalSettings.AlwaysTrueNamePlate;
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
        playerIcon.RefreshAllImages();
    }

    public void UpdateSpriteFromData()
    {
        PlayerPreset preset = new();
        preset.PopulateFromDataManager(dataManager);
        ApplyPreset(preset, true, false);

        UpdatePlayerIcon();
    }

    public void ApplyPreset(PlayerPreset preset, bool applyPersonalia, bool colourOnly)
    {
        // APPLY PERSONALIA
        if (applyPersonalia && !colourOnly)
        {
            nameInput.text = preset.playerName;
            characterName.color = TransientDataScript.GetColourFromHex(preset.playerNameColour);
            personalia.SetDropDownsFromPreset(preset);
        }
        else
        {
            nameInput.text = dataManager.playerName;
            characterName.color = TransientDataScript.GetColourFromHex(dataManager.playerNameColour);
        }

        // APPLY APPEARANCE
        PlayerSpriteData playerData;

        if (colourOnly)
        {
            playerData = dataManager.playerSprite;
            bodyToneNumber.text = dataManager.headIndex.ToString();
            playerSprite.ChangeHead(dataManager.headIndex);
        }
        else
        {
            playerData = preset.appearance;

            bodyToneNumber.text = preset.headIndex.ToString();
            playerSprite.ChangeHead(preset.headIndex);

            accentToggle.isOn = preset.appearance.enableAccent;
            ToggleAccent();

            accessoryToggle.isOn = preset.appearance.enableAccessory;
            ToggleAccessory();
        }

        var hairPackage = spriteFactory.hairCatalogue.GetPackageByID(playerData.hairID);
        playerSprite.playerHair.ApplyHairPackage(hairPackage, playerData.enableAccessory, playerData.enableAccent);
        int hairIndex = spriteFactory.hairCatalogue.hairPackages.IndexOf(hairPackage);
        spriteFactory.hairCatalogue.index = hairIndex;
        hairStyleNumber.text = hairIndex.ToString();

        var bodyPackage = spriteFactory.bodyCatalogue.GetPackageByID(playerData.bodyID);
        playerSprite.playerBody.ApplyBodyPackage(bodyPackage);
        int bodyIndex = spriteFactory.bodyCatalogue.bodyPackages.IndexOf(bodyPackage);
        spriteFactory.bodyCatalogue.index = bodyIndex;
        bodyTypeNumber.text = bodyIndex.ToString();

        var eyePackage = spriteFactory.eyesCatalogue.GetPackageByID(playerData.eyesID);
        playerSprite.playerEyes.ApplyEyesPackage(eyePackage);
        int eyeIndex = spriteFactory.eyesCatalogue.eyePackages.IndexOf(eyePackage);
        spriteFactory.eyesCatalogue.index = eyeIndex;
        eyesNumber.text = eyeIndex.ToString();

        // APPLY COLOURS
        Color lipColour = TransientDataScript.GetColourFromHex(preset.appearance.lipTintHexColour);
        playerSprite.lipTint.color = new Color(lipColour.r, lipColour.g, lipColour.b, preset.appearance.lipTintTransparency);

        preset.SaveToDataManager(dataManager, applyPersonalia, colourOnly);
    }

    public void ChangeHair(bool isPrevious)
    {
        var package = spriteFactory.hairCatalogue.GetNextPackageByIndex(isPrevious);

        if (package != null)
        {
            dataManager.playerSprite.hairID = package.hairID;
            playerSprite.playerHair.ApplyHairPackage(package, dataManager.playerSprite.enableAccessory, dataManager.playerSprite.enableAccent);
            hairStyleNumber.text = spriteFactory.hairCatalogue.index.ToString();

            CheckHairToggles(package);
        }

        UpdatePlayerIcon();
    }

    void CheckHairToggles(PlayerHairPackage package)
    {
        if (package.accessoryLines != null)
        {
            accessoryToggleContainer.SetActive(true);
            placeholderDotAccessory.SetActive(false);
        }
        else
        {
            accessoryToggleContainer.SetActive(false);
            placeholderDotAccessory.SetActive(true);
        }

        if (package.backAccent == null && package.frontAccent == null)
        {
            accentToggleContainer.SetActive(false);
            placeholderDotAccent.SetActive(true);
        }
        else
        {
            accentToggleContainer.SetActive(true);
            placeholderDotAccent.SetActive(false);
        }
    }

    public void ChangeBody(bool isPrevious)
    {
        var package = spriteFactory.bodyCatalogue.GetNextPackageByIndex(isPrevious);

        if (package != null)
        {
            dataManager.playerSprite.bodyID = package.bodyID;
            playerSprite.playerBody.ApplyBodyPackage(package);
            bodyTypeNumber.text = spriteFactory.bodyCatalogue.index.ToString();
        }

        UpdatePlayerIcon();
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
        var package = spriteFactory.eyesCatalogue.GetNextPackageByIndex(isPrevious);

        if (package != null)
        {
            dataManager.playerSprite.eyesID = package.eyesID;
            playerSprite.playerEyes.ApplyEyesPackage(package);
            eyesNumber.text = spriteFactory.eyesCatalogue.index.ToString();
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

    public void ToggleAccent()
    {
        dataManager.playerSprite.enableAccent = accentToggle.isOn;
        playerSprite.playerHair.ToggleAccent(dataManager.playerSprite.enableAccent);
        if (!accentToggle.isOn)
        {
            colourPicker.gameObject.SetActive(false);
        }
        UpdatePlayerIcon();
    }

    public void ToggleTrueName()
    {
        GlobalSettings.AlwaysTrueNamePlate = trueNameToggle.isOn;
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
            else if (targetIndex == 5 && !accentToggle.isOn)
            {
                accentToggle.isOn = true;
                ToggleAccent();
            }
        }
        else
        {
            colourPicker.SetActive(false);
        }
    }

    public void FinaliseButton()
    {
        if (!string.IsNullOrEmpty(dataManager.playerName) && dataManager.playerName.Length > 1)
        {
            GlobalSettings.SaveSettings();
            playerSprite.SetExpressionToDefault();
            Character player = Characters.FindByTag("Traveller", gameObject.name);
            player.trueName = dataManager.playerName;
            player.hexColour = dataManager.playerNameColour;
            player.NameSetup();
            DialogueTagParser.UpdateTags(dataManager);
            portraitRenderer.SetActive(false);
            TransientDataScript.ReturnToOverWorld("Character Creator", gameObject);
        }
        else
        {
            LogAlert.QueueTextAlert("Name cannot be blank.");
        }
    }
}