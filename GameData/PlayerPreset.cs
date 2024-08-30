using System;

[Serializable]
public class PlayerPreset
{
    public string presetName;
    public string presetFilePath;
    public string playerName;
    public string playerGender;
    public string pronounSub;
    public string pronounObj;
    public string pronounGen;

    public string playerNameColour;
    public int headIndex;

    public PlayerSpriteData appearance;

    public void PopulateFromDataManager(DataManagerScript dataManager)
    {
        playerName = dataManager.playerName;
        playerGender = dataManager.playerGender;
        pronounGen = dataManager.pronounGen;
        pronounObj = dataManager.pronounObj;
        pronounSub = dataManager.pronounSub;
        playerNameColour = dataManager.playerNameColour;
        headIndex = dataManager.headIndex;
        appearance = dataManager.playerSprite;
    }

    public void SaveToDataManager(DataManagerScript dataManager, bool savePersonalia, bool colourOnly)
    {
        if (savePersonalia)
        {
            dataManager.playerName = playerName;
            dataManager.playerGender = playerGender;
            dataManager.pronounGen = pronounGen;
            dataManager.pronounObj = pronounObj;
            dataManager.pronounSub = pronounSub;

            dataManager.playerNameColour = playerNameColour;
        }

        if (!colourOnly)
        {
            dataManager.headIndex = headIndex;

            dataManager.playerSprite = appearance;
        }
        else
        {
            dataManager.playerSprite.eyesHexColour = appearance.eyesHexColour;
            dataManager.playerSprite.hairHexColour = appearance.hairHexColour;
            dataManager.playerSprite.hairAccentHexColour = appearance.hairAccentHexColour;
            dataManager.playerSprite.accessoryHexColour = appearance.accessoryHexColour;
            dataManager.playerSprite.tightsHexColour = appearance.tightsHexColour;
            dataManager.playerSprite.cloakHexColour = appearance.cloakHexColour;
            dataManager.playerSprite.vestHexColour = appearance.vestHexColour;
            dataManager.playerSprite.lipTintHexColour = appearance.lipTintHexColour;
            dataManager.playerSprite.lipTintTransparency = appearance.lipTintTransparency;
        }
    }
}